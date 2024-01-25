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
        //不建议这么写，如果后期拓展则容易产生bug。最好直接用Find对象名称。
        levelText = transform.GetChild(2).GetComponent<Text>();
        healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
    }

    private void Update()
    {
        levelText.text = "LEVEL " + GameManager.Instance.playerStats.characterData.currentLevel.ToString("00");//为了让1显示为01加上ToString
        //放在Update里面轮询很不好，最好用事件invoke的方式
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
        //因为没有给exp写成属性，所以还要访问CharacterData
        expSlider.fillAmount = sliderPercent;
    }
}
