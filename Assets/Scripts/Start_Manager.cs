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
    /// ���� ���� UI ǥ��
    /// </summary>
    /// <param name="index"></param>
    /// <param name="levelText"></param>
    /// <param name="timeText"></param>
    public void Slot_Setting(int index, string levelText, string timeText)
    {
        slots[index].Slot_Setting(levelText, timeText);
    }


    #region ��ư �̺�Ʈ
    /// <summary>
    /// ������ ��ư
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
    /// ���� ��ư
    /// </summary>
    public void Click_Start()
    {
        curUI = UI.Start;
        selectSet.SetActive(true);
    }

    /// <summary>
    /// select ���Կ��� Ŭ������ �� ȣ�� - ���ڰ��� �����Ͱ� �ִ��� üũ �� ����
    /// </summary>
    /// <param name="index"></param>
    public void Click_Slot(int index)
    {
        // ������ �׽�Ʈ��
        /*
        if (SaveLoad_Manager.instance.CheckData(index))
        {
            // �ش� ���Կ� ����� �����Ͱ� ���� ���

            // ������ �ε�
            SaveLoad_Manager.instance.LoadData(index);

            // ���̷�����
            SceneLoad_Manager.LoadScene("0.Hideout");
        }
        else
        {
            // ����� �����Ͱ� ���ٸ� - �ű� ������ ����

            // Ʃ�丮�� �̵�
            SceneLoad_Manager.LoadScene("1.Chapter1_Tutorial");
        }
        */

        // �Ź���
        if(SaveLoad_Manager.instance.CheckData(index))
        {
            // ������ �ε�
            Data data = SaveLoad_Manager.instance.LoadData(index);

            // ������ ����

            // �� �̵�
            SceneLoad_Manager.LoadScene("0.Hideout");
        }
        else
        {
            // ������ ����
            SaveLoad_Manager.instance.Create_Data(index);

            // Ʃ�丮�� �̵�
            SceneLoad_Manager.LoadScene("1.Chapter1_Tutorial");
        }
    }

    /// <summary>
    /// �ɼ� ��ư
    /// </summary>
    public void Click_Option()
    {
        curUI = UI.Start;
        optionSet.SetActive(true);
    }

    /// <summary>
    /// ũ���� ��ư
    /// </summary>
    public void Click_Extra()
    {
        curUI = UI.Start;
        extraSet.SetActive(true);
    }

    /// <summary>
    /// ���� ���� ��ư
    /// </summary>
    public void Click_Exit()
    {
        curUI = UI.Start;
        exitSet.SetActive(true);
    }

    /// <summary>
    /// ���� ����
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
