using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using System.IO;


/// <summary>
/// 모든 데이터 총괄 묶음
/// </summary>
[System.Serializable]
public class Data
{
    [Header("---Ohter---")]
    public float playTime;


    [Header("---Status---")]
    public int level;

    // Defence Status
    public int curhp;
    public int maxHp;
    public int physicalDefence;
    public int magicalDefence;

    // Attack Status
    public int physicalDamage;
    public int magicalDamage;
    public float attackSpeed;
    public float criticalhit;
    public float critical_multiplier;

    // Other Status
    public float moveSpeed;
    public float curStamina;
    public float maxStamina;
    public float curAwakening;
    public float maxAwakening;


    [Header("---Item---")]
    public List<int> inevntory;
    public List<int> equipment;


    [Header("---Chapter---")]
    public ClearData clearData;
}

#region Stage Data
[System.Serializable]
public class ClearData
{
    // 챕터 묶음 - 최종 저장 데이터
    public List<ChapterData> chapterList;
}

[System.Serializable]
public class ChapterData
{
    // 챕터 데이터 - 각 장의 데이터 묶음
    public string chapterName;
    public List<StageData> stageList;
}

[System.Serializable]
public class StageData
{
    // 챕터 내에 존재하는 스테이지 데이터
    public bool isClear;
    public Rank clearRank;
    public float clearTime;
    public enum Rank { None, D, C, B, A, S }
}
#endregion


public class SaveLoad_Manager : MonoBehaviour
{
    public static SaveLoad_Manager instance;


    [Header("---Save Setting---")]
    public string savePath;
    public List<string> fileName;
    public int curSlot;
    [SerializeField] private List<Chapter_Data_SO> chapterDatas;


