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
        // ������ �޾ƿͼ� ����
        clearData = data.clearData[chapterCount];
    }

    /// <summary>
    /// �������� Ŭ���� �� ������ ����
    /// </summary>
    /// <param name="index"></param>
    /// <param name="data"></param>
    public void Stage_Clear(int index, StageData data)
    {
        clearData.stage[index] = data;
    }

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
