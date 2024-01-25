using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    Text levelText;
    Image healthSlider;
    Image expSlider;

    private void Awake()
    {
        //��������ôд�����������չ�����ײ���bug�����ֱ����Find�������ơ�
        levelText = transform.GetChild(2).GetComponent<Text>();
        healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
    }

    private void Update()
    {
        levelText.text = "LEVEL " + GameManager.Instance.playerStats.characterData.currentLevel.ToString("00");//Ϊ����1��ʾΪ01����ToString
        //����Update������ѯ�ܲ��ã�������¼�invoke�ķ�ʽ
        UpdateHealth();
        UpdateExp();
    }

    void UpdateHealth()
    {
        float sliderPercent = (float)GameManager.Instance.playerStats.currentHealth / GameManager.Instance.playerStats.MaxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    void UpdateExp()
    {
        float sliderPercent = (float)GameManager.Instance.playerStats.characterData.currentExp/ GameManager.Instance.playerStats.characterData.baseExp;
        //��Ϊû�и�expд�����ԣ����Ի�Ҫ����CharacterData
        expSlider.fillAmount = sliderPercent;
    }
}
