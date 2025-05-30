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
    [SerializeField] private Chapter_Data_SO uiData; // �������� UI ������
    [SerializeField] private ChapterData clearData; // ����� ������ - Ŭ���� ����


    [Header("---Component---")]
    private StageData_Manager sd_Manager;


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
    }


    #region ����Ʈ ���� �� ������ �ֽ�ȭ ����
    /// <summary>
    /// ����Ʈ ���� �� ������ �ֽ�ȭ ���
    /// </summary>
    public void Data_Setting()
    {
        // ����Ʈ ���� �� ���� - ������ ���� ����
        Data chapterData = SaveLoad_Manager.instance.LoadData(SaveLoad_Manager.instance.curSlot);
        clearData = chapterData.clearData[SaveLoad_Manager.instance.curSlot];


        // �������� Ŭ���� �� ����Ʈ ���� �� ���� - ������ �ֽ�ȭ ����
        if (sd_Manager != null && sd_Manager.haveNewData)
        {
            // ������ �ε�
            (int chapter, int stageIndex, StageData data) = sd_Manager.Get_StageData();

            // ���� ���������� �´� ������ & ������ �ε����� ������ ���ٸ�
            if (chapterCount == chapter && stageIndex < clearData.stage.Count)
            {
                // ������ ����
                clearData.stage[stageIndex] = data;

                // ���̺� �ֽ�ȭ
                SaveLoad_Manager.instance.SaveData(SaveLoad_Manager.instance.curSlot);
            }
            else
            {
                Debug.Log($"������ �ֽ�ȭ ���� / é�� �ε��� : {chapter} / �������� �ε��� : {stageIndex}");
            }
        }
    }

    /// <summary>
    /// �������� ���� UI �ֽ�ȭ
    /// </summary>
    private void SelectUI_Setting()
    {

    }
    #endregion


    #region Button Event
    /// <summary>
    /// �������� ���� UI
    /// </summary>
    /// <param name="isOn"></param>
    public void Click_Select(bool isOn)
    {
        // 1. �÷��̾��� ���൵ �����͸� �����ϴ� ���𰡿��� ������ �޾ƿ���

        // 2. �޾ƿ� ������ ��� UI Ȱ��ȭ
        selectSet.SetActive(isOn);
    }

    /// <summary>
    /// �������� �̵� ���
    /// </summary>
    public void Click_Stage(string sceneName)
    {
        StartCoroutine(StageCall(sceneName));
    }

    private IEnumerator StageCall(string sceneName)
    {
        // ���� ���� - ��������?
        UI_Manager.instance.Fade(true, 0.75f);
        while (UI_Manager.instance.isFade)
        {
            yield return null;
        }

        // �� �̵�
        SceneLoad_Manager.LoadScene(sceneName);
    }

    /// <summary>
    /// ���� ������ ���̺� ��ư
    /// </summary>
    public void Click_Save()
    {
        SaveLoad_Manager.instance.SaveUI(true);
    }
    #endregion
}
