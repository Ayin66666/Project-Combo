using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class ChapterData
{
    public string chapterName;
    public List<StageData> stage;
}

public class StageData
{
    public bool isClear;
    public Rank clearRank;
    public float cleraTime;
    public enum Rank { None, D, C, B, A, S }
}


public class Capter_Manager : MonoBehaviour
{
    [Header("---Save Setting---")]
    [SerializeField] private string path;
    [SerializeField] private string[] filename;


    [Header("---Data---")]
    public List<ChapterData> chapterDatas; // �� �κ� ��ũ���ͺ� ������Ʈ��

    public void SaveData()
    {
        
    }

    public void LoadData()
    {

    }
}
