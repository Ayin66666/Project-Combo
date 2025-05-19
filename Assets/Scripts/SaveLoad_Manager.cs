using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoad_Manager : MonoBehaviour
{
    [Header("--- Save & Load ---")]
    [SerializeField] private Character_Status_SO status;
    private string path;
    private string fileName = "/save";


    private void Awake()
    {
        // 세이브 & 로드
        path = Application.persistentDataPath + fileName;
        LoadData();
    }

    public void SaveData()
    {
        string data = JsonUtility.ToJson(status);
        File.WriteAllText(path, data);
    }

    public void LoadData()
    {
        if (!File.Exists(path))
        {
            SaveData();
        }

        string data = File.ReadAllText(path);
        status = JsonUtility.FromJson<Character_Status_SO>(data);
    }
}
