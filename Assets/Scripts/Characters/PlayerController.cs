using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;//NavMeshAgent本来就是个类
    private GameObject attackTarget;//攻击对象
    private CharacterStats characterStats;//这样就可以使用定义的数据模板了
    public Animator anmi;
    public CharacterController cc;

    private float h, v;
    private float lastAttackTime ;//攻击CD
    private float stopDistance;

    //角色状态动画的bool变量
    private bool isDead;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();//GetComponet是个泛型方法，()是参数列表
        anmi = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        characterStats = GetComponent<CharacterStats>();
        stopDistance = GetComponent<NavMeshAgent>().stoppingDistance;
    }

    private void OnEnable()
    {
        //首先实例化MouseManager
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
        //TODO:跑太快了
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

    public void MoveToTarget(Vector3 target)//点地板时——日常移动
    {
        StopAllCoroutines();//终止所有协程，达到停止攻击
        if (isDead) return;
        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;
        agent.destination = target;
    }
    public void EventAttack(GameObject target)//前面加一个Event表示是个事件
    {
        if (isDead) return;
        if (target != null)
        {
            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.critricalChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    //TODO:这块按理来说可以放在MoveToTarget
    IEnumerator MoveToAttackTarget()//移动到攻击目标。这块按理来说可以放在MoveToTarget
    {
        agent.isStopped = false;//重新赋为false防止走到敌人处后就走不动了//——但是在MoveToTarget解决了
        agent.stoppingDistance = characterStats.attackData.attackRange;

        transform.LookAt(attackTarget.transform);

        while ( Vector3.Distance(attackTarget.transform.position,transform.position) > characterStats.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }
        
        agent.isStopped = true;//保证到达目标后不动了


        //Attack
        if (lastAttackTime <= 0)
        {
            anmi.SetBool("Critical", characterStats.isCritical);
            anmi.SetTrigger("Attack");
            //重置攻击冷却时间
            lastAttackTime = characterStats.attackData.coolDown;//0.5s的冷却时间
        }

    }
    //Animation Event
    void Hit()
    {
        if (attackTarget.gameObject.CompareTag("Attackable"))
        {
            //什么时候让attackTarget为attackable呢——鼠标点击的时候，到MouseManager中
            if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing)//判断是不是石头——以后可能有别的可攻击物体
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();//通过这种方式获得敌人的数据

            targetStats.TakeDamage(characterStats, targetStats);//记得添加暴击判断
        }
        
    }

}
