using System.Collections.Generic;
using UnityEngine;


public class Start_Manager : MonoBehaviour
{
    public static Start_Manager instacne;

    [Header("---State---")]
    [SerializeField] private UI curUI;
    private enum UI { None, Start, Option, Extra, Exit }


    [Header("---UI---")]
    [SerializeField] private GameObject extraSet;
    [SerializeField] private GameObject exitSet;
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
        // ���� �� �˸���
        SaveLoad_Manager.instance.isStartScene = true;

        // OnOff �� ����Ʈ �߰�
        uiList.Add(extraSet);
        uiList.Add(exitSet);
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
    /// �ɼ� ��ư
    /// </summary>
    public void Click_Option()
    {
        curUI = UI.Start;
        UI_Manager.instance.PlayerUI_Setting(3);
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
