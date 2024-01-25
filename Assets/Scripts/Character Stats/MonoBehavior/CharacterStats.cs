using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;
    public CharacterData_SO templateData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    [HideInInspector]
    public bool isCritical;

    void Awake()
    {
        if (templateData != null)
            characterData = Instantiate(templateData);
    }

    #region Read from Data_SO
    public int MaxHealth
    {
        get { return characterData ? characterData.maxHealth : 0; }
        set { characterData.maxHealth = value; }//value�������洫������ֵ
    }
    public int currentHealth
    {
        get { return characterData ? characterData.currentHealth : 0; }
        set { characterData.currentHealth = value; }//value�������洫������ֵ
    }
    public int baseDefence
    {
        get { return characterData ? characterData.baseDefence : 0; }
        set { characterData.baseDefence = value; }//value�������洫������ֵ
    }
    public int currentDefence
    {
        get { return characterData ? characterData.currentDefence : 0; }
        set { characterData.currentDefence = value; }//value�������洫������ֵ
    }
    #endregion

    #region
    public void TakeDamage(CharacterStats attacker,CharacterStats defender)
    {
        int damage = Mathf.Max(attacker.currentDamage() - defender.currentDefence,0);
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        if (attacker.isCritical)
        {
            defender.GetComponent<Animator>().SetTrigger("Hit");
        }
        //update UI �˺���ʾ��UI
        UpdateHealthBarOnAttack?.Invoke(currentHealth, MaxHealth);
        //����Update
        if (currentHealth <= 0)//��ǰ��ɫѪ��Ϊ0
            attacker.characterData.UpdateExp(characterData.killExp);//�Ѿ���������� 
    }

    public void TakeDamage(int damage,CharacterStats defender)
    {
        int currentDamage = Mathf.Max(damage - defender.currentDefence,0);//�˺�����С��0
        currentHealth = Mathf.Max(currentHealth - currentDamage, 0);
        UpdateHealthBarOnAttack?.Invoke(currentHealth, MaxHealth);

        if (currentHealth <= 0)//��ǰ��ɫѪ��Ϊ0����û��attacker
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killExp);
    }
    private int currentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage,attackData.maxDamage);
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultipler;
            Debug.Log("����" + coreDamage);
        }
        return (int)coreDamage;

    }
    #endregion

}
