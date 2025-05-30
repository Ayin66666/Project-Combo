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
        if (CheckData(index))
        {
            // ����� �ȳ� UI
            StartCoroutine(CoverDataCall(index));
        }
        else
        {
            // �÷��̾� ������ ���� - Save() �Լ��� ���� + �Ұ� ��ȯ�� �����ϸ�, �̿� ���� ���� & ���� UI ǥ�Ⱑ ������!
            SaveResultUI(Save(index));
        }
    }

    /// <summary>
    /// ���� ���
    /// </summary>
    /// <returns></returns>
    private bool Save(int index)
    {
        try
        {
            Data playerData = new()
            {
                // �������ͽ�
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
            if(data == null)
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
