using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    [Header("Skill")]
    public float kickForce = 10; //击退的力

    public void KickOff()//动画调用的event
    {
        if(attackTarget != null)//先在enemycontroller中设置为protected
        {
            transform.LookAt(attackTarget.transform);//首先看向玩家

            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();//量化为1，0，或者-1

            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;//停止角色移动
            attackTarget.GetComponent<NavMeshAgent>().ResetPath();
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;//击退效果，用速度代替.方向乘力
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");

            
        }
    }

}
