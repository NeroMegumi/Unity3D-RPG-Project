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

    //���õ��Ĺ�������������Щ�����е����ݣ�ֻҪ�����и�ֵ����
    [Header("Kill")]
    public int killExp;//����Ļ�ɱ����

    [Header("Level")]
    public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;
    public float levelBuff;//����������

    public float levelExpMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }//����������������ľ���ҲԽ��Խ��
    }

    public void UpdateExp(int exp)//��������á��������˺���ʱ��
    {
        currentExp += exp;

        if (currentExp >= baseExp)
            LevelUp();
    }

    private void LevelUp()
    {
        //�����������µ����Զ���������
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);//��֤���ص�ֵһ����0��max֮��,���ᳬ��max
        baseExp += (int)(baseExp * levelExpMultiplier);
        
        maxHealth = (int)(maxHealth * levelExpMultiplier);
        currentHealth = maxHealth;

        Debug.Log("LEVEL UP: " + currentLevel + "Max Health: " + maxHealth);

    }
}
