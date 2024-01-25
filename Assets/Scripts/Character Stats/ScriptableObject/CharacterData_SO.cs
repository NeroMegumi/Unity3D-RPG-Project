using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Data",menuName ="Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;

    //不用担心怪物或者玩家有这些不该有的数据，只要不进行赋值即可
    [Header("Kill")]
    public int killExp;//怪物的击杀经验

    [Header("Level")]
    public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;
    public float levelBuff;//升级的增幅

    public float levelExpMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }//随着升级升级所需的经验也越来越高
    }

    public void UpdateExp(int exp)//在哪里调用——计算伤害的时候
    {
        currentExp += exp;

        if (currentExp >= baseExp)
            LevelUp();
    }

    private void LevelUp()
    {
        //所有升级更新的属性都在这里面
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);//保证返回的值一定在0到max之间,不会超过max
        baseExp += (int)(baseExp * levelExpMultiplier);
        
        maxHealth = (int)(maxHealth * levelExpMultiplier);
        currentHealth = maxHealth;

        Debug.Log("LEVEL UP: " + currentLevel + "Max Health: " + maxHealth);

    }
}
