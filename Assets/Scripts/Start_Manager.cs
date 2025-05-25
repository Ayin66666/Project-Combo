using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
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


    #region 버튼 이벤트

    /// <summary>
    /// 나가기 버튼
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
    /// 시작 버튼
    /// </summary>
    public void Click_Start()
    {
        curUI = UI.Start;
        selectSet.SetActive(true);
    }

    /// <summary>
    /// 옵션 버튼
    /// </summary>
    public void Click_Option()
    {
        curUI = UI.Start;
        optionSet.SetActive(true);
    }

    /// <summary>
    /// 크래딧 버튼
    /// </summary>
    public void Click_Extra()
    {
        curUI = UI.Start;
        extraSet.SetActive(true);
    }

    /// <summary>
    /// 게임 종료 버튼
    /// </summary>
    public void Click_Exit()
    {
        curUI = UI.Start;
        exitSet.SetActive(true);
    }

    /// <summary>
    /// 게임 종료
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
