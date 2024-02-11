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

        newGameBtn.onClick.AddListener(PlayTimeline);//newgame点击后播放Timeline而非Newgame
        continueBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);

        director = FindObjectOfType<PlayableDirector>();
        director.stopped += NewGame;//在结束的时候播放NewGame方法
        //报错没有与参数匹配的方法——这是个委托so必须参数相同，把下面的NewGame添加一个参数PlayableDirector
    }

    void PlayTimeline()//播放Timeline
    {
        director.Play();

    }
    void NewGame(PlayableDirector obj)
    {
        PlayerPrefs.DeleteAll();
        //转换场景
        SceneController.Instance.TransitionToFirstLevel();
    }

    void ContinueGame()
    {
        //转换场景，读取进度
        SceneController.Instance.TransitionToLoadGame();
    }
    void QuitGame()
    {
        Application.Quit();
        Debug.Log("退出游戏");
    }
}
