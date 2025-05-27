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
    [SerializeField] private List<Chapter_Data_SO> capter; // 고정 데이터
    [SerializeField] private List<ChapterData> capterDatas; // 저장용 데이터


    /// <summary>
    /// 해당 경로에 데이터가 있는지 반환
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool CheckData(int index)
    {
        // 이미 저장된 데이터가 있을 경우 대비
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
        // 데이터 저장
        string data = JsonUtility.ToJson(GetPlayerStatus());
        File.WriteAllText(sd.savePath + sd.fileName[index].filename_stageData, data);

        // 세이브 성공 UI
        SaveLoad_Manager.instance.SaveSuccessUI();
    }

    public void LoadData(int index)
    {
        string data = sd.savePath + sd.fileName[index].filename_PlayerData;
        if (!File.Exists(data))
        {
            // 로드 파일 없음!
            Debug.LogWarning("로드할 파일이 없습니다: " + data);
            return;
        }

        try
        {
            // 로드 시도
            string json = File.ReadAllText(data);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

            // 스테이터스 전달
            Player_Manager.instance.Status_Setting(playerData);
        }
        catch (IOException ex)
        {
            // 로드를 실패할 경우
            Debug.LogError("Load failed: " + ex.Message);
        }
    }
}
