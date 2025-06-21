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

        // ��ǲ ���� - �ɼ�â
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
    /// ���� ���� �� �������ͽ� UI �ֽ�ȭ
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
        // UI �ֽ�ȭ
        hpFSlider.value = pManager.status.curhp;
        hpText.text = $"{pManager.status.curhp} / {pManager.status.maxHp}";

        // UI ���� ����Ʈ ������
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
    /// ���� ���� ���� UI On/Off
    /// </summary>
    /// <param name="isOn"></param>
    public void Awakening_Setting(bool isOn)
    {
        awakeningObj.SetActive(isOn);
    }

    /// <summary>
    /// ������ �� ȿ���� ȸ�� �� UI ǥ��
    /// </summary>
    /// <param name="type">0 = ü��, 1 = ���׹̳� 2 = </param>
    /// <param name="value"></param>
    public void PlayerUI_Recovery(Player_Status.RecoveryType type, int value)
    {
        // ȸ�� �ؽ�Ʈ ���
        GameObject obj = Instantiate(recoveryTextObject, HitVFXPos(), Quaternion.identity);
        DamageUI d = obj.GetComponent<DamageUI>();
        d.RecoveryUI_Setting(type, value);
    }

    private Vector3 HitVFXPos()
    {
        Vector3 originPosition = damagePosCollider.transform.position;

        // �ݶ��̴��� ����� �������� bound.size ���
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
    /// ������ UI ǥ�� ���
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
        // UI �ֽ�ȭ & ǥ��
        levelUpSet.SetActive(true);
        levelUpText.text = level.ToString();
        levelUpCanvasGroup.alpha = 1;

        // ������
        yield return new WaitForSeconds(1.25f);

        // ���̵� �ƿ�
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            levelUpCanvasGroup.alpha = Mathf.Lerp(1, 0, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        // UI ����
        levelUpCanvasGroup.alpha = 0;
        levelUpSet.SetActive(false);
    }

    /// <summary>
    /// ���� �ؽ�Ʈ �ֽ�ȭ ����
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
    /// ���� ���̵� �ý���
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
        // �ؽ�Ʈ �ʱ�ȭ
        attackGuideCanvasGroup.alpha = 1;

        // �ؽ�Ʈ Ȱ��ȭ �� ����
        guide_nextAttackText.gameObject.SetActive(true);
        guide_nextAttackText.text = string.Join("\n", nextAttacks);

        // ���̵� ������
        yield return new WaitForSeconds(0.5f);

        // ���̵� �ƿ� (1.25�� ���� ������ �����)
        yield return attackGuideCanvasGroup.DOFade(0, 1.25f).WaitForCompletion();
    }


    /// <summary>
    /// ���� ���� �� ������ ���� / ���� ���� �� ������ ����
    /// </summary>
    /// <param name="isUp"></param>
    public void MiniMap_SizeSetting(bool isUp)
    {
        // ���� Ʈ���� ���� ���̸� �ߴ�
        miniMapRect.DOKill();

        // ��ǥ ũ�� ����
        Vector2 targetScale = isUp ? Vector2.one : new Vector2(0.7f, 0.7f);

        // Ʈ�� ���� (0.4�� ���� �ε巴�� ũ�� ����)
        miniMapRect.DOScale(targetScale, 0.4f).SetEase(Ease.OutQuad);
    }


    /// <summary>
    /// ���� �ʵ忡�� ���̾�α� ȣ�� �� �ش� ���̾�α׷�!
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

        // ���̾�α� ����
        for (int i = 0; i < data.Datas.Count; i++)
        {
            // ��� üũ - ����
            Dialog_Event(data.Datas[i], Dialog_Data_SO.EventPos.Start);

            // ���̾�α� ȣ��
            dialogNameText.text = data.Datas[i].characterName;
            dialogText.text = null;
            foreach (char text in data.Datas[i].dialog)
            {
                dialogText.text += text;
                yield return new WaitForSeconds(0.03f);
            }

            // ��� üũ - ����
            Dialog_Event(data.Datas[i], Dialog_Data_SO.EventPos.End);

            // ���� ���̾�α� ���
            yield return new WaitForSeconds(data.Datas[i].dialog_Daley);
        }

        // ��ȭ ���� �� ���̵� �ƿ�
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
            // ���� ������ �������� �����ϴ� �̺�Ʈ ����Ʈ��� ��ɵ���
            if (type.eventDatas[i].evnetPos == startPos)
            {
                // ��� �� ����ġ
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
    /// ����Ʈ ȣ�� ���
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
    /// ���� �¸� UI
    /// </summary>
    public void FieldClear_Normal()
    {
        StartCoroutine(FieldClaerNormalCall());
    }

    private IEnumerator FieldClaerNormalCall()
    {
        fieldClearSet.SetActive(true);
        isClear = true;

        // ���̵� ��
        fieldClearCanvasGroup.alpha = 0;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.25f;
            fieldClearCanvasGroup.alpha = Mathf.Lerp(0, 1, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        fieldClearCanvasGroup.alpha = 1;

        // ������
        yield return new WaitForSeconds(1.25f);

        // ���̵� �ƿ�
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.25f;
            fieldClearCanvasGroup.alpha = Mathf.Lerp(1, 0, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        fieldClearCanvasGroup.alpha = 0;

        // ����
        fieldClearSet.SetActive(false);
        isClear = false;
    }


    /// <summary>
    /// ��� ȣ�� ���
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
    /// ESC�� �����ϴ� �÷��̾� UI On/Off
    /// </summary>
    public void PlayerUI_Setting()
    {
        // ���� ȭ���̸� ������ �ɼ�â���� ���� - �ƴ϶�� �������� ������ UI�� Ȱ��ȭ
        PlayerUI_Setting(SaveLoad_Manager.instance.isStartScene ? 3 : (int)uiType);

        // ����ȭ���̸� �κ�, ��ų, Ÿ��Ʋ ���� UI ��ư ��Ȱ��ȭ
        PlayerUIButton_Setting(!SaveLoad_Manager.instance.isStartScene);

        // �ɼ� UI Ȱ/��Ȱ��ȭ
        playerUI.SetActive(!playerUI.activeSelf);
    }

    /// <summary>
    /// �÷��̾��� �ɼ� UI ������ On Off
    /// </summary>
    /// <param name="index"></param>
    public void PlayerUI_Setting(int index)
    {
        // ��ü ��Ȱ��ȭ
        for (int i = 0; i < playerUISet.Count; i++)
        {
            playerUISet[i].SetActive(false);
        }

        uiType = (UIType)index;
        playerUISet[index].SetActive(true);
    }

    /// <summary>
    /// ���� ���� ���� �κ��丮, ��ųƮ��, Ÿ��Ʋ UI ��ư Ȱ/��Ȱ��ȭ
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
    /// ��ų ���� ��� ǥ�� UI
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

        skillResultSetText.text = isSuccess ? "���� ����" : "��ų ����Ʈ�� �����մϴ�!";
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
    /// ��ų ���� UI
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
    /// ���� ��ų����Ʈ ǥ�� UI �ֽ�ȭ
    /// </summary>
    /// <param name="point"></param>
    public void Skill_Point(int point)
    {
        skillPointText.text = $"���� ����Ʈ : {point}";
    }
    #endregion


    #region Inventory
    /// <summary>
    /// ������ ���Կ� ���콺 ���� �� ���� UI
    /// </summary>
    /// <param name="item"></param>
    public void Item_DescriptionUI(bool isOn, Item_Base item)
    {
        if (isOn)
        {
            // ��ġ ����
            itemDescriptionTrans.anchoredPosition = GetTooltipPositionWithClamp(canvas, itemDescriptionTrans, Input.mousePosition, offSet);

            // UI �ֽ�ȭ
            itemIconImage.sprite = item.Icon;
            itemNameText.text = item.ItemName;
            itemTierText.text = $"{item.itemRating} ({item.itemType})";
            itemDescriptionText.text = item.ItemDescription;
        }

        itemDescriptionSet.SetActive(isOn);
    }

    /// <summary>
    /// ���� UI ��ġ ������Ʈ
    /// </summary>
    public void Item_DescriptionUIPosUpdata()
    {
        if(itemDescriptionSet.activeSelf)
        {
            itemDescriptionTrans.anchoredPosition = GetTooltipPositionWithClamp(canvas, itemDescriptionTrans, Input.mousePosition, offSet);
        }
    }

    /// <summary>
    /// Inventory UI ǥ�� ��ġ ���� UI
    /// </summary>
    /// <param name="canvasRect"></param>
    /// <param name="tooltipRect"></param>
    /// <param name="mouseScreenPosition"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    private Vector2 GetTooltipPositionWithClamp(RectTransform canvasRect, RectTransform tooltipRect, Vector2 mouseScreenPosition, Vector2 offset)
    {
        // ���콺 �� ����(UI) ��ǥ ��ȯ
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            mouseScreenPosition,
            null,
            out Vector2 localPoint
        );

        Vector2 anchoredPos = localPoint + offset;

        // ������ ���
        Vector2 tooltipSize = tooltipRect.sizeDelta;
        Vector2 canvasSize = canvasRect.rect.size;

        float halfCanvasW = canvasSize.x / 2f;
        float halfCanvasH = canvasSize.y / 2f;

        // X ����
        if (anchoredPos.x + tooltipSize.x > halfCanvasW)
            anchoredPos.x = halfCanvasW - tooltipSize.x;
        if (anchoredPos.x < -halfCanvasW)
            anchoredPos.x = -halfCanvasW;

        // Y ����
        if (anchoredPos.y + tooltipSize.y > halfCanvasH)
            anchoredPos.y = halfCanvasH - tooltipSize.y;
        if (anchoredPos.y < -halfCanvasH)
            anchoredPos.y = -halfCanvasH;

        return anchoredPos;
    }
    #endregion
}
