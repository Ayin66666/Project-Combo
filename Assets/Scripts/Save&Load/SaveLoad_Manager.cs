using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using System;
using System.IO;
using UnityEngine.SceneManagement;


#region Data
/// <summary>
/// ��� ������ �Ѱ� ����
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
    // é�� ���� - ���� ���� ������
    public List<ChapterData> chapterList;
}

[System.Serializable]
public class ChapterData
{
    // é�� ������ - �� ���� ������ ����
    public string chapterName;
    public List<StageData> stageList;
}

[System.Serializable]
public class StageData
{
    // é�� ���� �����ϴ� �������� ������
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

        // ���� ��� �� �� ����
        isStartScene = true;
        savePath = Application.persistentDataPath + "/";

        // ���� �ð� ����
        startTime = Time.realtimeSinceStartup;

        // ���� UI �ֽ�ȭ
        SlotUI_Setting();

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        pManager = Player_Manager.instance;
    }


    /// <summary>
    /// �÷���Ÿ�� ȣ�� ���
    /// </summary>
    /// <returns></returns>
    private float PlayTime()
    {
        addTime += Time.realtimeSinceStartup - startTime;
        return addTime + (Time.realtimeSinceStartup - startTime);
    }


    #region UI
    /// <summary>
    /// ���̺� & �ε� ���� UI ����
    /// </summary>
    public void SlotUI_Setting()
    {
        // ���� ������ ���� + UI �ֽ�ȭ
        for (int i = 0; i < slots.Count; i++)
        {
            Data data = Get_SlotUIData(i);
            if (data != null)
            {
                slots[i].Slot_Setting(data.chapter, data.curLevel.ToString(), data.playTime);
            }
            else
            {
                // ���� �����Ͱ� ���ٸ�
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
    /// ���̺� & �ε� ���Կ� �� ������ �ε�
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <returns></returns>
    public Data Get_SlotUIData(int slotIndex)
    {
        // ��� ��ȿ�� üũ
        string path = savePath + fileName[slotIndex];
        if (!File.Exists(path))
        {
            Debug.LogWarning($"���̺� & �ε� ���� {slotIndex} �� ������ �����ϴ�.");
            return null;
        }

        //������ �ҷ�����
        try
        {
            string json = File.ReadAllText(path);
            Data data = JsonUtility.FromJson<Data>(json);
            return data;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"���̺� & �ε� ���� {slotIndex} �ε� �� ���� �߻� : {ex.Message}");
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
        // ��ư UI Off
        SlotUI_ButtonOff();

        saveResultCanvas.alpha = 1f;
        saveResultText.text = isSuccess ? "���̺� �Ϸ�" : "���̺� ����";
        saveResultSet.SetActive(true);

        // ������
        yield return new WaitForSeconds(0.25f);

        // ���̵� �ƿ�
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
    /// �ش� ��ο� �����Ͱ� �ִ��� ��ȯ
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool CheckData(int index)
    {
        // �̹� ����� �����Ͱ� ���� ��� ���
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
    /// ���� �������� é�� üũ ���
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private string ChapterCheck(int index)
    {
        Data data = LoadData(index);
        // ������ ���� üũ
        if (data == null)
        {
            Debug.LogError($"[ChapterCheck] �����Ͱ� null�Դϴ�. index: {index}");
            return ChapterData_Manager.instance.chapterUIData[0].chapterName;
        }

        // �������� �ջ� üũ
        if (data.clearData?.chapterList == null)
        {
            Debug.LogError($"[ChapterCheck] clearData �Ǵ� chapterList�� null�Դϴ�. index: {index}");
            return ChapterData_Manager.instance.chapterUIData[0].chapterName;
        }

        // é�� üũ
        foreach (var chapter in data.clearData.chapterList)
        {
            // é�� ���� �������� Ŭ����
            foreach (var stage in chapter.stageList)
            {
                // Ŭ�������� ���� ���������� �����Ѵٸ�
                if (!stage.isClear)
                {
                    // �ش� é���� �̸� ��ȯ
                    return chapter.chapterName;
                }
            }
        }

        // ����ġ ���� ���·� �����Ͱ� ���� ���
        return ChapterData_Manager.instance.chapterUIData[0].chapterName;
    }




    private IEnumerator CreateDataCall(int index)
    {
        isNew = false;

        // ��ư UI Off
        SlotUI_ButtonOff();

        // ������ ���� UI
        newDataUI.SetActive(true);
        while (newDataUI.activeSelf)
        {
            yield return null;
        }

        // �ű� �����͸� ����
        if (isNew)
        {
            (Data data, bool isSuccess) = Create_Data(index);
            if (isSuccess)
            {
                isNew = false;

                // ���̵�
                UI_Manager.instance.Fade(true, 1.5f);
                while (UI_Manager.instance.isFade)
                {
                    yield return null;
                }

                // �÷��̾� �������ͽ�
                Player_Status.instacne.Status_Setting(data);

                // ��ųƮ��
                Player_Manager.instance.skill.Skill_Setting(data);

                // ��������
                ChapterData_Manager.instance.Data_Setting(data);

                // �κ��丮 & ���â

                // ���� UI Off
                SaveLoadUI(false);
                isStartScene = false;

                // ���� UI �ֽ�ȭ
                SlotUI_Setting();


                // ������ ���� ���� - Ʃ�丮�� �̵�
                SceneLoad_Manager.LoadScene("Chapter 1-1 Tutorial");
            }
            else
            {
                // ������ ���� ���� - ������ ���� ���� ��
                Debug.LogError($"������ ���� ����! {index}");
            }
        }
    }

    /// <summary>
    /// ó�� ���� �� �ű� ������ ����
    /// </summary>
    private (Data, bool) Create_Data(int slotCount)
    {
        Data data = new Data()
        {
            // é�� ���൵
            chapter = "Chapter 0",
            SceneName = "Chapter 1-1 Tutorial",
            playerPos = Vector3.zero,
            playTime = 0,

            // �������ͽ�
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

            // ������
            itemData = new List<ItemData>(),
            equipment = new List<int>(8),
            shortcut = new List<int>(4),

            // ��������
            clearData = new ClearData()
            {
                chapterList = new List<ChapterData>(ChapterData_Manager.instance.chapterUIData.Count)
            },


            // ��ųƮ��
            skillLevelData = new List<int>(),
            skillPoint = 0,
        };

        // ��ų ���� �Է�
        for (int i = 0; i < 12; i++)
        {
            data.skillLevelData.Add(0);
        }

        // ������ �ڵ� �ʱ�ȭ
        for (int i = 0; i < 40; i++)
        {
            data.itemData.Add(new ItemData { itemCode = -1, itemCount = 0});
        }

        // ��� �ڵ� �ʱ�ȭ
        for (int i = 0; i < 8; i++)
        {
            data.equipment.Add(-1);
        }

        // ��Ʈ�� �ʱ�ȭ
        for (int i = 0; i < 4; i++)
        {
            data.shortcut.Add(-1);
        }

        // �������� ���� ������ ����
        for (int i = 0; i < ChapterData_Manager.instance.chapterUIData.Count; i++)
        {
            // é�� ������ ����
            ChapterData chapter = new ChapterData
            {
                chapterName = ChapterData_Manager.instance.chapterUIData[i].chapterName,
                stageList = new List<StageData>()
            };


            // �������� ������ ����
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

        // ������ ����
        try
        {
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(savePath + fileName[slotCount], json);

            // ������ ���� - �������ͽ�
            Player_Manager.instance.status.Status_Setting(data);

            // ������ ���� - ����ġ
            Player_Manager.instance.status.Level_Setting();

            // ������ ���� - é��
            ChapterData_Manager.instance.Data_Setting(data);

            // ������ ���� - ��ųƮ��
            Player_Manager.instance.skill.Skill_Setting(data);

            // ������ ���� - �κ��丮 & ���â

            return (data, true);
        }
        catch (Exception e)
        {
            Debug.LogError($"���� ���� �߻�! {e.Message}");
            return (null, false);
        }
    }

    public void NewDataBool()
    {
        isNew = true;
    }




    /// <summary>
    /// ���� ���
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool Save(int index)
    {
        try
        {
            Data playerData = new()
            {
                // �������ͽ�
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

                // ��ųƮ��
                skillPoint = Player_Manager.instance.skill.skillPoint,
                skillLevelData = Player_Manager.instance.skill.GetSkillData(),

                // ������
                itemData = new List<ItemData>(),
                equipment = Player_Manager.instance.equipment.GetEquipmentData(),
                shortcut = Player_Manager.instance.shortCut.GetShortcutData(),

                // ���൵
                chapter = ChapterCheck(index),
                playTime = PlayTime(),
                SceneName = SceneManager.GetActiveScene().name,
                playerPos = Player_Manager.instance.action.transform.position,

                // �������� Ŭ����
                clearData = new ClearData()
                {
                    chapterList = ChapterData_Manager.instance.chapterData,
                }
            };


            // �κ��丮 ������ ����
            List<Vector2Int> items = Player_Manager.instance.inventory.GetItemData();
            for (int i = 0; i < 40; i++)
            {
                playerData.itemData.Add(new ItemData
                {
                    itemCode = items[i].x,
                    itemCount = items[i].y
                });
            }

            // ������ ����
            string data = JsonUtility.ToJson(playerData);
            File.WriteAllText(savePath + fileName[index], data);

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Save Error] ������ ���� ����: {e.Message}\n{e.StackTrace}");
            return false;
        }
    }

    private IEnumerator CoverDataCall(int index)
    {
        // ��ư UI Off
        SlotUI_ButtonOff();

        // ���� �ʱ�ȭ
        isCover = false;

        // UI Ȱ��ȭ
        CoverUI(true);

        // UI ���� ���
        while (coverUISet.activeSelf)
        {
            yield return null;
        }

        // ���� ������ ����
        if (isCover)
        {
            isCover = false;

            // ������ ����
            SaveResultUI(Save(index));

            // ���� �ֽ�ȭ
            SlotUI_Setting();
        }
    }

    public void CoverBool()
    {
        isCover = true;
    }



    /// <summary>
    /// ������ �ε� �� ���� ��� - ���������� �÷��̾�� ������ ����
    /// </summary>
    /// <param name="slotIndex"></param>
    public Data LoadData(int slotIndex)
    {
        if (!CheckData(slotIndex))
        {
            // �ε� ���� ����!
            Debug.LogWarning("�ε��� ������ �����ϴ�: ");
            return null;
        }

        try
        {
            //������ �ҷ�����
            string json = File.ReadAllText(savePath + fileName[slotIndex]);
            Data data = JsonUtility.FromJson<Data>(json);

            // ����ڵ� - Ȥ�ö� ������ ������ ���� ���
            if (data == null)
            {
                Debug.Log($"�ε� ���� �߻� : {savePath + fileName[slotIndex]}");
                return null;
            }

            // ������ ����
            return data;
        }
        catch (IOException ex)
        {
            // �ε带 ������ ���
            Debug.LogError("Load failed: " + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// �ε� ���� üũ
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private IEnumerator LoadDataCall(int index)
    {
        // ��ư UI Off
        SlotUI_ButtonOff();

        // ���� �ʱ�ȭ
        isLoad = false;

        // UI Ȱ��ȭ & ���� ���
        LoadUI(true);
        while (loadUI.activeSelf)
        {
            yield return null;
        }

        // ���� ������ �ε�
        if (isLoad)
        {
            isLoad = false;

            // ���� ����
            curSlot = index;

            // ������ �ε� & ����
            Data data;
            try
            {
                //������ �ҷ�����
                string json = File.ReadAllText(savePath + fileName[index]);
                data = JsonUtility.FromJson<Data>(json);

                // ����ڵ� - Ȥ�ö� ������ ������ ���� ���
                if (data == null)
                {
                    Debug.Log($"�ε� ���� �߻� : {savePath + fileName[index]}");
                    yield break;
                }
            }
            catch (IOException ex)
            {
                // �ε带 ������ ���
                data = null;
                Debug.LogError("Load failed: " + ex.Message);
            }

            // ���̵� ��
            UI_Manager.instance.Fade(true, 1.5f);
            while (UI_Manager.instance.isFade)
            {
                yield return null;
            }

            Debug.Log(data);
            Debug.Log(pManager);

            // ������ ���� - é��
            ChapterData_Manager.instance.Data_Setting(data);

            // ������ ���� - �������ͽ�
            pManager.status.Status_Setting(data);

            // ������ ���� - ����ġ
            pManager.status.Level_Setting();

            // ������ ���� - ��ųƮ��
            pManager.skill.Skill_Setting(data);

            // ������ ���� - �κ��丮 & ���â
            pManager.inventory.Inventory_Setting(data);

            // ������ ���� - ��Ʈ��
            pManager.shortCut.LoadData(data);

            // ���� UI Off
            SaveLoadUI(false);

            // ���� �ֽ�ȭ
            SlotUI_Setting();

            // �� �ε�
            isStartScene = false;
            SceneLoad_Manager.LoadScene(data.SceneName);
        }
    }

    /// <summary>
    /// �ε� ��ư
    /// </summary>
    public void LoadBool()
    {
        isLoad = true;
    }



    /// <summary>
    /// ���Կ� ����� ������ ���� ���
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public IEnumerator RemoveCall(int index)
    {
        // ��ư UI Off
        SlotUI_ButtonOff();

        isRemove = false;

        // ��� UI
        removeUI.SetActive(true);
        while (removeUI.activeSelf)
        {
            yield return null;
        }

        // �����͸� �����Ѵٸ�
        if (isRemove)
        {
            try
            {
                // ������ ����
                File.Delete(savePath + fileName[index]);

                // ���� ������ UI Off
                slots[index].ButtonOnOff(false);

                // UI �ֽ�ȭ
                SlotUI_Setting();
                // slots[index].Slot_Setting("None", "0", 0);
            }
            catch (Exception ex)
            {
                Debug.LogError($"������ ������ ����! : {ex} / {savePath + fileName[index]}");
            }
        }
    }

    public void RemoveBool()
    {
        isRemove = true;
    }
    #endregion
}
