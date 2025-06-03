using DG.Tweening;
using Easing.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;


    [Header("---State---")]
    public bool isUIOn;
    public bool isFade;
    [SerializeField] private UIType uiType;
    public enum UIType { Item, Skill, Tutorial, Option }


    [SerializeField] private GameObject fightUI;


    [Header("---Player Status---")]
    [SerializeField] private GameObject StatusSet;
    [SerializeField] private Slider hpFSlider;
    [SerializeField] private Slider hpBSlider;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Slider awankingSlider;
    private Coroutine hpCoroutine;


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


    [Header("---Fade UI---")]
    [SerializeField] private GameObject fadeSet;
    [SerializeField] private CanvasGroup fadeCanvasGroup;


    [Header("---Inventory---")]


    [Header("---Short Cut---")]
    [SerializeField] private GameObject obj;



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

    private void Update()
    {
        if (isUIOn)
        {
            Stamina();
            Awanking();
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
            fadeCanvasGroup.alpha = Mathf.Lerp(start, end, t);
           yield return null;
        }
        fadeCanvasGroup.alpha = end;

        if(!isOn)
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

        hpFSlider.maxValue = Player_Manager.instance.status.maxHp;
        hpFSlider.value = Player_Manager.instance.status.curhp;

        hpBSlider.maxValue = Player_Manager.instance.status.maxHp;
        hpBSlider.value = Player_Manager.instance.status.curhp;

        staminaSlider.maxValue = Player_Manager.instance.status.maxStamina;
        staminaSlider.value = Player_Manager.instance.status.curStamina;

        awankingSlider.maxValue = Player_Manager.instance.status.maxAwakening;
        awankingSlider.value = Player_Manager.instance.status.curAwakening;
    }

    public void Hp()
    {
        if (hpCoroutine != null)
            StopCoroutine(hpCoroutine);

        hpCoroutine = StartCoroutine(HpCall());
    }

    private IEnumerator HpCall()
    {
        hpFSlider.value = Player_Manager.instance.status.curhp;
        yield return new WaitForSeconds(0.25f);

        hpBSlider.DOValue(Player_Manager.instance.status.curhp, 0.75f).SetEase(Ease.Linear);
    }

    public void Stamina()
    {
        staminaSlider.value = Player_Manager.instance.status.curStamina;
    }

    public void Awanking()
    {
        awankingSlider.value = Player_Manager.instance.status.curAwakening;
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
        fieldClearCanvasGroup.alpha = 0;

        // ���̵� ��
        float timer = 0;
        float a = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            a = Mathf.Lerp(0, 1, EasingFunctions.OutExpo(timer));
            fieldClearCanvasGroup.alpha = a;
            yield return null;
        }
        fieldClearCanvasGroup.alpha = 1;

        // ������
        yield return new WaitForSeconds(1f);

        // ���̵� �ƿ�
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            a = Mathf.Lerp(1, 0, EasingFunctions.OutExpo(timer));
            fieldClearCanvasGroup.alpha = a;
            yield return null;
        }
        fieldClearCanvasGroup.alpha = 0;

        // ����
        fieldClearSet.SetActive(false);
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


    #region �κ��丮 & ��Ʈ���� ���� ��ũ��Ʈ���� ������ ���� / ǥ��?
    public void Inventory()
    {

    }

    public void ShortCut()
    {

    }
    #endregion
}
