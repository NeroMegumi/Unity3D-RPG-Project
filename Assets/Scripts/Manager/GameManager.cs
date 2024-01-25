using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;

    private CinemachineFreeLook followCamara;//需要什么类型的相机类型就是什么

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>(); 
    public void RegisterPlayer(CharacterStats player)
    {
        playerStats = player;

        followCamara = FindObjectOfType<CinemachineFreeLook>();

        if( followCamara!= null)
        {
            followCamara.Follow = playerStats.transform.GetChild(2);
            followCamara.LookAt = playerStats.transform.GetChild(2);
        }

    }

    public void addObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }

    public void removeObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }

    public Transform GetEntrance()
    {
        foreach(var item in FindObjectsOfType<TransitionDestination>())
        {
            if (item.destinationTag == TransitionDestination.DestinationTag.ENTER)
                return item.transform;
        }
        return null;
    }
}
