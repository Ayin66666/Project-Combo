using System.Collections.Generic;
using UnityEngine;


public class ChapterData_Manager : MonoBehaviour
{
    public static ChapterData_Manager instance;


    [Header("---Data---")]
    public int chapterCount;
    public List<ChapterData> chapterData;
    public List<Chapter_Data_SO> chapterUIData;


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

    }


    /// <summary>
    /// 게임 시작 시 데이터 로드
    /// </summary>
    public void Data_Setting(Data data)
    {
        chapterData = data.clearData.chapterList;
        chapterCount = chapterData.Count;
    }

    /// <summary>
    /// 클리어 데이터 업데이트
    /// </summary>
    /// <param name="data"></param>
    public void Data_Updata(int chapterIndex, ChapterData data)
    {
        if (chapterIndex >= 0 && chapterIndex < chapterData.Count)
        {
            chapterData[chapterIndex] = data;
        }
        else
        {
            Debug.LogWarning($"잘못된 챕터 인덱스입니다: {chapterIndex}");
        }
    }
}
