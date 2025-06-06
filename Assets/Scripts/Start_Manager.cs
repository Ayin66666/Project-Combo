using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Start_Manager : MonoBehaviour
{
    public static Start_Manager instacne;

    [Header("---State---")]
    [SerializeField] private UI curUI;
    private bool isLoad;
    private bool isNew;
    private enum UI { None, Start, Option, Extra, Exit }


    [Header("---UI---")]
    [SerializeField] private GameObject optionSet;
    [SerializeField] private GameObject extraSet;
    [SerializeField] private GameObject exitSet;
    [SerializeField] private GameObject loadUI;
    [SerializeField] private GameObject newDataUI;
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
        uiList.Add(optionSet);
        uiList.Add(extraSet);
        uiList.Add(exitSet);
    }

    private IEnumerator LoadData(int index)
    {
        isLoad = false;

        // ������ �ε� UI
        loadUI.SetActive(true);
        while (loadUI.activeSelf)
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

            // ���̵�
            UI_Manager.instance.Fade(true, 1.25f);
            while (UI_Manager.instance.isFade)
            {
                yield return null;
            }

            // �� �̵�
            SceneLoad_Manager.LoadScene("0.Hideout");
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
        SaveLoad_Manager.instance.SaveLoadUI(true);
    }

    /// <summary>
    /// ������ ���� ���� bool �� ���� �Լ� - ��ư Ŭ�� ȣ��
    /// </summary>
    /// <param name="isNew"></param>
    public void NewData(bool isNew)
    {
        this.isNew = isNew;
        newDataUI.SetActive(false);
    }

    /// <summary>
    /// ������ ����� ���� bool �� ���� �Լ� - ��ư Ŭ�� ȣ��
    /// </summary>
    /// <param name="isLoad"></param>
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
