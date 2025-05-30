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
    public int attackSpeed;
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
    public List<GameObject> equipment;


    [Header("---Chapter---")]
    public List<ChapterData> clearData;
}


public class SaveLoad_Manager : MonoBehaviour
{
    public static SaveLoad_Manager instance;


    [Header("---Save Setting---")]
    public string savePath;
    public List<string> fileName;
    public int curSlot;


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
        if (CheckData(index))
        {
            // 덮어쓰기 안내 UI
            StartCoroutine(CoverDataCall(index));
        }
        else
        {
            // 플레이어 데이터 저장 - Save() 함수는 저장 + 불값 반환을 실행하며, 이에 따라 성공 & 실패 UI 표기가 동작함!
            SaveResultUI(Save(index));
        }
    }

    /// <summary>
    /// 저장 기능
    /// </summary>
    /// <returns></returns>
    private bool Save(int index)
    {
        try
        {
            Data playerData = new()
            {
                // 스테이터스
                level = Player_Manager.instance.level,
                curhp = Player_Manager.instance.curhp,
                maxHp = Player_Manager.instance.maxHp,
                physicalDefence = Player_Manager.instance.physicalDefence,
                magicalDefence = Player_Manager.instance.magicalDefence,

                physicalDamage = Player_Manager.instance.physicalDamage,
                magicalDamage = Player_Manager.instance.magicalDamage,
                attackSpeed = Player_Manager.instance.attackSpeed,
                criticalhit = Player_Manager.instance.criticalhit,
                critical_multiplier = Player_Manager.instance.critical_multiplier,

                moveSpeed = Player_Manager.instance.moveSpeed,
                curAwakening = Player_Manager.instance.curAwakening,
                maxAwakening = Player_Manager.instance.maxAwakening,
                curStamina = Player_Manager.instance.curStamina,
                maxStamina = Player_Manager.instance.maxStamina,


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
            if(data == null)
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
