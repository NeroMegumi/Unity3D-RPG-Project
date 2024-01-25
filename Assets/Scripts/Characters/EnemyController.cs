using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD,PATROL,CHASE,DEAD }

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour,IEndGameObserver
{
    private NavMeshAgent agent;
    private EnemyStates enemyStates;
    private Collider coll;
    protected CharacterStats characterStats;

    [Header("Basic Settings")]
    public float sightRadius;
    protected GameObject attackTarget;
    public bool isGuard;
    private float speed;
    public float lookAtTime;//��Inspector���������,public
    private float remainLookAtTime;//ֻ��Ҫ�ڽű����Լ��жϺ͸ı�����ˣ�����Ҫ�����ı䣬private
    private float lastAttackTime;//������ʱ��
    private Quaternion guardRotation;//վ׮�ĳ�ʼ�Ƕȡ�����Ԫ������

    private Animator anmi;
    [Header("Patrol Range")]
    public float patrolRange;
    private Vector3 wayPoint;
    private Vector3 guardPos;


    //bool��϶���
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    bool PlayerDead;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anmi = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();

        speed = agent.speed;
        guardPos = transform.position;//�ʼ��λ��
        guardRotation = transform.rotation;

        remainLookAtTime = lookAtTime;
    }
    void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();//�������ʼ�����ͻ�һ��ʼ����ͼ����(0,0,0)�ƶ�
        }
        //FIXEME:�����л����޸ĵ�
        GameManager.Instance.addObserver(this);//��Ϊ���͵�������������Instance����instance����ֻ������
    }

    //�л�����ʱʹ��
    void OnEnable()
    {
        //�ú����ǳ��������ˣ�����������ʱ����

        //GameManager.Instance.addObserver(this);//��Ϊ���͵�������������Instance����instance����ֻ������
        //this�ǵ�ǰgameobject�������˱���������addObserver��������β��ǽӿ�IEndGameObserver�����thisҲ���ǰɣ���ô����TODO
    }

    void OnDisable()
    {
        //������ʧ����һ�Σ���Ϸ��������һ�Ρ����༭���ᱨ��
        if (!GameManager.IsInitialized) return;//��Ϸ�����˾Ͳ�������
        GameManager.Instance.removeObserver(this);
    }

    private void Update()
    {
        if (characterStats.currentHealth == 0) isDead = true;//����=0����
        if (!PlayerDead)
        {
            SwitchStates();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;//��update�и��¡����������κ������Ӧ�ò��ϼ�����һ�ι���ʱ��
        }
        
        

    }

    void SwitchAnimation()
    {
        anmi.SetBool("Walk", isWalk);
        anmi.SetBool("Chase",isChase);
        anmi.SetBool("Follow",isFollow);
        anmi.SetBool("Critical", characterStats.isCritical);
        anmi.SetBool("Death", isDead);

    } 
    void SwitchStates()
    {
        if (isDead) enemyStates = EnemyStates.DEAD;
        //�������player�л�׷��
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
            //Debug.Log("�ҵ�player");
        }

        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;
                if(transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;
                    //��ǰλ�ú�guardPos����С��stoppingDistance��ֹͣ
                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);//����������Խ��ת��Խ��
                    }               
                    
                }

                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;//�˷��ȳ�������С�����ܸ���
                //�ж��Ƿ������Ѳ�ߵ�
                if (Vector3.Distance(transform.position,wayPoint)<=agent.stoppingDistance)//����sagent.stoppingDistance�ж��Ƿ��ƶ�;
                {
                    isWalk = false;
                    if(remainLookAtTime > 0)//Ѳ�ߺ�����һ��ʱ��
                        remainLookAtTime -= Time.deltaTime;
                    else
                        GetNewWayPoint();
                }else 
                {
                    isWalk=true;
                    agent.destination = wayPoint;
                }
                    break;
            case EnemyStates.CHASE:
                
                
                
                isWalk = false;
                isChase = true;

                agent.speed = speed;

                if (!FoundPlayer())//û�ҵ����
                {
                    //���ѻص���һ��״̬
                    if (remainLookAtTime > 0)//��ʱ���ж� �Ƿ�������ʱ��
                    {
                        remainLookAtTime -= Time.deltaTime; 
                        agent.destination = this.transform.position;//�������Ѻ�����ͣ����
                    }
                    //��������ʱ�䣬�ж���վ׮orѲ�ߵĵ���
                    else if (isGuard)
                        enemyStates = EnemyStates.GUARD;
                    else
                        enemyStates = EnemyStates.PATROL;
                    isFollow = false;
                    
                }
                else  //������ң�һֱ׷��
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                    remainLookAtTime = lookAtTime;
                }
                //�ڹ�����Χ�ڹ���player
                //1.�ж��Ƿ��ڽ�ս������Զ�̹�����Χ�ڡ���>����bool���صĺ���
                if( TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;//�رո��涯��
                    agent.isStopped = true;//ֹͣ�ƶ�
                    if( lastAttackTime < 0)
                    {
                        lastAttackTime = characterStats.attackData.coolDown;

                        //�����ж�
                        //������Ҫһ��boolֵ�ж��Ƿ񱩻��ˣ���CharacterStats������isCritical
                        characterStats.isCritical = Random.value < characterStats.attackData.critricalChance;
                        //ִ�й���
                        Attack();
                    }
                }

                break;
            case EnemyStates.DEAD:
                coll.enabled = false;
                //agent.enabled = false;//Ϊ�˲��谭����ƶ�
                agent.radius = 0;//�ĳɰ뾶Ϊ0����
                Destroy(gameObject, 2f);
                break;
        }  
    }
    void Attack()
    {
        transform.LookAt(attackTarget.transform);

        
        if (TargetInSkillRange())
        {
            //Զ�̹�������
            anmi.SetTrigger("Skill");
        }
        if (TargetInAttackRange())
        {
            //����������
            anmi.SetTrigger("Attack");
        }

    }
    bool TargetInAttackRange()
    {
        if (attackTarget != null)//����ϵͳ����
        {
            
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        }
        else
            return false;
    }
    
    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position,transform.position) <= characterStats.attackData.skillRange;
        else
            return false;
    }

    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position,sightRadius);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }
    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;//���»�ȡ·�����ʱ��Ͳ���Ҫ������
        float randomX = Random.Range(-patrolRange,patrolRange);
        float randomZ = Random.Range(-patrolRange,patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);//y������Ϊ����ɽ�µȵذ��յ�ǰ��yȥ�ģ�����͸���
        

        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1)? hit.position :transform.position;
        
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,sightRadius);
    }

    //Animation Event
    void Hit()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))//�����ж�ʱ����ܿ���
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    public void EndNotify()
    {
        //��ʤ����
        //ֹͣ�����ƶ�
        //ֹͣAgent
        isChase = false;
        isWalk = false;
        PlayerDead = true;
        anmi.SetBool("Win", true);
        attackTarget = null;//û�й���������Ȼ������

    }
}
