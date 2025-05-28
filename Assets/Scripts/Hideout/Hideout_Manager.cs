using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region Stage Data
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
    public float clearTime;
    public enum Rank { None, D, C, B, A, S }
}
#endregion


public class Hideout_Manager : MonoBehaviour
{
    public static Hideout_Manager instance;


    [Header("---Chapter Setting---")]
    [SerializeField] private int chapterCount;


    [Header("---Select UI---")]
    [SerializeField] private GameObject selectSet;


    [Header("---Data---")]
    [SerializeField] private Chapter_Data_SO uiData; // 스테이지 UI 데이터
    [SerializeField] private ChapterData clearData; // 저장용 데이터 - 클리어 여부


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

        DontDestroyOnLoad(gameObject);
    }

    public void Data_Setting(Data data)
    {
        // 데이터 받아와서 셋팅
        clearData = data.clearData[chapterCount];
    }

    /// <summary>
    /// 스테이지 클리어 시 데이터 전달
    /// </summary>
    /// <param name="index"></param>
    /// <param name="data"></param>
    public void Stage_Clear(int index, StageData data)
    {
        clearData.stage[index] = data;
    }

    #region Button Event
    /// <summary>
    /// 스테이지 선택 UI
    /// </summary>
    /// <param name="isOn"></param>
    public void Click_Select(bool isOn)
    {
        // 1. 플레이어의 진행도 데이터를 저장하는 무언가에서 데이터 받아오기

        // 2. 받아온 데이터 기반 UI 활성화
        selectSet.SetActive(isOn);
    }

    /// <summary>
    /// 스테이지 이동 기능
    /// </summary>
    public void Click_Stage(string sceneName)
    {
        StartCoroutine(StageCall(sceneName));
    }

    private IEnumerator StageCall(string sceneName)
    {
        // 선행 연출 - 넣을건지?
        UI_Manager.instance.Fade(true, 0.75f);
        while (UI_Manager.instance.isFade)
        {
            yield return null;
        }

        // 씬 이동
        SceneLoad_Manager.LoadScene(sceneName);
    }

    /// <summary>
    /// 게임 데이터 세이브 버튼
    /// </summary>
    public void Click_Save()
    {
        SaveLoad_Manager.instance.SaveUI(true);
    }
    #endregion
}
