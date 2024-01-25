using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T:Singleton<T> //Լ����T����̳�Singleton<T>��������ܹ�дSingleton<Animator>����
    //�ô�����ȷ�÷��͵����Ǹ��������͵���ʹ�õģ������͸��������
{
    private static T instance;//˽����Ϊ�����ƶ���ֱ�ӷ��ʣ���֤����ֻ�������ڲ����п��ƺ��޸ġ���̬��Ŀ����ʹ������ֶ����������ʵ������Ψһ�ġ�

    public static T Instance
    {
        get { return instance; }//��Ϊprivate��粻�ܷ��ʣ�����ʹ�������ṩ���������ǲ���д����ֻ��getֻ��
    }

    protected virtual void Awake()//protectedֻ�м̳е�������Է��ʣ�virtual--���������д
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = (T)this;//�� Unity �У�this ���õ��ǵ�ǰ�ű������ص� GameObject���� (T)this ���ʾ����ǰ�� GameObject ǿ��ת��Ϊ���� T
    }

    public static bool IsInitialized//��һ������
    {
        get { return instance != null; }//��ʼ���˾ͷ���ture
    }

    protected virtual void OnDestory()//��gameObject������ʱ����
    {
        if(instance == this)
        {
            instance = null;//Ϊ�˰�ȫ��gameobject���ٺ�instanceҲӦ��Ϊ�գ�������������������ڲ�ͬ��������Ϊ�տ��ܻ��ظ�����Awake���Destroy
        }
    }
}
