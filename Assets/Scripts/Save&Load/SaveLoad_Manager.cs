using System.Collections.Generic;
using UnityEngine;
using System.IO; // 인풋 아웃풋
using System.Collections; 


public class SaveLoad_Manager : MonoBehaviour
{
    public static SaveLoad_Manager instance;

    [Header("---Save Setting---")]
    public string savePath;
    public List<Path> fileName;
    public int curSlot;

    [System.Serializable]
    public struct Path
    {
        public string filename_PlayerData;
        public string filename_stageData;
    }


    [Header("---UI---")]
    public bool isCover;
    public GameObject saveUIset;
    public GameObject coverUISet;
    [SerializeField] private GameObject saveSuccessSet;
    [SerializeField] private CanvasGroup saveSuccessCanvas;
    private Coroutine saveSuccessCoroutine;


    [Header("---Component---")]
    public PlayerData_Manager playerData_Manager;
    public Chapter_Manager chapter_Manager;


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
        savePath = Application.persistentDataPath + "/";
    }


    #region UI
    public void SaveUI(bool isOn)
    {
        saveUIset.SetActive(isOn);
    }

    public void CoverUI(bool isOn)
    {
        coverUISet.SetActive(isOn);
    }

    public void SaveSuccessUI()
    {
        if(saveSuccessCoroutine != null)
            StopCoroutine(saveSuccessCoroutine);

        saveSuccessCoroutine = StartCoroutine(SaveSuccessCall());
    }

    private IEnumerator SaveSuccessCall()
    {
        saveSuccessCanvas.alpha = 1f;
        saveSuccessSet.SetActive(true);

        // 딜레이
        yield return new WaitForSeconds(0.25f);

        // 페이드 아웃
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            saveSuccessCanvas.alpha = Mathf.Lerp(1, 0, timer);
            yield return null;
        }

        saveSuccessCanvas.alpha = 0;
        saveSuccessSet.SetActive(false);
    }
    #endregion


    #region Save & Load
    /// <summary>
    /// 해당 슬롯에 데이터가 있는지 체크 - 플레이어 & 스테이지 둘 다 있어야함!
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool CheckData(int index)
    {
        if (playerData_Manager.CheckData(index) && chapter_Manager.CheckData(index))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 데이터 세이브 기능
    /// </summary>
    /// <param name="index"></param>
    public void SaveData(int index)
    {
        // 이미 저장된 데이터가 있을 경우 대비
        if (CheckData(index))
        {
            // 덮어쓰기 안내 UI

        }
        else
        {
            // 플레이어 데이터 저장
            playerData_Manager.SaveData(index);

            // 맵 데이터 저장
            chapter_Manager.SaveData(index);

            // 세이브 성공 UI
            SaveSuccessUI();
        }
    }

    /// <summary>
    /// 데이터 로드 기능
    /// </summary>
    /// <param name="index"></param>
    public void LoadData(int index)
    {
        if (!CheckData(index))
        {
            // 로드 파일 없음!
            Debug.LogWarning("로드할 파일이 없습니다: ");
            return;
        }

        try
        {
            // 스테이터스 로드
            playerData_Manager.LoadData(index);

            // 맵 데이터 로드
            chapter_Manager.LoadData(index);

        }
        catch (IOException ex)
        {
            // 로드를 실패할 경우
            Debug.LogError("Load failed: " + ex.Message);
        }
    }
    #endregion
}
