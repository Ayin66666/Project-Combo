using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using System;
using System.IO;
using UnityEngine.SceneManagement;


#region Data
/// <summary>
/// 모든 데이터 총괄 묶음
/// </summary>
[System.Serializable]
public class Data
{
    [Header("---Ohter---")]
    public string chapter;
    public float playTime;
    public string SceneName;
    public Vector3 playerPos;


    [Header("---Status---")]
    public int curLevel;
    public int curExp;

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
    public float staminaRecovery;


    [Header("---Item---")]
    public List<ItemData> itemData;
    public List<int> equipment;
    public List<int> shortcut;


    [Header("---Chapter---")]
    public ClearData clearData;


    [Header("---Skill Tree---")]
    public int skillPoint;
    public List<int> skillLevelData;
}

[System.Serializable]
public class ItemData
{
    public int itemCode;
    public int itemCount;
}

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
    public enum Rank { N, D, C, B, A, S }
}
#endregion


public class SaveLoad_Manager : MonoBehaviour
{
    public static SaveLoad_Manager instance;
    private Player_Manager pManager;


    [Header("---Save Setting---")]
    public string savePath;
    public List<string> fileName;
    public int curSlot;
    public bool isStartScene;


    [Header("---Slot UI---")]
    [SerializeField] private List<Save_Slot> slots;
    public bool isUIOn;


    [Header("---Save UI---")]
    public bool isCover;
    public GameObject saveLoadUIset;
    public GameObject coverUISet;
    [SerializeField] private GameObject saveResultSet;
    [SerializeField] private CanvasGroup saveResultCanvas;
    [SerializeField] private TextMeshProUGUI saveResultText;
    private Coroutine saveUICoroutine;


    [Header("---New Data UI---")]
    [SerializeField] private GameObject newDataUI;
    public bool isNew;


    [Header("---Load UI---")]
    [SerializeField] private GameObject loadUI;
    public bool isLoad;


    [Header("---Remove UI---")]
    [SerializeField] private GameObject removeUI;
    public bool isRemove;

    [Header("---PlayTime---")]
    private float startTime;
    private float addTime;


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

        // 저장 경로 및 씬 셋팅
        isStartScene = true;
        savePath = Application.persistentDataPath + "/";

        // 시작 시간 저장
        startTime = Time.realtimeSinceStartup;

