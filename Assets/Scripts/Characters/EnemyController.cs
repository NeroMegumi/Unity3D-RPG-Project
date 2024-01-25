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
    public float lookAtTime;//在Inspector里面调整的,public
    private float remainLookAtTime;//只需要在脚本里自己判断和改变就行了，不需要外力改变，private
    private float lastAttackTime;//攻击计时器
    private Quaternion guardRotation;//站桩的初始角度——四元组类型

    private Animator anmi;
    [Header("Patrol Range")]
    public float patrolRange;
    private Vector3 wayPoint;
    private Vector3 guardPos;


    //bool配合动画
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
        guardPos = transform.position;//最开始的位置
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
            GetNewWayPoint();//如果不初始化，就会一开始往地图中心(0,0,0)移动
        }
        //FIXEME:场景切换后修改掉
        GameManager.Instance.addObserver(this);//因为泛型单例里面属性是Instance而非instance所以只能这样
    }

    //切换场景时使用
    void OnEnable()
    {
        //该函数是场景生成了，敌人再生成时激活

        //GameManager.Instance.addObserver(this);//因为泛型单例里面属性是Instance而非instance所以只能这样
        //this是当前gameobject，即敌人本身——但是addObserver这个函数形参是接口IEndGameObserver，这个this也不是吧？怎么回事TODO
    }

    void OnDisable()
    {
        //人物消失调用一次，游戏结束调用一次——编辑器会报错
        if (!GameManager.IsInitialized) return;//游戏结束了就不调用了
        GameManager.Instance.removeObserver(this);
    }

    private void Update()
    {
        if (characterStats.currentHealth == 0) isDead = true;//生命=0死亡
        if (!PlayerDead)
        {
            SwitchStates();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;//在update中更新——人物在任何情况都应该不断减少上一次攻击时间
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
        //如果发现player切换追击
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
            //Debug.Log("找到player");
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
                    //当前位置和guardPos距离小于stoppingDistance就停止
                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);//第三个参数越大转向越快
                    }               
                    
                }

                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;//乘法比除法开销小，性能更好
                //判断是否到了随机巡逻点
                if (Vector3.Distance(transform.position,wayPoint)<=agent.stoppingDistance)//根据sagent.stoppingDistance判断是否移动;
                {
                    isWalk = false;
                    if(remainLookAtTime > 0)//巡逻后望风一段时间
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

                if (!FoundPlayer())//没找到玩家
                {
                    //拉脱回到上一个状态
                    if (remainLookAtTime > 0)//计时器判断 是否还在望风时间
                    {
                        remainLookAtTime -= Time.deltaTime; 
                        agent.destination = this.transform.position;//让它拉脱后立刻停下来
                    }
                    //不在望风时间，判断是站桩or巡逻的敌人
                    else if (isGuard)
                        enemyStates = EnemyStates.GUARD;
                    else
                        enemyStates = EnemyStates.PATROL;
                    isFollow = false;
                    
                }
                else  //发现玩家，一直追击
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                    remainLookAtTime = lookAtTime;
                }
                //在攻击范围内攻击player
                //1.判断是否在近战攻击和远程攻击范围内——>两个bool返回的函数
                if( TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;//关闭跟随动画
                    agent.isStopped = true;//停止移动
                    if( lastAttackTime < 0)
                    {
                        lastAttackTime = characterStats.attackData.coolDown;

                        //暴击判断
                        //首先需要一个bool值判断是否暴击了，在CharacterStats中增加isCritical
                        characterStats.isCritical = Random.value < characterStats.attackData.critricalChance;
                        //执行攻击
                        Attack();
                    }
                }

                break;
            case EnemyStates.DEAD:
                coll.enabled = false;
                //agent.enabled = false;//为了不阻碍玩家移动
                agent.radius = 0;//改成半径为0即可
                Destroy(gameObject, 2f);
                break;
        }  
    }
    void Attack()
    {
        transform.LookAt(attackTarget.transform);

        
        if (TargetInSkillRange())
        {
            //远程攻击动画
            anmi.SetTrigger("Skill");
        }
        if (TargetInAttackRange())
        {
            //近身攻击动画
            anmi.SetTrigger("Attack");
        }

    }
    bool TargetInAttackRange()
    {
        if (attackTarget != null)//避免系统报错
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
        remainLookAtTime = lookAtTime;//重新获取路径点的时候就不需要望风了
        float randomX = Random.Range(-patrolRange,patrolRange);
        float randomZ = Random.Range(-patrolRange,patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);//y不改是为了在山坡等地按照当前的y去改，否则就浮空
        

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
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))//攻击判定时玩家跑开了
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    public void EndNotify()
    {
        //获胜动画
        //停止所有移动
        //停止Agent
        isChase = false;
        isWalk = false;
        PlayerDead = true;
        anmi.SetBool("Win", true);
        attackTarget = null;//没有攻击对象自然不会找

    }
}
