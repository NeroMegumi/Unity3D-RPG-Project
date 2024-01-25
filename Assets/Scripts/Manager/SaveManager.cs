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

    // �������ݵ�PlayerPrefs
    public void Save(Object data, string key)
    {
        // �����ݶ���ת��ΪJSON��ʽ���ַ���
        string jsonData = JsonUtility.ToJson(data,true);

        // ��JSON�ַ������浽PlayerPrefs�У�ʹ�ø����ļ�ֵ��key��
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);
        // ��������PlayerPrefs���޸�
        PlayerPrefs.Save();
    }

    // ��PlayerPrefs��������
    public void Load(Object data, string key)
    {
        // ���PlayerPrefs���Ƿ����ָ���ļ�ֵ��key��
        if (PlayerPrefs.HasKey(key))
        {
            // �����������JSON�ַ����лָ����ݶ��󣬸��Ǵ���� data ����
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
    }




}