        // 슬롯 UI 최신화
        SlotUI_Setting();

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        pManager = Player_Manager.instance;
    }


    /// <summary>
    /// 플레이타임 호출 기능
    /// </summary>
    /// <returns></returns>
    private float PlayTime()
    {
        addTime += Time.realtimeSinceStartup - startTime;
        return addTime + (Time.realtimeSinceStartup - startTime);
    }


    #region UI
    /// <summary>
    /// 세이브 & 로드 슬롯 UI 셋팅
    /// </summary>
    public void SlotUI_Setting()
    {
        // 슬롯 데이터 셋팅 + UI 최신화
        for (int i = 0; i < slots.Count; i++)
        {
            Data data = Get_SlotUIData(i);
            if (data != null)
            {
                slots[i].Slot_Setting(data.chapter, data.curLevel.ToString(), data.playTime);
            }
            else
            {
                // 저장 데이터가 없다면
                slots[i].Slot_Setting("None", "0", 0);
            }
        }
    }

    public void SlotUI_ButtonOff()
    {
        foreach (Save_Slot slot in slots)
        {
            slot.ButtonOnOff(false);
        }
    }

    /// <summary>
    /// 세이브 & 로드 슬롯에 들어갈 데이터 로드
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <returns></returns>
    public Data Get_SlotUIData(int slotIndex)
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

    public void NewDataUI(bool isOn)
    {
        newDataUI.SetActive(isOn);
    }

    public void SaveLoadUI(bool isOn)
    {
        isUIOn = isOn;
        saveLoadUIset.SetActive(isOn);
    }

    public void LoadUI(bool isOn)
    {
        loadUI.SetActive(isOn);
    }

    public void CoverUI(bool isOn)
    {
        coverUISet.SetActive(isOn);
    }

    public void RemoveUI(bool isOn)
    {
        removeUI.SetActive(isOn);
    }

    public void SaveResultUI(bool isSuccess)
    {
        if (saveUICoroutine != null)
            StopCoroutine(saveUICoroutine);

        saveUICoroutine = StartCoroutine(SaveSuccessCall(isSuccess));
    }

    private IEnumerator SaveSuccessCall(bool isSuccess)
    {
        // 버튼 UI Off
        SlotUI_ButtonOff();

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


    #region Slot Click Evnet
    public void Click_Create(int index)
    {
        StartCoroutine(CreateDataCall(index));
    }

    public void Click_Save(int index)
    {
        StartCoroutine(CoverDataCall(index));
    }

    public void Click_Load(int index)
    {
        StartCoroutine(LoadDataCall(index));
    }

    public void Click_Remvoe(int index)
    {
        StartCoroutine(RemoveCall(index));
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
    /// 현재 진행중인 챕터 체크 기능
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private string ChapterCheck(int index)
    {
        Data data = LoadData(index);
        // 데이터 여부 체크
        if (data == null)
        {
            Debug.LogError($"[ChapterCheck] 데이터가 null입니다. index: {index}");
            return ChapterData_Manager.instance.chapterUIData[0].chapterName;
        }

        // 데이터의 손상 체크
        if (data.clearData?.chapterList == null)
        {
            Debug.LogError($"[ChapterCheck] clearData 또는 chapterList가 null입니다. index: {index}");
            return ChapterData_Manager.instance.chapterUIData[0].chapterName;
        }

        // 챕터 체크
        foreach (var chapter in data.clearData.chapterList)
        {
            // 챕터 내의 스테이지 클리어
            foreach (var stage in chapter.stageList)
            {
                // 클리어하지 않은 스테이지가 존재한다면
                if (!stage.isClear)
                {
                    // 해당 챕터의 이름 반환
                    return chapter.chapterName;
                }
            }
        }

        // 예기치 못한 사태로 데이터가 없을 경우
        return ChapterData_Manager.instance.chapterUIData[0].chapterName;
    }




    private IEnumerator CreateDataCall(int index)
    {
        isNew = false;

        // 버튼 UI Off
        SlotUI_ButtonOff();

        // 데이터 생성 UI
        newDataUI.SetActive(true);
        while (newDataUI.activeSelf)
        {
            yield return null;
        }

        // 신규 데이터를 생성
        if (isNew)
        {
            (Data data, bool isSuccess) = Create_Data(index);
            if (isSuccess)
            {
                isNew = false;

                // 페이드
                UI_Manager.instance.Fade(true, 1.5f);
                while (UI_Manager.instance.isFade)
                {
                    yield return null;
                }

                // 플레이어 스테이터스
                Player_Status.instacne.Status_Setting(data);

                // 스킬트리
                Player_Manager.instance.skill.Skill_Setting(data);

                // 스테이지
                ChapterData_Manager.instance.Data_Setting(data);

                // 인벤토리 & 장비창

                // 선택 UI Off
                SaveLoadUI(false);
                isStartScene = false;

                // 슬롯 UI 최신화
                SlotUI_Setting();


                // 데이터 생성 성공 - 튜토리얼 이동
                SceneLoad_Manager.LoadScene("Chapter 1-1 Tutorial");
            }
            else
            {
                // 데이터 생성 실패 - 데이터 생성 실패 시
                Debug.LogError($"데이터 생성 실패! {index}");
            }
        }
    }

    /// <summary>
    /// 처음 시작 시 신규 데이터 생성
    /// </summary>
    private (Data, bool) Create_Data(int slotCount)
    {
        Data data = new Data()
        {
            // 챕터 진행도
            chapter = "Chapter 0",
            SceneName = "Chapter 1-1 Tutorial",
            playerPos = Vector3.zero,
            playTime = 0,

            // 스테이터스
            curLevel = 1,
            curExp = 0,

            curhp = 500,
            maxHp = 500,
            physicalDefence = 15,
            magicalDefence = 15,

            physicalDamage = 60,
            magicalDamage = 60,
            attackSpeed = 1f,
            criticalhit = 0,
            critical_multiplier = 1.5f,

            moveSpeed = 10f,
            curAwakening = 0,
            maxAwakening = 200,
            curStamina = 200,
            maxStamina = 200,
            staminaRecovery = 5f,

            // 아이템
            itemData = new List<ItemData>(),
            equipment = new List<int>(8),
            shortcut = new List<int>(4),

            // 스테이지
            clearData = new ClearData()
            {
                chapterList = new List<ChapterData>(ChapterData_Manager.instance.chapterUIData.Count)
            },


            // 스킬트리
            skillLevelData = new List<int>(),
            skillPoint = 0,
        };

        // 스킬 레벨 입력
        for (int i = 0; i < 12; i++)
        {
            data.skillLevelData.Add(0);
        }

        // 아이템 코드 초기화
        for (int i = 0; i < 40; i++)
        {
            data.itemData.Add(new ItemData { itemCode = -1, itemCount = 0});
        }

        // 장비 코드 초기화
        for (int i = 0; i < 8; i++)
        {
            data.equipment.Add(-1);
        }

        // 쇼트컷 초기화
        for (int i = 0; i < 4; i++)
        {
            data.shortcut.Add(-1);
        }

        // 스테이지 세부 데이터 저장
        for (int i = 0; i < ChapterData_Manager.instance.chapterUIData.Count; i++)
        {
            // 챕터 데이터 생성
            ChapterData chapter = new ChapterData
            {
                chapterName = ChapterData_Manager.instance.chapterUIData[i].chapterName,
                stageList = new List<StageData>()
            };


            // 스테이지 데이터 생성
            for (int j = 0; j < ChapterData_Manager.instance.chapterUIData[i].stageData.Count; j++)
            {
                StageData stage = new StageData()
                {
                    isClear = false,
                    clearRank = StageData.Rank.N,
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
            Player_Manager.instance.status.Status_Setting(data);

            // 데이터 적용 - 경험치
            Player_Manager.instance.status.Level_Setting();

            // 데이터 적용 - 챕터
            ChapterData_Manager.instance.Data_Setting(data);

            // 데이터 적용 - 스킬트리
            Player_Manager.instance.skill.Skill_Setting(data);

            // 데이터 적용 - 인벤토리 & 장비창

            return (data, true);
        }
        catch (Exception e)
        {
            Debug.LogError($"저장 에러 발생! {e.Message}");
            return (null, false);
        }
    }

    public void NewDataBool()
    {
        isNew = true;
    }




    /// <summary>
    /// 저장 기능
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool Save(int index)
    {
        try
        {
            Data playerData = new()
            {
                // 스테이터스
                curLevel = Player_Manager.instance.status.curLevel,
                curExp = Player_Manager.instance.status.curExp,

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

                // 스킬트리
                skillPoint = Player_Manager.instance.skill.skillPoint,
                skillLevelData = Player_Manager.instance.skill.GetSkillData(),

                // 아이템
                itemData = new List<ItemData>(),
                equipment = Player_Manager.instance.equipment.GetEquipmentData(),
                shortcut = Player_Manager.instance.shortCut.GetShortcutData(),

                // 진행도
                chapter = ChapterCheck(index),
                playTime = PlayTime(),
                SceneName = SceneManager.GetActiveScene().name,
                playerPos = Player_Manager.instance.action.transform.position,

                // 스테이지 클리어
                clearData = new ClearData()
                {
                    chapterList = ChapterData_Manager.instance.chapterData,
                }
            };


            // 인벤토리 데이터 셋팅
            List<Vector2Int> items = Player_Manager.instance.inventory.GetItemData();
            for (int i = 0; i < 40; i++)
            {
                playerData.itemData.Add(new ItemData
                {
                    itemCode = items[i].x,
                    itemCount = items[i].y
                });
            }

            // 데이터 저장
            string data = JsonUtility.ToJson(playerData);
            File.WriteAllText(savePath + fileName[index], data);

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Save Error] 데이터 저장 실패: {e.Message}\n{e.StackTrace}");
            return false;
        }
    }

    private IEnumerator CoverDataCall(int index)
    {
        // 버튼 UI Off
        SlotUI_ButtonOff();

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
            isCover = false;

            // 데이터 저장
            SaveResultUI(Save(index));

            // 슬롯 최신화
            SlotUI_Setting();
        }
    }

    public void CoverBool()
    {
        isCover = true;
    }



    /// <summary>
    /// 데이터 로드 후 전달 기능 - 스테이지나 플레이어에게 데이터 전달
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

    /// <summary>
    /// 로드 여부 체크
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private IEnumerator LoadDataCall(int index)
    {
        // 버튼 UI Off
        SlotUI_ButtonOff();

        // 조건 초기화
        isLoad = false;

        // UI 활성화 & 종료 대기
        LoadUI(true);
        while (loadUI.activeSelf)
        {
            yield return null;
        }

        // 조건 만족시 로드
        if (isLoad)
        {
            isLoad = false;

            // 슬롯 변경
            curSlot = index;

            // 데이터 로드 & 적용
            Data data;
            try
            {
                //데이터 불러오기
                string json = File.ReadAllText(savePath + fileName[index]);
                data = JsonUtility.FromJson<Data>(json);

                // 방어코딩 - 혹시라도 데이터 문제가 있을 경우
                if (data == null)
                {
                    Debug.Log($"로드 에러 발생 : {savePath + fileName[index]}");
                    yield break;
                }
            }
            catch (IOException ex)
            {
                // 로드를 실패할 경우
                data = null;
                Debug.LogError("Load failed: " + ex.Message);
            }

            // 페이드 인
            UI_Manager.instance.Fade(true, 1.5f);
            while (UI_Manager.instance.isFade)
            {
                yield return null;
            }

            Debug.Log(data);
            Debug.Log(pManager);

            // 데이터 적용 - 챕터
            ChapterData_Manager.instance.Data_Setting(data);

            // 데이터 적용 - 스테이터스
            pManager.status.Status_Setting(data);

            // 데이터 적용 - 경험치
            pManager.status.Level_Setting();

            // 데이터 적용 - 스킬트리
            pManager.skill.Skill_Setting(data);

            // 데이터 적용 - 인벤토리 & 장비창
            pManager.inventory.Inventory_Setting(data);

            // 데이터 적용 - 쇼트컷
            pManager.shortCut.LoadData(data);

            // 선택 UI Off
            SaveLoadUI(false);

            // 슬롯 최신화
            SlotUI_Setting();

            // 씬 로딩
            isStartScene = false;
            SceneLoad_Manager.LoadScene(data.SceneName);
        }
    }

    /// <summary>
    /// 로드 버튼
    /// </summary>
    public void LoadBool()
    {
        isLoad = true;
    }



    /// <summary>
    /// 슬롯에 저장된 데이터 삭제 기능
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public IEnumerator RemoveCall(int index)
    {
        // 버튼 UI Off
        SlotUI_ButtonOff();

        isRemove = false;

        // 경고 UI
        removeUI.SetActive(true);
        while (removeUI.activeSelf)
        {
            yield return null;
        }

        // 데이터를 삭제한다면
        if (isRemove)
        {
            try
            {
                // 데이터 삭제
                File.Delete(savePath + fileName[index]);

                // 슬롯 선택지 UI Off
                slots[index].ButtonOnOff(false);

                // UI 최신화
                SlotUI_Setting();
                // slots[index].Slot_Setting("None", "0", 0);
            }
            catch (Exception ex)
            {
                Debug.LogError($"삭제할 데이터 없음! : {ex} / {savePath + fileName[index]}");
            }
        }
    }

    public void RemoveBool()
    {
        isRemove = true;
    }
    #endregion
}
