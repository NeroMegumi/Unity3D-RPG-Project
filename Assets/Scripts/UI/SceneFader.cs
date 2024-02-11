using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    CanvasGroup CanvasGroup;

    public float fadeInDuration;//�������ʱ��
    public float fadeOutDuration;//��������ʱ��

    private void Awake()
    {
        CanvasGroup = GetComponent<CanvasGroup>();

        DontDestroyOnLoad(gameObject);//��Ҫ�ڳ���ת��ʱ��������
    }


    //��ת��������ͬʱ���뵭������ͬʱִ�о���Э��IEnumerator
/*    public IEnumerator FadeOutIn()
    {
        yield return FadeOut(fadeOutDuration);
        yield return FadeIn(fadeInDuration);
    }*/
    public IEnumerator FadeOut()
    {
        while(CanvasGroup.alpha < 1)//alpha ��0��1
        {
            CanvasGroup.alpha += Time.deltaTime / fadeOutDuration;//����fadeOutDuration��3����ôÿ֡��ʱ���ۼӣ�ֱ����������3s��alpha����1
            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        while (CanvasGroup.alpha > 0)//alpha ��1��0
        {
            CanvasGroup.alpha -= Time.deltaTime / fadeInDuration;//����time��3����ôÿ֡��ʱ���ۼӣ�ֱ����������3s��alpha����1
            yield return null;
        }
        Destroy(gameObject);
    }
}
