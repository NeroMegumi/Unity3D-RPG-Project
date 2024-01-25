using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStates { HitPlayer,HitEnemy,HitNothing};
    public RockStates rockStates;
    private Rigidbody rb;
    [Header("Basic Settings")]

    public float force;
    public int damage;
    public GameObject target;
    public GameObject breakEffect;
    private Vector3 direction;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        rockStates = RockStates.HitPlayer;
        FlyToTarget();
    }

    private void FixedUpdate()
    {
        /*Debug.Log(rb.velocity.sqrMagnitude);*/
        if (rb.velocity.sqrMagnitude < 1.0f)
        {
            rockStates = RockStates.HitNothing;
        }
    }
    public void FlyToTarget()
    {
        direction = (target.transform.position - transform.position + Vector3.up).normalized;//Vector3.up����0,1,0
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    //�������ں���
    void OnCollisionEnter(Collision collision)
    {
        //�жϷ�����ײʱ�Է���ʲô���л���ͬ״̬
        switch (rockStates)
        {
            case RockStates.HitPlayer:
                if (collision.gameObject.CompareTag("Player"))//����ң�������ң�����˺�
                {
                    collision.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    collision.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;

                    collision.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    collision.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, collision.gameObject.GetComponent<CharacterStats>());

                    rockStates = RockStates.HitNothing;
                }
                break;

            case RockStates.HitEnemy:
                if (collision.gameObject.GetComponent<Golem>())//���Բ��ñ�ǩ�ж�
                {
                    //ʵ������˺�,�����ͼ��������д��
                    var otherStats = collision.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
        }
    }
}
