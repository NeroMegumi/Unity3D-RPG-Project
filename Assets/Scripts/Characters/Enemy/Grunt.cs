using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    [Header("Skill")]
    public float kickForce = 10; //���˵���

    public void KickOff()//�������õ�event
    {
        if(attackTarget != null)//����enemycontroller������Ϊprotected
        {
            transform.LookAt(attackTarget.transform);//���ȿ������

            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();//����Ϊ1��0������-1

            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;//ֹͣ��ɫ�ƶ�
            attackTarget.GetComponent<NavMeshAgent>().ResetPath();
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;//����Ч�������ٶȴ���.�������
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");

            
        }
    }

}
