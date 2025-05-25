using System.Collections.Generic;
using UnityEngine;
using System.IO; // 인풋 아웃풋

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
    [SerializeField] private GameObject saveUIset; // 이거 위치 고민해볼것 - 만약 여기 들어가면 소팅 레이어도 고민해야함!


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
        // 데이터 저장 -> 구역별 저장 동작함!
        string data = JsonUtility.ToJson(playerData);
        File.WriteAllText(path + filename[index], data);

        /*
        // 이미 저장된 데이터가 있을 경우 대비
        string data = File.ReadAllText(path + filename[index]);
        if(data != null)
        {
            // 덮어쓰기 안내 UI

            // 조건 만족시 저장
        }
        else
        {
            // 데이터 저장
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
            // 로드 파일 없음!
            return;
        }

        try
        {
            // 로드 시도
            string json = File.ReadAllText(data);
            playerData = JsonUtility.FromJson<PlayerData>(json);
        }
        catch(IOException ex)
        {
            // 로드를 실패할 경우
            Debug.LogError("Load failed: " + ex.Message);
        }
    }
}
