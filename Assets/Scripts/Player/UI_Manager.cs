using DG.Tweening;
using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;


    [Header("---State---")]
    public bool isUIOn;
    public bool isFade;
    public bool isClear;
    [SerializeField] private UIType uiType;
    public enum UIType { Item, Skill, Tutorial, Option, Exit }
    private Player_Manager pManager;


    [Header("---UI Set---")]
    [SerializeField] private GameObject fightUI;
    [SerializeField] private GameObject playerUI;
    [SerializeField] private RectTransform canvas;


    [Header("---Player UI---")]
    [SerializeField] private List<GameObject> playerUISet;
    [SerializeField] private List<GameObject> buttonSet;


    [Header("---Fade UI---")]
    [SerializeField] private GameObject fadeSet;
    [SerializeField] private CanvasGroup fadeCanvasGroup;


    #region Player Fight UI
    [Header("---Player Status---")]
    [SerializeField] private GameObject StatusSet;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider hpFSlider;
    [SerializeField] private Slider hpBSlider;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Slider awakeningSlider;
    [SerializeField] private GameObject awakeningObj;
    private Coroutine hpCoroutine;


    [Header("---Recovery---")]
    [SerializeField] private GameObject recoveryTextObject;
    [SerializeField] private Collider damagePosCollider;


    [Header("---Attack Guide---")]
    [SerializeField] private GameObject attackGuideSet;
    [SerializeField] private CanvasGroup attackGuideCanvasGroup;
    [SerializeField] private TextMeshProUGUI guide_CurAttackText;
    [SerializeField] private TextMeshProUGUI guide_nextAttackText;
    private Coroutine attackGuideCoroutine;


    [Header("---Mini Map---")]
    [SerializeField] private GameObject NiniMapSet;
    [SerializeField] private RectTransform miniMapRect;


    [Header("---Quest---")]
    [SerializeField] private GameObject questSet;
    [SerializeField] private CanvasGroup questCanvasGroup;
    [SerializeField] private TextMeshProUGUI questTitleText;
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    private Coroutine questCoroutine;


    [Header("---Dialog---")]
    [SerializeField] private GameObject dialogSet;
    [SerializeField] private CanvasGroup dialogCanvasGroup;
    [SerializeField] private TextMeshProUGUI dialogNameText;
    [SerializeField] private TextMeshProUGUI dialogText;
    private Coroutine dialogCoroutine;


    [Header("---Clear UI---")]
    [SerializeField] private GameObject fieldClearSet;
    [SerializeField] private CanvasGroup fieldClearCanvasGroup;


    [Header("---Die UI---")]
    [SerializeField] private GameObject dieUIset;
    [SerializeField] private CanvasGroup dieCanvasGroup;
    public bool isDieUI;
    #endregion


    #region Option UI
    [Header("---Level Up UI---")]
    [SerializeField] private GameObject levelUpSet;
    [SerializeField] private TextMeshProUGUI levelUpText;
    [SerializeField] private CanvasGroup levelUpCanvasGroup;
    private Coroutine levelUpCoroutine;


    [Header("---Skill Result---")]
    [SerializeField] private GameObject skillResultSet;
    [SerializeField] private TextMeshProUGUI skillResultSetText;
    [SerializeField] private CanvasGroup skillResultSetCanvasGroup;
    private Coroutine skillResultSetCoroutine;


    [Header("---Skill Description---")]
    [SerializeField] private TextMeshProUGUI skillDescriptionText;
    [SerializeField] private VideoPlayer skillVideoPlayer;
    [SerializeField] private TextMeshProUGUI skillPointText;


    [Header("---Inventory---")]
    [SerializeField] private Vector2 offSet;
    [SerializeField] private GameObject itemDescriptionSet;
    [SerializeField] private RectTransform itemDescriptionTrans;
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTierText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;


    [Header("---Short Cut---")]
    [SerializeField] private GameObject obj;
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
        pManager = Player_Manager.instance;

        // 인풋 셋팅 - 옵션창
        Input_Manager.instance.Action_Setting(8, PlayerUI_Setting);
    }

    private void Update()
    {
        if (isUIOn)
        {
            Stamina();
            Awakening();
        }
    }


    public void Fade(bool isOn, float speed)
    {
        StartCoroutine(FadeCall(isOn, speed));
    }

    private IEnumerator FadeCall(bool isOn, float speed)
    {
        float start = isOn ? 0 : 1;
        float end = isOn ? 1 : 0;
        float timer = 0;
        isFade = true;

        fadeSet.SetActive(true);
        fadeCanvasGroup.alpha = start;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / speed);
            fadeCanvasGroup.alpha = Mathf.Lerp(start, end, EasingFunctions.OutExpo(t));
            yield return null;
        }
        fadeCanvasGroup.alpha = end;

        if (!isOn)
        {
            fadeSet.SetActive(false);
        }

        isFade = false;
    }


    #region Status
    /// <summary>
    /// 게임 시작 시 스테이터스 UI 최신화
    /// </summary>
    public void Reset_StatusUI()
    {
        uiType = UIType.Item;

        hpFSlider.maxValue = pManager.status.maxHp;
        hpFSlider.value = pManager.status.curhp;

        hpBSlider.maxValue = pManager.status.maxHp;
        hpBSlider.value = pManager.status.curhp;

        staminaSlider.maxValue = pManager.status.maxStamina;
        staminaSlider.value = pManager.status.curStamina;

        awakeningSlider.maxValue = pManager.status.maxAwakening;
        awakeningSlider.value = pManager.status.curAwakening;
    }

    public void Hp()
    {
        if (hpCoroutine != null)
            StopCoroutine(hpCoroutine);

        hpCoroutine = StartCoroutine(HpCall());
    }

    private IEnumerator HpCall()
    {
        // UI 최신화
        hpFSlider.value = pManager.status.curhp;
        hpText.text = $"{pManager.status.curhp} / {pManager.status.maxHp}";

        // UI 감소 이펙트 딜레이
        yield return new WaitForSeconds(0.25f);
        hpBSlider.DOValue(pManager.status.curhp, 0.75f).SetEase(Ease.Linear);
    }

    public void Stamina()
    {
        staminaSlider.value = pManager.status.curStamina;
    }

    public void Awakening()
    {
        awakeningSlider.value = pManager.status.curAwakening;
    }

    /// <summary>
    /// 각성 가능 여부 UI On/Off
    /// </summary>
    /// <param name="isOn"></param>
    public void Awakening_Setting(bool isOn)
    {
        awakeningObj.SetActive(isOn);
    }

    /// <summary>
    /// 아이템 및 효과로 회복 시 UI 표기
    /// </summary>
    /// <param name="type">0 = 체력, 1 = 스테미너 2 = </param>
    /// <param name="value"></param>
    public void PlayerUI_Recovery(Player_Status.RecoveryType type, int value)
    {
        // 회복 텍스트 출력
        GameObject obj = Instantiate(recoveryTextObject, HitVFXPos(), Quaternion.identity);
        DamageUI d = obj.GetComponent<DamageUI>();
        d.RecoveryUI_Setting(type, value);
    }

    private Vector3 HitVFXPos()
    {
        Vector3 originPosition = damagePosCollider.transform.position;

        // 콜라이더의 사이즈를 가져오는 bound.size 사용
        float range_X = damagePosCollider.bounds.size.x;
        float range_Y = damagePosCollider.bounds.size.y;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Y = Random.Range((range_Y / 2) * -1, range_Y / 2);
        Vector3 RandomPostion = new Vector3(range_X, range_Y);

        Vector3 respawnPosition = originPosition + RandomPostion;
        return respawnPosition;
    }
    #endregion


    #region Level
    /// <summary>
    /// 레벨업 UI 표기 기능
    /// </summary>
    /// <param name="level"></param>
    public void LevelUpUI(int level)
    {
        if (levelUpCoroutine != null)
            StopCoroutine(levelUpCoroutine);

        levelUpCoroutine = StartCoroutine(LevelUpCall(level));
    }

    private IEnumerator LevelUpCall(int level)
    {
        // UI 최신화 & 표기
        levelUpSet.SetActive(true);
        levelUpText.text = level.ToString();
        levelUpCanvasGroup.alpha = 1;

        // 딜레이
        yield return new WaitForSeconds(1.25f);

        // 페이드 아웃
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            levelUpCanvasGroup.alpha = Mathf.Lerp(1, 0, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        // UI 종료
        levelUpCanvasGroup.alpha = 0;
        levelUpSet.SetActive(false);
    }

    /// <summary>
    /// 레벨 텍스트 최신화 로직
    /// </summary>
    /// <param name="level"></param>
    public void Level(int level)
    {
        levelText.text = $"Lv. {level}";
    }
    #endregion


    #region InGame Systeam
    /// <summary>
    /// UI OnOff
    /// </summary>
    /// <param name="isOn"></param>
    public void UI_Setting(bool isOn)
    {
        fightUI.SetActive(isOn);
    }

    /// <summary>
    /// 공격 가이드 시스템
    /// </summary>
    /// <param name="nextAttacks"></param>
    public void AttackGuide(string[] nextAttacks)
    {
        if (attackGuideCoroutine != null)
            StopCoroutine(attackGuideCoroutine);
        attackGuideCanvasGroup.DOKill();

        attackGuideCoroutine = StartCoroutine(AttackGuideCall(nextAttacks));
    }

    private IEnumerator AttackGuideCall(string[] nextAttacks)
    {
        // 텍스트 초기화
        attackGuideCanvasGroup.alpha = 1;

        // 텍스트 활성화 및 설정
        guide_nextAttackText.gameObject.SetActive(true);
        guide_nextAttackText.text = string.Join("\n", nextAttacks);

        // 페이드 딜레이
        yield return new WaitForSeconds(0.5f);

        // 페이드 아웃 (1.25초 동안 서서히 사라짐)
        yield return attackGuideCanvasGroup.DOFade(0, 1.25f).WaitForCompletion();
    }


    /// <summary>
    /// 전투 진입 시 사이즈 감소 / 전투 종료 시 사이즈 증가
    /// </summary>
    /// <param name="isUp"></param>
    public void MiniMap_SizeSetting(bool isUp)
    {
        // 기존 트윈이 실행 중이면 중단
        miniMapRect.DOKill();

        // 목표 크기 설정
        Vector2 targetScale = isUp ? Vector2.one : new Vector2(0.7f, 0.7f);

        // 트윈 실행 (0.4초 동안 부드럽게 크기 조정)
        miniMapRect.DOScale(targetScale, 0.4f).SetEase(Ease.OutQuad);
    }


    /// <summary>
    /// 전투 필드에서 다이얼로그 호출 시 해당 다이얼로그로!
    /// </summary>
    /// <param name="data"></param>
    public void Dialog_Fight(Dialog_Data_SO data)
    {
        if (dialogCoroutine != null)
            StopCoroutine(dialogCoroutine);

        dialogCoroutine = StartCoroutine(DialogFightCall(data));
    }

    private IEnumerator DialogFightCall(Dialog_Data_SO data)
    {
        dialogCanvasGroup.alpha = 1;
        dialogSet.SetActive(true);

        // 다이얼로그 동작
        for (int i = 0; i < data.Datas.Count; i++)
        {
            // 기능 체크 - 시작
            Dialog_Event(data.Datas[i], Dialog_Data_SO.EventPos.Start);

            // 다이얼로그 호출
            dialogNameText.text = data.Datas[i].characterName;
            dialogText.text = null;
            foreach (char text in data.Datas[i].dialog)
            {
                dialogText.text += text;
                yield return new WaitForSeconds(0.03f);
            }

            // 기능 체크 - 종료
            Dialog_Event(data.Datas[i], Dialog_Data_SO.EventPos.End);

            // 다음 다이얼로그 대기
            yield return new WaitForSeconds(data.Datas[i].dialog_Daley);
        }

        // 대화 종료 후 페이드 아웃
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.25f;
            dialogCanvasGroup.alpha = Mathf.Lerp(1, 0, timer);
            yield return null;
        }
        dialogCanvasGroup.alpha = 0;
        dialogSet.SetActive(false);
    }

    private void Dialog_Event(Dialog_Data_SO.Data type, Dialog_Data_SO.EventPos startPos)
    {
        for (int i = 0; i < type.eventDatas.Count; i++)
        {
            // 내가 지정한 순번에서 시작하는 이벤트 리스트라면 기능동작
            if (type.eventDatas[i].evnetPos == startPos)
            {
                // 기능 별 스위치
                switch (type.eventDatas[i].evnetType)
                {
                    case Dialog_Data_SO.EventType.Door:
                        Stage_Manager.instance.Door(type.eventDatas[i].typeOnOff, type.eventDatas[i].typeIndex);
                        break;

                    case Dialog_Data_SO.EventType.WayPoint:
                        Stage_Manager.instance.WayPoint(type.eventDatas[i].typeOnOff, type.eventDatas[i].typeIndex);
                        break;

                    case Dialog_Data_SO.EventType.Quest:
                        Stage_Manager.instance.Quest(type.eventDatas[i].typeIndex);
                        break;

                    case Dialog_Data_SO.EventType.Tooltip:
                        Tutorial_Manager.instance.Tutorial_Tooltip(type.eventDatas[i].typeOnOff, type.eventDatas[i].typeIndex);
                        break;

                    case Dialog_Data_SO.EventType.Tutorial:
                        Tutorial_Manager.instance.Tutorial_Big(type.eventDatas[i].typeIndex);
                        break;

                    case Dialog_Data_SO.EventType.Spawn:
                        Tutorial_Manager.instance.Tutorial_Spawn(type.eventDatas[i].typeIndex);
                        break;

                    case Dialog_Data_SO.EventType.StageEnd:
                        Stage_Manager.instance.Stage_End();
                        break;
                }
            }
        }
    }


    /// <summary>
    /// 퀘스트 호출 기능
    /// </summary>
    /// <param name="data"></param>
    public void Quset(Quest_Data_SO.Data data)
    {
        if (questCoroutine != null)
            StopCoroutine(questCoroutine);

        questCoroutine = StartCoroutine(QusetCall(data));
    }

    private IEnumerator QusetCall(Quest_Data_SO.Data data)
    {
        questCanvasGroup.alpha = 0;
        questTitleText.text = $"{data.questTitle}";
        questDescriptionText.text = $"{data.questDescription}";

        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.25f;
            questCanvasGroup.alpha = timer;
            yield return null;
        }
    }


    /// <summary>
    /// 전투 승리 UI
    /// </summary>
    public void FieldClear_Normal()
    {
        StartCoroutine(FieldClaerNormalCall());
    }

    private IEnumerator FieldClaerNormalCall()
    {
        fieldClearSet.SetActive(true);
        isClear = true;

        // 페이드 인
        fieldClearCanvasGroup.alpha = 0;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.25f;
            fieldClearCanvasGroup.alpha = Mathf.Lerp(0, 1, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        fieldClearCanvasGroup.alpha = 1;

        // 딜레이
        yield return new WaitForSeconds(1.25f);

        // 페이드 아웃
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.25f;
            fieldClearCanvasGroup.alpha = Mathf.Lerp(1, 0, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        fieldClearCanvasGroup.alpha = 0;

        // 종료
        fieldClearSet.SetActive(false);
        isClear = false;
    }


    /// <summary>
    /// 사망 호출 기능
    /// </summary>
    /// <param name="isOn"></param>
    public void DieUI(bool isOn)
    {
        StartCoroutine(isOn ? DieUIOnCall() : DieUIOffCall());
    }

    private IEnumerator DieUIOnCall()
    {
        isDieUI = false;
        dieUIset.SetActive(true);

        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.25f;
            dieCanvasGroup.alpha = Mathf.Lerp(0, 1, timer);
            yield return null;
        }
        dieCanvasGroup.alpha = 1;
        isDieUI = true;
    }

    public IEnumerator DieUIOffCall()
    {
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.25f;
            dieCanvasGroup.alpha = Mathf.Lerp(1, 0, timer);
            yield return null;
        }
        dieCanvasGroup.alpha = 0;
        dieUIset.SetActive(false);
    }
    #endregion


    #region Player UI Button Event
    /// <summary>
    /// ESC로 접근하는 플레이어 UI On/Off
    /// </summary>
    public void PlayerUI_Setting()
    {
        // 시작 화면이면 무조건 옵션창으로 고정 - 아니라면 마지막에 열었던 UI로 활성화
        PlayerUI_Setting(SaveLoad_Manager.instance.isStartScene ? 3 : (int)uiType);

        // 시작화면이면 인벤, 스킬, 타이틀 복귀 UI 버튼 비활성화
        PlayerUIButton_Setting(!SaveLoad_Manager.instance.isStartScene);

        // 옵션 UI 활/비활성화
        playerUI.SetActive(!playerUI.activeSelf);
    }

    /// <summary>
    /// 플레이어의 옵션 UI 종류별 On Off
    /// </summary>
    /// <param name="index"></param>
    public void PlayerUI_Setting(int index)
    {
        // 전체 비활성화
        for (int i = 0; i < playerUISet.Count; i++)
        {
            playerUISet[i].SetActive(false);
        }

        uiType = (UIType)index;
        playerUISet[index].SetActive(true);
    }

    /// <summary>
    /// 현제 씬에 따른 인벤토리, 스킬트리, 타이틀 UI 버튼 활/비활성화
    /// </summary>
    /// <param name="isOn"></param>
    private void PlayerUIButton_Setting(bool isOn)
    {
        foreach(GameObject button in buttonSet)
        {
            button.SetActive(isOn);
        }
    }
    #endregion


    #region Skill
    /// <summary>
    /// 스킬 습득 결과 표기 UI
    /// </summary>
    /// <param name="isSuccess"></param>
    public void Skill_Result(bool isSuccess)
    {
        if (skillResultSetCoroutine != null)
            StopCoroutine(skillResultSetCoroutine);

        skillResultSetCoroutine = StartCoroutine(SkillResultCall(isSuccess));
    }

    private IEnumerator SkillResultCall(bool isSuccess)
    {
        skillResultSet.SetActive(true);

        skillResultSetText.text = isSuccess ? "습득 성공" : "스킬 포인트가 부족합니다!";
        skillResultSetCanvasGroup.alpha = 1;

        yield return new WaitForSeconds(1.25f);

        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            skillResultSetCanvasGroup.alpha = Mathf.Lerp(1, 0, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        skillResultSetCanvasGroup.alpha = 0;

        skillResultSet.SetActive(false);
    }

    /// <summary>
    /// 스킬 설명 UI
    /// </summary>
    /// <param name="data"></param>
    public void Skill_Description(Skill_UI_SO data)
    {
        if (data != null)
        {
            skillDescriptionText.text = data.SkillDescription;
            skillVideoPlayer.clip = data.SkillClip;
            skillVideoPlayer.Play();
        }
        else
        {
            skillDescriptionText.text = "";
            skillVideoPlayer.Pause();
            skillVideoPlayer.clip = null;
        }
    }

    /// <summary>
    /// 남은 스킬포인트 표기 UI 최신화
    /// </summary>
    /// <param name="point"></param>
    public void Skill_Point(int point)
    {
        skillPointText.text = $"보유 포인트 : {point}";
    }
    #endregion


    #region Inventory
    /// <summary>
    /// 아이템 슬롯에 마우스 오버 시 설명 UI
    /// </summary>
    /// <param name="item"></param>
    public void Item_DescriptionUI(bool isOn, Item_Base item)
    {
        if (isOn)
        {
            // 위치 조절
            itemDescriptionTrans.anchoredPosition = GetTooltipPositionWithClamp(canvas, itemDescriptionTrans, Input.mousePosition, offSet);

            // UI 최신화
            itemIconImage.sprite = item.Icon;
            itemNameText.text = item.ItemName;
            itemTierText.text = $"{item.itemRating} ({item.itemType})";
            itemDescriptionText.text = item.ItemDescription;
        }

        itemDescriptionSet.SetActive(isOn);
    }

    /// <summary>
    /// 설명 UI 위치 업데이트
    /// </summary>
    public void Item_DescriptionUIPosUpdata()
    {
        if(itemDescriptionSet.activeSelf)
        {
            itemDescriptionTrans.anchoredPosition = GetTooltipPositionWithClamp(canvas, itemDescriptionTrans, Input.mousePosition, offSet);
        }
    }

    /// <summary>
    /// Inventory UI 표기 위치 설정 UI
    /// </summary>
    /// <param name="canvasRect"></param>
    /// <param name="tooltipRect"></param>
    /// <param name="mouseScreenPosition"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    private Vector2 GetTooltipPositionWithClamp(RectTransform canvasRect, RectTransform tooltipRect, Vector2 mouseScreenPosition, Vector2 offset)
    {
        // 마우스 → 로컬(UI) 좌표 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            mouseScreenPosition,
            null,
            out Vector2 localPoint
        );

        Vector2 anchoredPos = localPoint + offset;

        // 보정값 계산
        Vector2 tooltipSize = tooltipRect.sizeDelta;
        Vector2 canvasSize = canvasRect.rect.size;

        float halfCanvasW = canvasSize.x / 2f;
        float halfCanvasH = canvasSize.y / 2f;

        // X 보정
        if (anchoredPos.x + tooltipSize.x > halfCanvasW)
            anchoredPos.x = halfCanvasW - tooltipSize.x;
        if (anchoredPos.x < -halfCanvasW)
            anchoredPos.x = -halfCanvasW;

        // Y 보정
        if (anchoredPos.y + tooltipSize.y > halfCanvasH)
            anchoredPos.y = halfCanvasH - tooltipSize.y;
        if (anchoredPos.y < -halfCanvasH)
            anchoredPos.y = -halfCanvasH;

        return anchoredPos;
    }
    #endregion
}
