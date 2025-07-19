using System.Collections.Generic;
using UnityEngine;


public class ChapterData_Manager : MonoBehaviour
{
    public static ChapterData_Manager instance;


    [Header("---Stage Data---")]
    public int chapterCount;
    public List<Chapter_Data_SO> chapterUIData;


    [Header("---Claer Data---")]
    public Data claerData;


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
    /// ������ �ε�
    /// </summary>
    public void Data_Setting(Data data)
    {
        claerData = data;
    }

    /// <summary>
    /// �������� Ŭ���� �� ������ �ֽ�ȭ ����
    /// </summary>
    /// <param name="chapter"></param>
    /// <param name="stage"></param>
    /// <param name="rank"></param>
    public void Data_Updata(int chapter, int stage, StageData data)
    {
        claerData.claerData.chapterList[chapter].stageList[stage].isClear = true;
        claerData.claerData.chapterList[chapter].stageList[stage].clearRank = data.clearRank;
        claerData.claerData.chapterList[chapter].stageList[stage].clearTime = data.clearTime;
    }

    /// <summary>
    /// ����Ʈ ���� �� ������ ���� ����
    /// </summary>
    public ChapterData Chapter_Setting(int chapterIndex)
    {
        return claerData.claerData.chapterList[chapterIndex];
    }
}
