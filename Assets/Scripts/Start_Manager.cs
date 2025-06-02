using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Start_Manager : MonoBehaviour
{
    public static Start_Manager instacne;

    [Header("---State---")]
    [SerializeField] private UI curUI;
    private bool isLoad;
    private enum UI { None, Start, Option, Extra, Exit }


    [Header("---UI---")]
    [SerializeField] private GameObject selectSet;
    [SerializeField] private GameObject optionSet;
    [SerializeField] private GameObject extraSet;
    [SerializeField] private GameObject exitSet;
    [SerializeField] private GameObject loadUI;
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
    }

    private void Start()
    {
        // OnOff 용 리스트 추가
        uiList.Add(selectSet);
        uiList.Add(optionSet);
        uiList.Add(extraSet);
        uiList.Add(exitSet);

        // 슬롯 데이터 셋팅 + UI 최신화
        for (int i = 0; i < slots.Count; i++)
        {
            Data data = SaveLoad_Manager.instance.SlotUI(i);
            if(data != null)
            {
                slots[i].Slot_Setting(data.level.ToString(), data.playTime);
            }
        }
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
        isLoad = false;

        // 신버전
        if (SaveLoad_Manager.instance.CheckData(index))
        {
            // 데이터 로드
            StartCoroutine(Load(index));
        }
        else
        {
            // 데이터 생성
            SaveLoad_Manager.instance.Create_Data(index);

            // 튜토리얼 이동
            SceneLoad_Manager.LoadScene("1.Chapter1_Tutorial");
        }
    }

    private IEnumerator Load(int index)
    {
        // 데이터 로드 UI
        loadUI.SetActive(true);
        while(loadUI.activeSelf)
        {
            yield return null;
        }

        // 데이터를 로드한다면
        if (isLoad)
        {
            // 데이터 로드
            Data data = SaveLoad_Manager.instance.LoadData(index);

            // 스테이터스 데이터 적용
            Player_Status.instacne.Status_Setting(data);

            // 인벤토리 & 장비 데이터 적용

            // 스킬 데이터 적용

            // 클리어 데이터는 해당 씬에 진입했을 때 적용함!

            // 슬롯 셋팅
            SaveLoad_Manager.instance.curSlot = index;

            // 씬 이동
            SceneLoad_Manager.LoadScene("0.Hideout");
        }
    }

    public void Loadbool(bool isLoad)
    {
        this.isLoad = isLoad;
        loadUI.SetActive(false);
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
