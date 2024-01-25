using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab;
    public Transform barPoint;
    public bool alwaysVisable;//血条是否一直可见
    public float visibleTime = 3;//可视化时间

    float timeLeft;//血条还能可视化多久

    Image healthSlider;
    Transform UIbar;
    Transform cam;

    CharacterStats currentStats;

    private void Awake()
    {
        currentStats = GetComponent<CharacterStats>();

        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;//在此处注册
    }

    private void OnEnable()
    {
        cam = Camera.main.transform;

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())//可能有很多canvas，背包，系统等等
        {
            if(canvas.renderMode == RenderMode.WorldSpace)//如果是世界坐标（弊端是如果场景有其他的世界坐标的canvas就不行，但是这里只保证只有血条这一个是世界坐标）
            {
                UIbar = Instantiate(healthUIPrefab, canvas.transform).transform;//这种预制体都需要先生成然后赋值给一个变量
                                                                                //生成血条UI到世界坐标（上面判断过canvas的renderMode已经是WorldSpace了）
                                                                                //把transform给UIbar
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();//按索引返回transform子项——currentHealth，再获取子项的Image组件
                UIbar.gameObject.SetActive(alwaysVisable);//gameobject是一个对象， 就跟java里面的this一样， 指的是这个脚本所附着的游戏物件
            }
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0)
        {
            Destroy(UIbar.gameObject);
            return;
        }
        
        UIbar.gameObject.SetActive(true);//收到攻击必须强行可见
        timeLeft = visibleTime;//每次受到攻击就重置血条显示的时间

        //因为Fill Amount是0到1的所以创建一个百分比
        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;
        
    }

    //解决血条不跟随
    private void LateUpdate()//在update的下一帧执行
    {
        if(UIbar != null)
        {
            UIbar.position = barPoint.position;//跟随barPoint
            UIbar.forward = -cam.forward;//朝向和摄像机朝向相反——保证转动视角也始终是平面

            if (timeLeft <= 0 && !alwaysVisable)
                UIbar.gameObject.SetActive(false);
            else if (timeLeft > 0 && !alwaysVisable)
                timeLeft -= Time.deltaTime;
        }
    }


}
