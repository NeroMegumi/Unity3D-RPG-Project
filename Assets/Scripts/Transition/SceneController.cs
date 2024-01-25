using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : Singleton<SceneController>
{
    public GameObject playerPrefab;
    GameObject player;
    NavMeshAgent playerAgent;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        //���ж�ͬ/�쳡��
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name,transitionPoint.destinationTag));
                //SceneManager�ǵ�������ֿռ��е�һ��ϵͳ�Դ��࣬��õ�ǰ�����������
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }

    //ʹ��Э�̡����첽������Ҫ�ȴ�����������ž���
    IEnumerator Transition(string sceneName,TransitionDestination.DestinationTag destinationTag)// C# �У�ö�����͵ĳ�ԱĬ���� public��static����ı�����ʵ����
    {
        //TODO:�������ݡ�������֮ǰ����
        SaveManager.Instance.SavePlayerData();

        if (SceneManager.GetActiveScene().name != sceneName)//�����ǰ�������ֺ�Ҫ���ͳ������ֲ�ͬ
        {
            yield return SceneManager.LoadSceneAsync(sceneName);//yield return������⣺��ǰ֡�ȴ�return��������ɣ���ɺ����ִ����һ�е�����
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);//�ȴ���������
            //��ȡ����
            SaveManager.Instance.LoadPlayerData();
            yield break;//�ж�Э��  

        }
        else
        {
            //����Ҵ��͹�ȥ�����ȵõ��������
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;
            //ʹ��api�����������ת��������������������ת����ͨ������Tag��������ȡ������TransitionPoint
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            //������ TransitionDestination �����������Ϸ����� Transform ���
            playerAgent.enabled = true;
            yield return null;
        }
        
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();//���ﷵ�ص���TransitionDestination��������

        //�ҵ�Ŀ��Tag�Ĵ�����
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

    //Э�̲���ֱ�ӵ���������Ҫд�ɺ�����MainMenu.cs����
    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("Game"));
    }

    IEnumerator LoadLevel(string scene)
    {
        if(name != "")
        {
            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);
            //��ȡ���͵�λ�á�����Ϸһֱ���ڵ������ȥ��á���GameManager����������GetDestination�ķ���

            //�������ݡ������ɣ���ȷ���ز���Ҫ
            SaveManager.Instance.SavePlayerData();
            yield break;
        }
    }

    IEnumerator LoadMain()
    {
        yield return SceneManager.LoadSceneAsync("Main");
        yield break;
    }
}

