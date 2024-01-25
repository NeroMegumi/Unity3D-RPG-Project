using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethod
{
    private const float dotThreshold = 0.5f;
    public static bool IsFacingTarget(this Transform transform,Transform target)//第一个不是参数，代表拓展哪个类（的方法），第二个是形参
    {
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();

        float dot = Vector3.Dot(transform.forward, vectorToTarget);
        //Dot点积：自己的前向与敌人与自己的夹角的点积（此处因为都是方向向量，所以就是余弦值）
        //如果大于0.5那就是大于60度，敌人在自己前向的120度的扇形范围内
        return dot >= dotThreshold;
    }
}
