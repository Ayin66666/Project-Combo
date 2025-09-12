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

        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        // 시작 씬 알리기
        SaveLoad_Manager.instance.isStartScene = true;
        Cursor.lockState = CursorLockMode.None;

        // OnOff 용 리스트 추가
        uiList.Add(extraSet);
        uiList.Add(exitSet);

        // 만약 페이드 상태라면 - 페이드 해제
        if(UI_Manager.instance.fadeSet.activeSelf)
        {
            UI_Manager.instance.Fade(false, 1f);
        }
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
        SaveLoad_Manager.instance.SaveLoadUI(true);
    }

    /// <summary>
    /// 옵션 버튼
    /// </summary>
    public void Click_Option()
    {
        curUI = UI.Option;
        UI_Manager.instance.PlayerUI_Setting(4);
        UI_Manager.instance.PlayerUI_Setting();
    }

    /// <summary>
    /// 크래딧 버튼
    /// </summary>
    public void Click_Extra()
    {
        curUI = UI.Extra;
        extraSet.SetActive(true);
    }

    /// <summary>
    /// 게임 종료 버튼
    /// </summary>
    public void Click_Exit()
    {
        curUI = UI.Exit;
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
