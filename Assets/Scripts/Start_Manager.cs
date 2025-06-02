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
        // OnOff �� ����Ʈ �߰�
        uiList.Add(selectSet);
        uiList.Add(optionSet);
        uiList.Add(extraSet);
        uiList.Add(exitSet);

        // ���� ������ ���� + UI �ֽ�ȭ
        for (int i = 0; i < slots.Count; i++)
        {
            Data data = SaveLoad_Manager.instance.SlotUI(i);
            if(data != null)
            {
                slots[i].Slot_Setting(data.level.ToString(), data.playTime);
            }
        }
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
        isLoad = false;

        // �Ź���
        if (SaveLoad_Manager.instance.CheckData(index))
        {
            // ������ �ε�
            StartCoroutine(Load(index));
        }
        else
        {
            // ������ ����
            SaveLoad_Manager.instance.Create_Data(index);

            // Ʃ�丮�� �̵�
            SceneLoad_Manager.LoadScene("1.Chapter1_Tutorial");
        }
    }

    private IEnumerator Load(int index)
    {
        // ������ �ε� UI
        loadUI.SetActive(true);
        while(loadUI.activeSelf)
        {
            yield return null;
        }

        // �����͸� �ε��Ѵٸ�
        if (isLoad)
        {
            // ������ �ε�
            Data data = SaveLoad_Manager.instance.LoadData(index);

            // �������ͽ� ������ ����
            Player_Status.instacne.Status_Setting(data);

            // �κ��丮 & ��� ������ ����

            // ��ų ������ ����

            // Ŭ���� �����ʹ� �ش� ���� �������� �� ������!

            // ���� ����
            SaveLoad_Manager.instance.curSlot = index;

            // �� �̵�
            SceneLoad_Manager.LoadScene("0.Hideout");
        }
    }

    public void Loadbool(bool isLoad)
    {
        this.isLoad = isLoad;
        loadUI.SetActive(false);
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
