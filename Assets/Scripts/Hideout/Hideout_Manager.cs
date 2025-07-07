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
    [SerializeField] private Chapter_Data_SO uiData; // �������� UI ������
    [SerializeField] private ChapterData stageClearData; // ����� ������ - Ŭ���� ����


    [Header("---Component---")]
    private ClearData_Manager sd_Manager;


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

        sd_Manager = ClearData_Manager.instance;
    }

    private void Start()
    {
        // �������� ����
        Data_Setting();
        DescriptionUI_Setting(0);
        StageSlot_Setting();

        // ���̵� ����
        UI_Manager.instance.Fade(false, 1.5f);

        // �÷��̾� Ȱ��ȭ
        Player_Manager.instance.Player_Hideout_Setting();
        Player_Manager.instance.PlayerPos_Setting(startPos.position);
        Player_Manager.instance.PlayerOnOff_Setting(true);
    }


    #region UI
    /// <summary>
    /// UI On/Off
    /// </summary>
    public void Hideout_Setting(bool isOn)
    {
        // �÷��̾� UI
        UI_Manager.instance.UI_Setting(isOn);

        // �÷��̾� ����
        Player_Manager.instance.action.canAction = !isOn;

        // Ŀ�� ����
        Player_Manager.instance.Cursor_Setting(!isOn);

        // �������� UI
        selectSet.SetActive(isOn);
    }

    /// <summary>
    /// �������� ��ư Ŭ�� �� �� UI ǥ��
    /// </summary>
    public void DescriptionUI_Setting(int stageIndex)
    {
        // �������� �⺻ ������
        stageImage.sprite = null;
        stageTypeText.text = uiData.stageData[stageIndex].stageType.ToString(); // �����߻� - �ε��� ����?
        levelText.text = uiData.stageData[stageIndex].stageLevel.ToString();
        stageNameText.text = uiData.stageData[stageIndex].stageName;
        descriptionText.text = uiData.stageData[stageIndex].stageSummation;
        ddText.text = uiData.stageData[stageIndex].stageDescription;

        // Ŭ���� ������
        clearTimeText.text = stageClearData.stageList[stageIndex].clearTime.ToString();
        rankText.text = stageClearData.stageList[stageIndex].clearRank.ToString();

        // ���� ������ ����
        curSelectStage = uiData.stageData[stageIndex].sceneName;
    }

    /// <summary>
    /// �������� ���� ���� �Ҹ��� �� �ȳ� UI
    /// </summary>
    public void EnterUI()
    {
        if (enterCoroutine != null)
            StopCoroutine(enterCoroutine);

        enterCoroutine = StartCoroutine(EnterUICall());
    }

    private IEnumerator EnterUICall()
    {
        // UI ǥ��
        enterUI.SetActive(true);
        enterCanvasGroup.alpha = 1;

        // ������
        yield return new WaitForSeconds(1.25f);

        // ���̵� �ƿ�
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.25f;
            enterCanvasGroup.alpha = EasingFunctions.OutExpo(timer);
            yield return null;
        }
        enterCanvasGroup.alpha = 0;

        // UI ��Ȱ��ȭ
        enterUI.SetActive(false);
    }
    #endregion


    #region Icon
    /// <summary>
    /// ������Ʈ ���� ������ UI On/Off
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

        // ������ Ȱ��ȭ
        iconCanvasGroup.alpha = 1;
        while (isPlayerIn)
        {
            // �ٶ󺸱�
            LookAt();

            yield return null;
        }
    }

    protected IEnumerator IconOff()
    {
        if (!isUIOn) yield break;
        isUIOn = false;

        // ������ ��Ȱ��ȭ
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


        Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized); // ī�޶� �ٶ󺸴� ����
        float deltaY = Quaternion.Angle(originalRot, targetRot);            // �� ȸ�� ������ ��ü ���� ����

        // ȸ�� ������ �ڱ� �������� ��� ���� Vector3.Angle ��� SignedAngle ���
        Vector3 originalForward = originalRot * Vector3.forward;
        Vector3 targetForward = targetRot * Vector3.forward;
        float signedAngle = Vector3.SignedAngle(originalForward, targetForward, Vector3.up);

        float clampedAngle = Mathf.Clamp(signedAngle, -45f, 45f); // ��30�� ����

        // ���ѵ� ȸ�� ������ŭ ȸ���� ���ο� ���� ���
        Quaternion limitedRot = Quaternion.AngleAxis(clampedAngle, Vector3.up) * originalRot;

        iconSet.transform.rotation = limitedRot;
    }
    #endregion


    #region ����Ʈ ���� �� ������ �ֽ�ȭ ����
    /// <summary>
    /// ����Ʈ ���� �� ������ �ֽ�ȭ ���
    /// </summary>
    public void Data_Setting()
    {
        // ����Ʈ ���� �� ���� - ������ ���� ����
        Data saveData = SaveLoad_Manager.instance.LoadData(SaveLoad_Manager.instance.curSlot);
        ChapterData ch = saveData.clearData.chapterList[curChapter];
        stageClearData = ch;


        // �������� Ŭ���� �� ����Ʈ ���� �� ���� - ������ �ֽ�ȭ ����
        if (sd_Manager != null && sd_Manager.haveNewData)
        {
            // ������ �ε�
            (int chapter, int stageIndex, StageData data) = sd_Manager.Get_StageData();


            // ���� ���������� �´� ������ & ������ �ε����� ������ ���ٸ�
            if (curChapter == chapter && stageIndex < ch.stageList.Count)
            {
                // ������ ����
                saveData.clearData.chapterList[curChapter].stageList[stageIndex] = data;

                // ���̺� �ֽ�ȭ
                SaveLoad_Manager.instance.Save(SaveLoad_Manager.instance.curSlot);
            }
            else
            {
                Debug.Log($"������ �ֽ�ȭ ���� / é�� �ε��� : {chapter} / �������� �ε��� : {stageIndex}");
            }
        }
    }

    /// <summary>
    /// ����Ʈ ���� �� ���� ���� UI �ֽ�ȭ ���
    /// </summary>
    public void StageSlot_Setting()
    {
        // ���� ���� UI ����
        Data data = SaveLoad_Manager.instance.LoadData(SaveLoad_Manager.instance.curSlot);
        ChapterData cData = data.clearData.chapterList[curChapter];
        slots[0].SlotUI_Setting(uiData, 0, true);
        for (int i = 1; i < slots.Count; i++)
        {
            // ���� ���������� Ŭ����Ǿ� ������ ���� ����
            bool canEnter = cData.stageList[i - 1].isClear;
            slots[i].SlotUI_Setting(uiData, i, canEnter);
        }
    }
    #endregion


    #region Button Event
    /// <summary>
    /// �� ���� UI On/Off
    /// </summary>
    /// <param name="isOn"></param>
    public void Click_DetailedDescription(bool isOn)
    {
        detailedDescriptionSet.SetActive(isOn);
    }

    /// <summary>
    /// �������� ���� UI
    /// </summary>
    /// <param name="isOn"></param>
    public void Click_Select(bool isOn)
    {
        selectSet.SetActive(isOn);
    }

    /// <summary>
    /// �������� �̵� ���
    /// </summary>
    public void Click_Stage()
    {
        StartCoroutine(StageCall());
    }

    private IEnumerator StageCall()
    {
        // �÷��̾� ���� ����
        Player_Manager.instance.Player_Action_Setting(false);

        // ���̵�
        UI_Manager.instance.Fade(true, 1.5f);
        while (UI_Manager.instance.isFade)
        {
            yield return null;
        }

        // �÷��̾� ��Ȱ��ȭ
        Player_Manager.instance.PlayerOnOff_Setting(false);

        // �� �̵�
        SceneLoad_Manager.LoadScene(curSelectStage);
    }

    /// <summary>
    /// ���� ������ ���̺� ��ư
    /// </summary>
    public void Click_Save()
    {
        SaveLoad_Manager.instance.SaveLoadUI(true);
    }
    #endregion
}
