using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T:Singleton<T> //约束：T必须继承Singleton<T>，否则就能够写Singleton<Animator>这类
    //好处是明确该泛型单例是给哪种类型的类使用的，函数就更有针对性
{
    private static T instance;//私有是为了限制对其直接访问，保证单例只能在类内部进行控制和修改。静态的目的是使得这个字段在整个类的实例中是唯一的。

    public static T Instance
    {
        get { return instance; }//因为private外界不能访问，所以使用属性提供读——但是不让写所以只加get只读
    }

    protected virtual void Awake()//protected只有继承的子类可以访问；virtual--子类必须重写
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = (T)this;//在 Unity 中，this 引用的是当前脚本所挂载的 GameObject，而 (T)this 则表示将当前的 GameObject 强制转换为泛型 T
    }

    public static bool IsInitialized//又一个属性
    {
        get { return instance != null; }//初始化了就返回ture
    }

    protected virtual void OnDestory()//在gameObject被销毁时调用
    {
        if(instance == this)
        {
            instance = null;//为了安全在gameobject销毁后instance也应该为空，避免与物体的生命周期不同——若不为空可能会重复调用Awake里的Destroy
        }
    }
}
