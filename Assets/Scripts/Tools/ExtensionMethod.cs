using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethod
{
    private const float dotThreshold = 0.5f;
    public static bool IsFacingTarget(this Transform transform,Transform target)//��һ�����ǲ�����������չ�ĸ��ࣨ�ķ��������ڶ������β�
    {
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();

        float dot = Vector3.Dot(transform.forward, vectorToTarget);
        //Dot������Լ���ǰ����������Լ��ļнǵĵ�����˴���Ϊ���Ƿ������������Ծ�������ֵ��
        //�������0.5�Ǿ��Ǵ���60�ȣ��������Լ�ǰ���120�ȵ����η�Χ��
        return dot >= dotThreshold;
    }
}
