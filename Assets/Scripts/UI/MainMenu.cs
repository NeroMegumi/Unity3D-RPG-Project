using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour
{
    Button newGameBtn;
    Button continueBtn;
    Button quitBtn;
    PlayableDirector director;

    private void Awake()
    {
        newGameBtn = transform.GetChild(1).GetComponent<Button>();
        continueBtn = transform.GetChild(2).GetComponent<Button>();
        quitBtn = transform.GetChild(3).GetComponent<Button>();

        newGameBtn.onClick.AddListener(PlayTimeline);//newgame����󲥷�Timeline����Newgame
        continueBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);

        director = FindObjectOfType<PlayableDirector>();
        director.stopped += NewGame;//�ڽ�����ʱ�򲥷�NewGame����
        //����û�������ƥ��ķ����������Ǹ�ί��so���������ͬ���������NewGame���һ������PlayableDirector
    }

    void PlayTimeline()//����Timeline
    {
        director.Play();

    }
    void NewGame(PlayableDirector obj)
    {
        PlayerPrefs.DeleteAll();
        //ת������
        SceneController.Instance.TransitionToFirstLevel();
    }

    void ContinueGame()
    {
        //ת����������ȡ����
        SceneController.Instance.TransitionToLoadGame();
    }
    void QuitGame()
    {
        Application.Quit();
        Debug.Log("�˳���Ϸ");
    }
}
