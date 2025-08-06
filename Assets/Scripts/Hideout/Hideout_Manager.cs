using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Hideout_Manager : MonoBehaviour
{
    public static Hideout_Manager instance;


    [Header("---Chapter Setting---")]
    [SerializeField] private int curChapter;
    [SerializeField] private string curSelectStage;
    [SerializeField] private Transform startPos;


    [Header("---Select UI---")]
    [SerializeField] private GameObject selectSet;
    [SerializeField] private List<Hideout_StageSlot> slots;

    [Header("---Data---")]
    [SerializeField] private Chapter_Data_SO uiData; // 스테이지 UI 데이터

    #region UI
    [Header("---Description UI---")]
    [SerializeField] private TextMeshProUGUI stageTypeText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI stageNameText;
    [SerializeField] private TextMeshProUGUI clearTimeText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private Image stageImage;


    [Header("---Detailed description UI---")]
    [SerializeField] private GameObject detailedDescriptionSet;
    [SerializeField] private TextMeshProUGUI ddText;


    [Header("---do not enter UI---")]
    [SerializeField] private GameObject enterUI;
    [SerializeField] private CanvasGroup enterCanvasGroup;
    private Coroutine enterCoroutine;


    [Header("---Icon UI---")]
    [SerializeField] private GameObject iconSet;
    [SerializeField] private CanvasGroup iconCanvasGroup;
    private bool isUIOn;
    private bool isPlayerIn;
    private Quaternion originalRot;
    protected Coroutine uiCoroutine;
    #endregion


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

    private void Start()
    {
        // 스테이지 셋팅
        Data_Setting();

        // 페이드 종료
        UI_Manager.instance.Fade(false, 1.5f);
        
        Player_Setting();
    }


    #region UI
    /// <summary>
    /// UI On/Off
    /// </summary>
    public void Hideout_Setting(bool isOn)
    {
        // 플레이어 UI
        UI_Manager.instance.UI_Setting(isOn);

        // 플레이어 동작
        Player_Manager.instance.action.canAction = !isOn;

        // 커서 셋팅
        Player_Manager.instance.Cursor_Setting(!isOn);

        // 스테이지 UI
        selectSet.SetActive(isOn);
    }

    /// <summary>
    /// 스테이지 버튼 클릭 시 상세 UI 표기
    /// </summary>
    public void DescriptionUI_Setting(int stageIndex)
    {
        Debug.Log(stageIndex);

        // 스테이지 기본 데이터
        stageImage.sprite = null;
        stageTypeText.text = uiData.stageData[stageIndex].stageType.ToString(); // 에러발생 - 인덱스 오버?
        levelText.text = uiData.stageData[stageIndex].stageLevel.ToString();
        stageNameText.text = uiData.stageData[stageIndex].stageName;
        descriptionText.text = uiData.stageData[stageIndex].stageSummation;
        ddText.text = uiData.stageData[stageIndex].stageDescription;

        // 클리어 데이터
        clearTimeText.text = ChapterData_Manager.instance.claerData.claerData.chapterList[curChapter].stageList[stageIndex].clearTime.ToString();
        rankText.text = ChapterData_Manager.instance.claerData.claerData.chapterList[curChapter].stageList[stageIndex].clearRank.ToString();

        // 진입 데이터 셋팅
        curSelectStage = uiData.stageData[stageIndex].sceneName;
    }

    /// <summary>
    /// 스테이지 진입 조건 불만족 시 안내 UI
    /// </summary>
    public void EnterUI()
    {
        if (enterCoroutine != null)
            StopCoroutine(enterCoroutine);

        enterCoroutine = StartCoroutine(EnterUICall());
    }

    private IEnumerator EnterUICall()
    {
        // UI 표기
        enterUI.SetActive(true);
        enterCanvasGroup.alpha = 1;

        // 딜레이
        yield return new WaitForSeconds(1.25f);

        // 페이드 아웃
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.25f;
            enterCanvasGroup.alpha = EasingFunctions.OutExpo(timer);
            yield return null;
        }
        enterCanvasGroup.alpha = 0;

        // UI 비활성화
        enterUI.SetActive(false);
    }
    #endregion


    #region Icon
    /// <summary>
    /// 오브젝트 위에 아이콘 UI On/Off
    /// </summary>
    /// <param name="isOn"></param>
    public void Icon_Setting(bool isOn)
    {
        if (uiCoroutine != null)
            StopCoroutine(uiCoroutine);

        uiCoroutine = StartCoroutine(isOn ? IconOn() : IconOff());
    }

    protected IEnumerator IconOn()
    {
        if (isUIOn) yield break;
        isUIOn = true;

        // 아이콘 활성화
        iconCanvasGroup.alpha = 1;
        while (isPlayerIn)
        {
            // 바라보기
            LookAt();

            yield return null;
        }
    }

    protected IEnumerator IconOff()
    {
        if (!isUIOn) yield break;
        isUIOn = false;

        // 아이콘 비활성화
        float start = iconCanvasGroup.alpha;
        float end = 0;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            iconCanvasGroup.alpha = Mathf.Lerp(start, end, timer);
            yield return null;
        }
    }

    protected IEnumerator IconUseOff()
    {
        float timer = 0f;
        while (timer < 1)
        {
            timer += Time.deltaTime * 0.65f;
            iconCanvasGroup.alpha = Mathf.Lerp(1, 0, EasingFunctions.InOutElastic(timer));
            yield return null;
        }

        iconCanvasGroup.alpha = 0f;
    }

    protected void LookAt()
    {

        Vector3 lookDir = iconSet.transform.position - PlayerAction_Manager.instance.cam.transform.position;
        lookDir.y = 0;
        iconSet.transform.rotation = Quaternion.LookRotation(lookDir.normalized);


        Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized); // 카메라 바라보는 방향
        float deltaY = Quaternion.Angle(originalRot, targetRot);            // 두 회전 사이의 전체 각도 차이

        // 회전 방향을 자기 기준으로 얻기 위해 Vector3.Angle 대신 SignedAngle 사용
        Vector3 originalForward = originalRot * Vector3.forward;
        Vector3 targetForward = targetRot * Vector3.forward;
        float signedAngle = Vector3.SignedAngle(originalForward, targetForward, Vector3.up);

        float clampedAngle = Mathf.Clamp(signedAngle, -45f, 45f); // ±30도 제한

        // 제한된 회전 각도만큼 회전한 새로운 방향 계산
        Quaternion limitedRot = Quaternion.AngleAxis(clampedAngle, Vector3.up) * originalRot;

        iconSet.transform.rotation = limitedRot;
    }
    #endregion


    #region 아지트 입장 시 데이터 최신화 로직
    /// <summary>
    /// 아지트 입장 시 데이터 최신화 기능
    /// </summary>
    public void Data_Setting()
    {
        // 데이터 받아오기
        ChapterData data = ChapterData_Manager.instance.Chapter_Setting(curChapter);

        // 데이터 적용

        // 1번 스테이지는 무조건 입장 가능 - 클리어 여부 상관 x
        slots[0].SlotUI_Setting(uiData, 0, true);

        // 이전 스테이지가 클리어되어 있으면 입장 가능
        for (int i = 1; i < slots.Count; i++)
        {
            bool canEnter = data.stageList[i - 1].isClear;
            slots[i].SlotUI_Setting(uiData, i, canEnter);
        }

        // 설명 UI 설정 - 최초 실행 시 무조건 0번 설명이 나오도록?
        DescriptionUI_Setting(0);
    }

    public void Player_Setting()
    {
        // 스테이터스 조절
        Player_Manager.instance.status.curhp = Player_Manager.instance.status.maxHp;
        Player_Manager.instance.status.curStamina = Player_Manager.instance.status.maxStamina;
        Player_Manager.instance.status.curAwakening = 0;

        // 플레이어 활성화
        Player_Manager.instance.Player_Hideout_Setting();
        Player_Manager.instance.PlayerPos_Setting(startPos.position);
        Player_Manager.instance.PlayerOnOff_Setting(true);
    }
    #endregion


    #region Button Event
    /// <summary>
    /// 상세 설명 UI On/Off
    /// </summary>
    /// <param name="isOn"></param>
    public void Click_DetailedDescription(bool isOn)
    {
        detailedDescriptionSet.SetActive(isOn);
    }

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
        // 플레이어 동작 정지
        Player_Manager.instance.Player_Action_Setting(false);

        // 페이드
        UI_Manager.instance.Fade(true, 1.5f);
        while (UI_Manager.instance.isFade)
        {
            yield return null;
        }

        // 플레이어 비활성화
        Player_Manager.instance.PlayerOnOff_Setting(false);

        // 씬 이동
        SceneLoad_Manager.LoadScene(curSelectStage);
    }

    /// <summary>
    /// 게임 데이터 세이브 버튼
    /// </summary>
    public void Click_Save()
    {
        SaveLoad_Manager.instance.SaveLoadUI(true);
    }
    #endregion
}
