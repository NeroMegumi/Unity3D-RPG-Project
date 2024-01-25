using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack",menuName = "Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;
    public float skillRange;//Ô¶³Ì¹¥»÷·¶Î§
    public float coolDown;//ÀäÈ´Ê±¼ä
    public int minDamage;
    public int maxDamage;

    public float criticalMultipler;//±©»÷³ËÉË
    public float critricalChance;//±©»÷ÂÊ

}
