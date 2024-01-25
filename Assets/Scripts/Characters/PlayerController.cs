using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;//NavMeshAgent�������Ǹ���
    private GameObject attackTarget;//��������
    private CharacterStats characterStats;//�����Ϳ���ʹ�ö��������ģ����
    public Animator anmi;
    public CharacterController cc;

    private float h, v;
    private float lastAttackTime ;//����CD
    private float stopDistance;

    //��ɫ״̬������bool����
    private bool isDead;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();//GetComponet�Ǹ����ͷ�����()�ǲ����б�
        anmi = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        characterStats = GetComponent<CharacterStats>();
        stopDistance = GetComponent<NavMeshAgent>().stoppingDistance;
    }

    private void OnEnable()
    {
        //����ʵ����MouseManager
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.AttackEnemy += EventAttack;
        GameManager.Instance.RegisterPlayer(characterStats);
    }

    private void Start()
    {
        SaveManager.Instance.LoadPlayerData();
        //characterStats.MaxHealth = 2;
    }

    private void OnDisable()
    {
        if (!MouseManager.IsInitialized) return;
        MouseManager.Instance.OnMouseClicked -= MoveToTarget;
        MouseManager.Instance.AttackEnemy -= EventAttack;
    }

    private void Update()
    {
        isDead = characterStats.currentHealth == 0;
        if (isDead)
            GameManager.Instance.NotifyObservers();
        SwitchAnimation();
        //TODO:��̫����
        /*h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        cc.Move(new Vector3 (h, 0,v));*/
        lastAttackTime = lastAttackTime - Time.deltaTime;
    }
    void SwitchAnimation()
    {
        anmi.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anmi.SetBool("Death", isDead);
    }

    public void MoveToTarget(Vector3 target)//��ذ�ʱ�����ճ��ƶ�
    {
        StopAllCoroutines();//��ֹ����Э�̣��ﵽֹͣ����
        if (isDead) return;
        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;
        agent.destination = target;
    }
    public void EventAttack(GameObject target)//ǰ���һ��Event��ʾ�Ǹ��¼�
    {
        if (isDead) return;
        if (target != null)
        {
            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.critricalChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    //TODO:��鰴����˵���Է���MoveToTarget
    IEnumerator MoveToAttackTarget()//�ƶ�������Ŀ�ꡣ��鰴����˵���Է���MoveToTarget
    {
        agent.isStopped = false;//���¸�Ϊfalse��ֹ�ߵ����˴�����߲�����//����������MoveToTarget�����
        agent.stoppingDistance = characterStats.attackData.attackRange;

        transform.LookAt(attackTarget.transform);

        while ( Vector3.Distance(attackTarget.transform.position,transform.position) > characterStats.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }
        
        agent.isStopped = true;//��֤����Ŀ��󲻶���


        //Attack
        if (lastAttackTime <= 0)
        {
            anmi.SetBool("Critical", characterStats.isCritical);
            anmi.SetTrigger("Attack");
            //���ù�����ȴʱ��
            lastAttackTime = characterStats.attackData.coolDown;//0.5s����ȴʱ��
        }

    }
    //Animation Event
    void Hit()
    {
        if (attackTarget.gameObject.CompareTag("Attackable"))
        {
            //ʲôʱ����attackTargetΪattackable�ء����������ʱ�򣬵�MouseManager��
            if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing)//�ж��ǲ���ʯͷ�����Ժ�����б�Ŀɹ�������
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();//ͨ�����ַ�ʽ��õ��˵�����

            targetStats.TakeDamage(characterStats, targetStats);//�ǵ���ӱ����ж�
        }
        
    }

}
