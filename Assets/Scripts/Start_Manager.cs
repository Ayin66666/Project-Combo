using System.Collections.Generic;
using UnityEngine;

public class Start_Manager : MonoBehaviour
{
    public static Start_Manager instacne;

    [Header("---State---")]
    [SerializeField] private UI curUI;
    private enum UI { None, Start, Option, Extra, Exit }


    [Header("---UI---")]
    [SerializeField] private GameObject selectSet;
    [SerializeField] private GameObject optionSet;
    [SerializeField] private GameObject extraSet;
    [SerializeField] private GameObject exitSet;
    [SerializeField] private List<Save_Slot> slots;
    private List<GameObject> uiList = new List<GameObject>();


    private void Awake()
    {
        if (instacne == null)
        {
            instacne = this;
        }
        else
        {
            Destroy(gameObject);
        }

        uiList.Add(selectSet);
        uiList.Add(optionSet);
        uiList.Add(extraSet);
        uiList.Add(exitSet);
    }

    /// <summary>
    /// 슬롯 선택 UI 표기
    /// </summary>
    /// <param name="index"></param>
    /// <param name="levelText"></param>
    /// <param name="timeText"></param>
    public void Slot_Setting(int index, string levelText, string timeText)
    {
        slots[index].Slot_Setting(levelText, timeText);
    }


    #region 버튼 이벤트
    /// <summary>
    /// 나가기 버튼
    /// </summary>
    public void Click_Out()
    {
        curUI = UI.None;
        foreach (GameObject go in uiList)
        {
            go.SetActive(false);
        }
    }

    /// <summary>
    /// 시작 버튼
    /// </summary>
    public void Click_Start()
    {
        curUI = UI.Start;
        selectSet.SetActive(true);
    }

    /// <summary>
    /// select 슬롯에서 클릭했을 때 호출 - 인자값의 데이터가 있는지 체크 후 동작
    /// </summary>
    /// <param name="index"></param>
    public void Click_Slot(int index)
    {
        // 구버전 테스트용
        /*
        if (SaveLoad_Manager.instance.CheckData(index))
        {
            // 해당 슬롯에 저장된 데이터가 있을 경우

            // 데이터 로드
            SaveLoad_Manager.instance.LoadData(index);

            // 마이룸으로
            SceneLoad_Manager.LoadScene("0.Hideout");
        }
        else
        {
            // 저장된 데이터가 없다면 - 신규 데이터 생성

            // 튜토리얼 이동
            SceneLoad_Manager.LoadScene("1.Chapter1_Tutorial");
        }
        */

        // 신버전
        if(SaveLoad_Manager.instance.CheckData(index))
        {
            // 데이터 로드
            Data data = SaveLoad_Manager.instance.LoadData(index);

            // 데이터 적용

            // 씬 이동
            SceneLoad_Manager.LoadScene("0.Hideout");
        }
        else
        {
            // 데이터 생성
            SaveLoad_Manager.instance.Create_Data(index);

            // 튜토리얼 이동
            SceneLoad_Manager.LoadScene("1.Chapter1_Tutorial");
        }
    }

    /// <summary>
    /// 옵션 버튼
    /// </summary>
    public void Click_Option()
    {
        curUI = UI.Start;
        optionSet.SetActive(true);
    }

    /// <summary>
    /// 크래딧 버튼
    /// </summary>
    public void Click_Extra()
    {
        curUI = UI.Start;
        extraSet.SetActive(true);
    }

    /// <summary>
    /// 게임 종료 버튼
    /// </summary>
    public void Click_Exit()
    {
        curUI = UI.Start;
        exitSet.SetActive(true);
    }

    /// <summary>
    /// 게임 종료
    /// </summary>
    public void Click_GameOut()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion
}
