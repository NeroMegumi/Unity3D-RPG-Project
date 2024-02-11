using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    CanvasGroup CanvasGroup;

    public float fadeInDuration;//淡入持续时间
    public float fadeOutDuration;//淡出持续时间

    private void Awake()
    {
        CanvasGroup = GetComponent<CanvasGroup>();

        DontDestroyOnLoad(gameObject);//不要在场景转换时把我销毁
    }


    //在转换场景的同时淡入淡出——同时执行就用协程IEnumerator
/*    public IEnumerator FadeOutIn()
    {
        yield return FadeOut(fadeOutDuration);
        yield return FadeIn(fadeInDuration);
    }*/
    public IEnumerator FadeOut()
    {
        while(CanvasGroup.alpha < 1)//alpha 从0到1
        {
            CanvasGroup.alpha += Time.deltaTime / fadeOutDuration;//比如fadeOutDuration是3，那么每帧的时间累加，直到加起来是3s则alpha就是1
            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        while (CanvasGroup.alpha > 0)//alpha 从1到0
        {
            CanvasGroup.alpha -= Time.deltaTime / fadeInDuration;//比如time是3，那么每帧的时间累加，直到加起来是3s则alpha就是1
            yield return null;
        }
        Destroy(gameObject);
    }
}
