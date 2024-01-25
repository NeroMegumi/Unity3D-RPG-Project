using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    string sceneName = "";
    public string SceneName { get { return PlayerPrefs.GetString(sceneName); } }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneController.Instance.TransitionToMain();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SavePlayerData();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadPlayerData();
        }
    }

    public void SavePlayerData()
    {
        Save(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }

    public void LoadPlayerData()
    {
        Load(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }

    // 保存数据到PlayerPrefs
    public void Save(Object data, string key)
    {
        // 将数据对象转换为JSON格式的字符串
        string jsonData = JsonUtility.ToJson(data,true);

        // 将JSON字符串保存到PlayerPrefs中，使用给定的键值（key）
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);
        // 立即保存PlayerPrefs的修改
        PlayerPrefs.Save();
    }

    // 从PlayerPrefs加载数据
    public void Load(Object data, string key)
    {
        // 检查PlayerPrefs中是否包含指定的键值（key）
        if (PlayerPrefs.HasKey(key))
        {
            // 如果包含，从JSON字符串中恢复数据对象，覆盖传入的 data 对象
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
    }




}
