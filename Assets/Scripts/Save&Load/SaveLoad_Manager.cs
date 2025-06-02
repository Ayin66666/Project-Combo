using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using System.IO;


/// <summary>
/// ��� ������ �Ѱ� ����
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
    /// ������ ���̺� ���
    /// </summary>
    /// <param name="index"></param>
    public void SaveData(int index)
    {
        // �̹� ����� �����Ͱ� ���� ��� ���
        bool s = CheckData(index);
        Debug.Log(s);
        if (CheckData(index))
        {
            // ����� �ȳ� UI
            StartCoroutine(CoverDataCall(index));
        }
        else
        {
            // �÷��̾� ������ ����
            SaveResultUI(Create_Data(index));
        }
    }


    /// <summary>
    /// ���� ���
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private bool Save(int index)
    {
        try
        {
            Data playerData = new()
            {
                // �������ͽ�
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


                // ������


                // ��������
            };

            // ������ ����
            string data = JsonUtility.ToJson(playerData);
            File.WriteAllText(savePath + fileName[index], data);

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Save Error] ������ ���� ����: {e.Message}\n{e.StackTrace}");
            return false;
        }
    }

    private IEnumerator CoverDataCall(int index)
    {
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
            // ������ ����
            SaveResultUI(Save(index));
        }
    }

    public void Save_StageData(int chapterIndex, ChapterData data)
    {

    }

    /// <summary>
    /// ó�� ���� �� �ű� ������ ����
    /// </summary>
    public bool Create_Data(int slotCount)
    {
        Data data = new Data()
        {
            // �������ͽ�
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


            // ������
            inevntory = new List<int>(40),
            equipment = new List<int>(8),


            // ��������
            clearData = new ClearData()
            {
                chapterList = new List<ChapterData>(chapterDatas.Count)
            }
        };


        // ������ �ڵ� �ʱ�ȭ
        for (int i = 0; i < 40; i++)
        {
            data.inevntory.Add(-1);
        }

        // ��� �ڵ� �ʱ�ȭ
        for (int i = 0; i < 8; i++)
        {
            data.equipment.Add(-1);
        }


        // �������� ���� ������ ����
        for (int i = 0; i < chapterDatas.Count; i++)
        {
            // é�� ������ ����
            ChapterData chapter = new ChapterData
            {
                chapterName = chapterDatas[i].chapterName,
                stageList = new List<StageData>()
            };


            // �������� ������ ����
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

        // ������ ����
        try
        {
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(savePath + fileName[slotCount], json);

            // ������ ���� - �������ͽ�
            Player_Status.instacne.Status_Setting(data);

            // ������ ���� - �κ��丮

            // ������ ���� - ��ųƮ��

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"���� ���� �߻�! {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// ������ �ε� ���
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
    #endregion
}
