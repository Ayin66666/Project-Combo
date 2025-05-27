using System.Collections.Generic;
using UnityEngine;
using System.IO;


[System.Serializable]
public class ChapterData
{
    public string chapterName;
    public List<StageData> stage;
}


[System.Serializable]
public class StageData
{
    public bool isClear;
    public Rank clearRank;
    public float cleraTime;
    public enum Rank { None, D, C, B, A, S }
}


public class Chapter_Manager : MonoBehaviour
{
    [Header("---Save Setting---")]
    [SerializeField] private string path;
    [SerializeField] private string[] filename;
    SaveLoad_Manager sd = SaveLoad_Manager.instance;


    [Header("---Data---")]
    [SerializeField] private List<Chapter_Data_SO> capter; // ���� ������
    [SerializeField] private List<ChapterData> capterDatas; // ����� ������


    /// <summary>
    /// �ش� ��ο� �����Ͱ� �ִ��� ��ȯ
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool CheckData(int index)
    {
        // �̹� ����� �����Ͱ� ���� ��� ���
        string data = sd.savePath + sd.fileName[index].filename_PlayerData;
        if (File.Exists(data))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public void SaveData(int index)
    {
        // ������ ����
        string data = JsonUtility.ToJson(GetPlayerStatus());
        File.WriteAllText(sd.savePath + sd.fileName[index].filename_stageData, data);

        // ���̺� ���� UI
        SaveLoad_Manager.instance.SaveSuccessUI();
    }

    public void LoadData(int index)
    {
        string data = sd.savePath + sd.fileName[index].filename_PlayerData;
        if (!File.Exists(data))
        {
            // �ε� ���� ����!
            Debug.LogWarning("�ε��� ������ �����ϴ�: " + data);
            return;
        }

        try
        {
            // �ε� �õ�
            string json = File.ReadAllText(data);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

            // �������ͽ� ����
            Player_Manager.instance.Status_Setting(playerData);
        }
        catch (IOException ex)
        {
            // �ε带 ������ ���
            Debug.LogError("Load failed: " + ex.Message);
        }
    }
}
