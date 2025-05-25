using System.Collections.Generic;
using UnityEngine;
using System.IO; // ��ǲ �ƿ�ǲ

public class PlayerData
{
    public int level;
    public List<int> itemCode;
}


public class SaveLoad_Manager : MonoBehaviour
{
    public static SaveLoad_Manager instance;

    [Header("---Save Setting---")]
    PlayerData playerData = new PlayerData();
    [SerializeField] private string path;
    [SerializeField] private string[] filename;


    [Header("---UI---")]
    [SerializeField] private GameObject saveUIset; // �̰� ��ġ ����غ��� - ���� ���� ���� ���� ���̾ ����ؾ���!


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
        path = Application.persistentDataPath + "/";
    }

    public void SaveData(int index)
    {
        // ������ ���� -> ������ ���� ������!
        string data = JsonUtility.ToJson(playerData);
        File.WriteAllText(path + filename[index], data);

        /*
        // �̹� ����� �����Ͱ� ���� ��� ���
        string data = File.ReadAllText(path + filename[index]);
        if(data != null)
        {
            // ����� �ȳ� UI

            // ���� ������ ����
        }
        else
        {
            // ������ ����
            data = JsonUtility.ToJson(playerData);
            File.WriteAllText(path + filename[index], data);
        }
        */
    }

    public void LoadData(int index)
    {
        string data = File.ReadAllText(path + filename[index]);
        if(!File.Exists(data))
        {
            // �ε� ���� ����!
            return;
        }

        try
        {
            // �ε� �õ�
            string json = File.ReadAllText(data);
            playerData = JsonUtility.FromJson<PlayerData>(json);
        }
        catch(IOException ex)
        {
            // �ε带 ������ ���
            Debug.LogError("Load failed: " + ex.Message);
        }
    }
}