    [Header("---UI---")]
    public bool isCover;
    public GameObject saveUIset;
    public GameObject coverUISet;
    [SerializeField] private GameObject saveResultSet;
    [SerializeField] private CanvasGroup saveResultCanvas;
    [SerializeField] private TextMeshProUGUI saveResultText;
    private Coroutine saveUICoroutine;


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
    public Data SlotUI(int slotIndex)
    {
        // 경로 유효성 체크
        string path = savePath + fileName[slotIndex];
        if (!File.Exists(path))
        {
            Debug.LogWarning($"세이브 & 로드 슬롯 {slotIndex} 에 파일이 없습니다.");
            return null;
        }

        //데이터 불러오기
        try
        {
            string json = File.ReadAllText(path);
            Data data = JsonUtility.FromJson<Data>(json);
            return data;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"세이브 & 로드 슬롯 {slotIndex} 로드 중 예외 발생 : {ex.Message}");
            return null;
        }
    }

    public void SaveUI(bool isOn)
    {
        saveUIset.SetActive(isOn);
    }

    public void CoverUI(bool isOn)
    {
        coverUISet.SetActive(isOn);
    }

    public void SaveResultUI(bool isSuccess)
    {
        if (saveUICoroutine != null)
            StopCoroutine(saveUICoroutine);

        saveUICoroutine = StartCoroutine(SaveSuccessCall(isSuccess));
    }

    private IEnumerator SaveSuccessCall(bool isSuccess)
    {
        saveResultCanvas.alpha = 1f;
        saveResultText.text = isSuccess ? "세이브 완료" : "세이브 실패";
        saveResultSet.SetActive(true);

        // 딜레이
        yield return new WaitForSeconds(0.25f);

        // 페이드 아웃
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            saveResultCanvas.alpha = Mathf.Lerp(1, 0, timer);
            yield return null;
        }

        saveResultCanvas.alpha = 0;
        saveResultSet.SetActive(false);
    }
    #endregion


    #region Save & Load
    /// <summary>
    /// 해당 경로에 데이터가 있는지 반환
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool CheckData(int index)
    {
        // 이미 저장된 데이터가 있을 경우 대비
        string data = savePath + fileName[index];
        if (File.Exists(data))
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
        bool s = CheckData(index);
        Debug.Log(s);
        if (CheckData(index))
        {
            // 덮어쓰기 안내 UI
            StartCoroutine(CoverDataCall(index));
        }
        else
        {
            // 플레이어 데이터 생성
            SaveResultUI(Create_Data(index));
        }
    }


    /// <summary>
    /// 저장 기능
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private bool Save(int index)
    {
        try
        {
            Data playerData = new()
            {
                // 스테이터스
                level = Player_Manager.instance.status.level,
                curhp = Player_Manager.instance.status.curhp,
                maxHp = Player_Manager.instance.status.maxHp,
                physicalDefence = Player_Manager.instance.status.physicalDefence,
                magicalDefence = Player_Manager.instance.status.magicalDefence,

                physicalDamage = Player_Manager.instance.status.physicalDamage,
                magicalDamage = Player_Manager.instance.status.magicalDamage,
                attackSpeed = Player_Manager.instance.status.attackSpeed,
                criticalhit = Player_Manager.instance.status.criticalhit,
                critical_multiplier = Player_Manager.instance.status.critical_multiplier,

                moveSpeed = Player_Manager.instance.status.moveSpeed,
                curAwakening = Player_Manager.instance.status.curAwakening,
                maxAwakening = Player_Manager.instance.status.maxAwakening,
                curStamina = Player_Manager.instance.status.curStamina,
                maxStamina = Player_Manager.instance.status.maxStamina,


                // 아이템


                // 스테이지
            };

            // 데이터 저장
            string data = JsonUtility.ToJson(playerData);
            File.WriteAllText(savePath + fileName[index], data);

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Save Error] 데이터 저장 실패: {e.Message}\n{e.StackTrace}");
            return false;
        }
    }

    private IEnumerator CoverDataCall(int index)
    {
        // 조건 초기화
        isCover = false;

        // UI 활성화
        CoverUI(true);

        // UI 종료 대기
        while (coverUISet.activeSelf)
        {
            yield return null;
        }

        // 조건 만족시 저장
        if (isCover)
        {
            // 데이터 저장
            SaveResultUI(Save(index));
        }
    }

    public void Save_StageData(int chapterIndex, ChapterData data)
    {

    }

    /// <summary>
    /// 처음 시작 시 신규 데이터 생성
    /// </summary>
    public bool Create_Data(int slotCount)
    {
        Data data = new Data()
        {
            // 스테이터스
            level = 1,
            curhp = 500,
            maxHp = 500,
            physicalDefence = 15,
            magicalDefence = 15,

            physicalDamage = 30,
            magicalDamage = 30,
            attackSpeed = 1f,
            criticalhit = 0,
            critical_multiplier = 1.5f,

            moveSpeed = 10f,
            curAwakening = 0,
            maxAwakening = 200,
            curStamina = 200,
            maxStamina = 200,


            // 아이템
            inevntory = new List<int>(40),
            equipment = new List<int>(8),


            // 스테이지
            clearData = new ClearData()
            {
                chapterList = new List<ChapterData>(chapterDatas.Count)
            }
        };


        // 아이템 코드 초기화
        for (int i = 0; i < 40; i++)
        {
            data.inevntory.Add(-1);
        }

        // 장비 코드 초기화
        for (int i = 0; i < 8; i++)
        {
            data.equipment.Add(-1);
        }


        // 스테이지 세부 데이터 저장
        for (int i = 0; i < chapterDatas.Count; i++)
        {
            // 챕터 데이터 생성
            ChapterData chapter = new ChapterData
            {
                chapterName = chapterDatas[i].chapterName,
                stageList = new List<StageData>()
            };


            // 스테이지 데이터 생성
            for (int j = 0; j < chapterDatas[i].stageData.Count; j++)
            {
                StageData stage = new StageData()
                {
                    isClear = false,
                    clearRank = StageData.Rank.None,
                    clearTime = 0
                };

                chapter.stageList.Add(stage);
            }

            data.clearData.chapterList.Add(chapter);
        }

        // 데이터 저장
        try
        {
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(savePath + fileName[slotCount], json);

            // 데이터 적용 - 스테이터스
            Player_Status.instacne.Status_Setting(data);

            // 데이터 적용 - 인벤토리

            // 데이터 적용 - 스킬트리

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"저장 에러 발생! {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 데이터 로드 기능
    /// </summary>
    /// <param name="slotIndex"></param>
    public Data LoadData(int slotIndex)
    {
        if (!CheckData(slotIndex))
        {
            // 로드 파일 없음!
            Debug.LogWarning("로드할 파일이 없습니다: ");
            return null;
        }

        try
        {
            //데이터 불러오기
            string json = File.ReadAllText(savePath + fileName[slotIndex]);
            Data data = JsonUtility.FromJson<Data>(json);

            // 방어코딩 - 혹시라도 데이터 문제가 있을 경우
            if (data == null)
            {
                Debug.Log($"로드 에러 발생 : {savePath + fileName[slotIndex]}");
                return null;
            }

            // 데이터 전달
            return data;
        }
        catch (IOException ex)
        {
            // 로드를 실패할 경우
            Debug.LogError("Load failed: " + ex.Message);
            return null;
        }
    }
    #endregion
}
