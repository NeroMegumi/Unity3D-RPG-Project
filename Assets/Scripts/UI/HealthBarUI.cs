using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab;
    public Transform barPoint;
    public bool alwaysVisable;//Ѫ���Ƿ�һֱ�ɼ�
    public float visibleTime = 3;//���ӻ�ʱ��

    float timeLeft;//Ѫ�����ܿ��ӻ����

    Image healthSlider;
    Transform UIbar;
    Transform cam;

    CharacterStats currentStats;

    private void Awake()
    {
        currentStats = GetComponent<CharacterStats>();

        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;//�ڴ˴�ע��
    }

    private void OnEnable()
    {
        cam = Camera.main.transform;

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())//�����кܶ�canvas��������ϵͳ�ȵ�
        {
            if(canvas.renderMode == RenderMode.WorldSpace)//������������꣨�׶���������������������������canvas�Ͳ��У���������ֻ��ֻ֤��Ѫ����һ�����������꣩
            {
                UIbar = Instantiate(healthUIPrefab, canvas.transform).transform;//����Ԥ���嶼��Ҫ������Ȼ��ֵ��һ������
                                                                                //����Ѫ��UI���������꣨�����жϹ�canvas��renderMode�Ѿ���WorldSpace�ˣ�
                                                                                //��transform��UIbar
                healthSlider = UIbar.GetChild(0).GetComponent<Image>();//����������transform�����currentHealth���ٻ�ȡ�����Image���
                UIbar.gameObject.SetActive(alwaysVisable);//gameobject��һ������ �͸�java�����thisһ���� ָ��������ű������ŵ���Ϸ���
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
        
        UIbar.gameObject.SetActive(true);//�յ���������ǿ�пɼ�
        timeLeft = visibleTime;//ÿ���ܵ�����������Ѫ����ʾ��ʱ��

        //��ΪFill Amount��0��1�����Դ���һ���ٷֱ�
        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;
        
    }

    //���Ѫ��������
    private void LateUpdate()//��update����һִ֡��
    {
        if(UIbar != null)
        {
            UIbar.position = barPoint.position;//����barPoint
            UIbar.forward = -cam.forward;//���������������෴������֤ת���ӽ�Ҳʼ����ƽ��

            if (timeLeft <= 0 && !alwaysVisable)
                UIbar.gameObject.SetActive(false);
            else if (timeLeft > 0 && !alwaysVisable)
                timeLeft -= Time.deltaTime;
        }
    }


}
