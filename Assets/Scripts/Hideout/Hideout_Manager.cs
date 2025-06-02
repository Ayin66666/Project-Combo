using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Hideout_Manager : MonoBehaviour
{
    public static Hideout_Manager instance;


    [Header("---Chapter Setting---")]
    [SerializeField] private int chapterCount;
    [SerializeField] private string curSelectStage;


    [Header("---Select UI---")]
    [SerializeField] private GameObject selectSet;


    [Header("---Data---")]
    [SerializeField] private Chapter_Data_SO uiData; // 스테이지 UI 데이터
    [SerializeField] private ChapterData stageClearData; // 저장용 데이터 - 클리어 여부


    [Header("---Component---")]
    private StageData_Manager sd_Manager;


    [Header("---Description UI---")]
    [SerializeField] private GameObject descriptionSet;
    [SerializeField] private TextMeshProUGUI stageTypeText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI stageNameText;
    [SerializeField] private TextMeshProUGUI clearTimeText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private Image stageImage;


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

        sd_Manager = StageData_Manager.instance;
    }

    private void Start()
    {
        Data_Setting();
        SelectUI_Setting();

        DescriptionUI_Setting(0);
    }


    /// <summary>
    /// 스테이지 버튼 클릭 시 상세 UI 표기
    /// </summary>
    public void DescriptionUI_Setting(int stageIndex)
    {
        // 스테이지 기본 데이터
        stageImage.sprite = null;
        stageTypeText.text = uiData.stageData[stageIndex].stageType.ToString();
        levelText.text = uiData.stageData[stageIndex].stageLevel.ToString();
        stageNameText.text = uiData.stageData[stageIndex].stageName;
        descriptionText.text = uiData.stageData[stageIndex].stageSummation;

        // 클리어 데이터
        clearTimeText.text = stageClearData.stageList[stageIndex].clearTime.ToString();
        rankText.text = stageClearData.stageList[stageIndex].clearRank.ToString();

        // 진입 데이터 셋팅
        curSelectStage = uiData.stageData[stageIndex].sceneName;
    }


    #region 아지트 입장 시 데이터 최신화 로직
    /// <summary>
    /// 아지트 입장 시 데이터 최신화 기능
    /// </summary>
    public void Data_Setting()
    {
        // 아지트 진입 시 동작 - 데이터 셋팅 로직
        Data saveData = SaveLoad_Manager.instance.LoadData(SaveLoad_Manager.instance.curSlot);
        ChapterData ch = saveData.clearData.chapterList[chapterCount];
        stageClearData = ch;


        // 스테이지 클리어 후 아지트 진입 시 동작 - 데이터 최신화 로직
        if (sd_Manager != null && sd_Manager.haveNewData)
        {
            // 데이터 로드
            (int chapter, int stageIndex, StageData data) = sd_Manager.Get_StageData();


            // 지금 스테이지에 맞는 데이터 & 데이터 인덱스에 문제가 없다면
            if (chapterCount == chapter && stageIndex < ch.stageList.Count)
            {
                // 데이터 셋팅
                saveData.clearData.chapterList[chapterCount].stageList[stageIndex] = data;

                // 세이브 최신화
                SaveLoad_Manager.instance.SaveData(SaveLoad_Manager.instance.curSlot);
            }
            else
            {
                Debug.Log($"데이터 최신화 에러 / 챕터 인덱스 : {chapter} / 스테이지 인덱스 : {stageIndex}");
            }
        }
    }

    /// <summary>
    /// 스테이지 선택 UI 최신화
    /// </summary>
    private void SelectUI_Setting()
    {

    }
    #endregion


    #region Button Event
    /// <summary>
    /// 스테이지 선택 UI
    /// </summary>
    /// <param name="isOn"></param>
    public void Click_Select(bool isOn)
    {
        selectSet.SetActive(isOn);
    }

    /// <summary>
    /// 스테이지 이동 기능
    /// </summary>
    public void Click_Stage()
    {
        StartCoroutine(StageCall());
    }

    private IEnumerator StageCall()
    {
        // 선행 연출 - 넣을건지?
        UI_Manager.instance.Fade(true, 0.75f);
        while (UI_Manager.instance.isFade)
        {
            yield return null;
        }

        // 씬 이동
        SceneLoad_Manager.LoadScene(curSelectStage);
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
