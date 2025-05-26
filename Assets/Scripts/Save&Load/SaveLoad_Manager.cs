using System.Collections.Generic;
using UnityEngine;
using System.IO; // 인풋 아웃풋
using System.Collections; 

public class PlayerData
{
    public int level;
    public List<int> itemCode;

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
}


public class SaveLoad_Manager : MonoBehaviour
{
    public static SaveLoad_Manager instance;

    [Header("---Save Setting---")]
    [SerializeField] private string path;
    [SerializeField] private string[] filename;


    [Header("---UI---")]
    [SerializeField] private GameObject saveUIset; // 이거 위치 고민해볼것 - 만약 여기 들어가면 소팅 레이어도 고민해야함!
    [SerializeField] private GameObject coverUISet;
    private bool isCover;

    [SerializeField] private GameObject saveSuccessSet;
    [SerializeField] private CanvasGroup saveSuccessCanvas;
    private Coroutine saveSuccessCoroutine;


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

        DontDestroyOnLoad(this.gameObject);
        path = Application.persistentDataPath + "/";
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
    /// 데이터 세이브 기능
    /// </summary>
    /// <param name="index"></param>
    public void SaveData(int index)
    {
        // 이미 저장된 데이터가 있을 경우 대비
        string data = path + filename[index];
        if (File.Exists(data))
        {
            // 덮어쓰기 안내 UI
            StartCoroutine(CoverData(index));
        }
        else
        {
            // 데이터 저장
            data = JsonUtility.ToJson(GetPlayerStatus());
            File.WriteAllText(path + filename[index], data);

            // 세이브 성공 UI
            SaveSuccessUI();
        }
    }

    public PlayerData GetPlayerStatus()
    {
        PlayerData playerData = new()
        {
            level = Player_Manager.instance.level,

            // Defence
            curhp = Player_Manager.instance.curhp,
            maxHp = Player_Manager.instance.maxHp,
            physicalDefence = Player_Manager.instance.physicalDefence,
            magicalDefence = Player_Manager.instance.magicalDefence,

            // Damage
            physicalDamage = Player_Manager.instance.physicalDamage,
            magicalDamage = Player_Manager.instance.magicalDamage,
            attackSpeed = Player_Manager.instance.attackSpeed,
            criticalhit = Player_Manager.instance.criticalhit,
            critical_multiplier = Player_Manager.instance.critical_multiplier,

            // Othehr
            moveSpeed = Player_Manager.instance.moveSpeed,
            curAwakening = Player_Manager.instance.curAwakening,
            maxAwakening = Player_Manager.instance.maxAwakening,
            curStamina = Player_Manager.instance.curStamina,
            maxStamina = Player_Manager.instance.maxStamina
        };

        return playerData;
    }

    public void CoverSetting(bool isOn)
    {
        isCover = isOn;
    }

    /// <summary>
    /// 이미 슬롯에 데이터가 있는 경우 동작
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private IEnumerator CoverData(int index)
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
            string data = JsonUtility.ToJson(GetPlayerStatus());
            File.WriteAllText(path + filename[index], data);

            // 세이브 성공 UI
            SaveSuccessUI();
        }
        else
        {
            // 아닐 경우 그냥 넘어감
        }
    }

    /// <summary>
    /// 데이터 로드 기능
    /// </summary>
    /// <param name="index"></param>
    public void LoadData(int index)
    {
        string data = path + filename[index];
        if (!File.Exists(data))
        {
            // 로드 파일 없음!
            Debug.LogWarning("로드할 파일이 없습니다: " + data);
            return;
        }

        try
        {
            // 로드 시도
            string json = File.ReadAllText(data);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

            // 플레이어 매니저에 데이터 전달
            // Player_Manager.instance.Status_Setting(playerData);
        }
        catch (IOException ex)
        {
            // 로드를 실패할 경우
            Debug.LogError("Load failed: " + ex.Message);
        }
    }
    #endregion
}
