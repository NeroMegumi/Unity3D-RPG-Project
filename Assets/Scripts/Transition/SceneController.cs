using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : Singleton<SceneController>,IEndGameObserver
{
    public GameObject playerPrefab;
    GameObject player;
    NavMeshAgent playerAgent;
    public SceneFader sceneFaderPrefab;
    bool fadeFinished;//防止玩家死亡后不断地loadMain

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        fadeFinished = true;
        GameManager.Instance.addObserver(this);//注册观察者
    }
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        //先判断同/异场景
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name,transitionPoint.destinationTag));
                //SceneManager是导入的名字空间中的一个系统自带类，获得当前激活场景的名字
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }

    //使用协程——异步加载需要等待场景加载完才就绪
    IEnumerator Transition(string sceneName,TransitionDestination.DestinationTag destinationTag)// C# 中，枚举类型的成员默认是 public、static；别的必须先实例化
    {
        //TODO:保存数据——传送之前保存
        SaveManager.Instance.SavePlayerData();

        if (SceneManager.GetActiveScene().name != sceneName)//如果当前场景名字和要传送场景名字不同
        {
            yield return SceneManager.LoadSceneAsync(sceneName);//yield return基本理解：当前帧等待return后的语句完成，完成后才能执行下一行的命令
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);//等待人物生成
            //读取数据
            SaveManager.Instance.LoadPlayerData();
            yield break;//中断协程  

        }
        else
        {
            //把玩家传送过去——先得到玩家数据
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;
            //使用api设置坐标和旋转。传送门蓝点的坐标和旋转——通过传入Tag进函数获取，返回TransitionPoint
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            //返回与 TransitionDestination 组件关联的游戏对象的 Transform 组件
            playerAgent.enabled = true;
            yield return null;
        }
        
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();//这里返回的是TransitionDestination类型数组

        //找到目标Tag的传送门
        for(int i = 0; i < entrances.Length; i++)
        {
            if(entrances[i].destinationTag == destinationTag)
            {
                return entrances[i];
            }
        }
        return null;
    }
    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }

    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    //协程不能直接调用所以需要写成函数供MainMenu.cs调用
    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("Game"));
    }

    IEnumerator LoadLevel(string scene)
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        if(scene != "")
        {
            yield return StartCoroutine(fade.FadeOut());
            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);
            //获取传送点位置——游戏一直存在的组件中去获得——GameManager，类似上面GetDestination的方法

            //保存数据——存疑，不确定必不必要
            SaveManager.Instance.SavePlayerData();
            yield return StartCoroutine(fade.FadeIn());

            fadeFinished = true;//死亡后重新进入游戏一定会加载场景，此时再让fadeFinished为true
            yield break;
        }
    }

    IEnumerator LoadMain()
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut());
        yield return SceneManager.LoadSceneAsync("Main");
        yield return StartCoroutine(fade.FadeIn());
        yield break;
    }

    public void EndNotify()
    {
        if (fadeFinished)
        {
            fadeFinished = false;
            StartCoroutine(LoadMain());
        }

    }
}

