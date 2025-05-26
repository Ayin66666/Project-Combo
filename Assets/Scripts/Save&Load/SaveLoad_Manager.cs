using System.Collections.Generic;
using UnityEngine;
using System.IO; // ��ǲ �ƿ�ǲ
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
    [SerializeField] private GameObject saveUIset; // �̰� ��ġ ����غ��� - ���� ���� ���� ���� ���̾ ����ؾ���!
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

        // ������
        yield return new WaitForSeconds(0.25f);

        // ���̵� �ƿ�
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
    /// ������ ���̺� ���
    /// </summary>
    /// <param name="index"></param>
    public void SaveData(int index)
    {
        // �̹� ����� �����Ͱ� ���� ��� ���
        string data = path + filename[index];
        if (File.Exists(data))
        {
            // ����� �ȳ� UI
            StartCoroutine(CoverData(index));
        }
        else
        {
            // ������ ����
            data = JsonUtility.ToJson(GetPlayerStatus());
            File.WriteAllText(path + filename[index], data);

            // ���̺� ���� UI
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
    /// �̹� ���Կ� �����Ͱ� �ִ� ��� ����
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private IEnumerator CoverData(int index)
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
            string data = JsonUtility.ToJson(GetPlayerStatus());
            File.WriteAllText(path + filename[index], data);

            // ���̺� ���� UI
            SaveSuccessUI();
        }
        else
        {
            // �ƴ� ��� �׳� �Ѿ
        }
    }

    /// <summary>
    /// ������ �ε� ���
    /// </summary>
    /// <param name="index"></param>
    public void LoadData(int index)
    {
        string data = path + filename[index];
        if (!File.Exists(data))
        {
            // �ε� ���� ����!
            Debug.LogWarning("�ε��� ������ �����ϴ�: " + data);
            return;
        }

        try
        {
            // �ε� �õ�
            string json = File.ReadAllText(data);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

            // �÷��̾� �Ŵ����� ������ ����
            // Player_Manager.instance.Status_Setting(playerData);
        }
        catch (IOException ex)
        {
            // �ε带 ������ ���
            Debug.LogError("Load failed: " + ex.Message);
        }
    }
    #endregion
}
