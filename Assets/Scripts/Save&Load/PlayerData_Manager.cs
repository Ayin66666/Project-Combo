using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Collections;
using static Attack_Base;


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


public class PlayerData_Manager : MonoBehaviour
{
    [Header("---Save Setting---")]
    SaveLoad_Manager sd = SaveLoad_Manager.instance;


    /// <summary>
    /// �ش� ��ο� �����Ͱ� �ִ��� ��ȯ
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool CheckData(int index)
    {
        // �̹� ����� �����Ͱ� ���� ��� ���
        string data = sd.savePath + sd.fileName[index].filename_PlayerData;
        if (File.Exists(data))
        {
            return true;
        }
        else
        {
            return false;
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


    public void SaveData(int index)
    {
        // ������ ����
        string data = JsonUtility.ToJson(GetPlayerStatus());
        File.WriteAllText(sd.savePath + sd.fileName[index].filename_PlayerData, data);

        // ���̺� ���� UI
        SaveLoad_Manager.instance.SaveSuccessUI();
    }

    /// <summary>
    /// �̹� ���Կ� �����Ͱ� �ִ� ��� ����
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private IEnumerator CoverDataCall(int index)
    {
        // ���� �ʱ�ȭ
        SaveLoad_Manager.instance.isCover = false;

        // UI Ȱ��ȭ
        SaveLoad_Manager.instance.CoverUI(true);

        // UI ���� ���
        while (SaveLoad_Manager.instance.coverUISet.activeSelf)
        {
            yield return null;
        }

        // ���� ������ ����
        if (SaveLoad_Manager.instance.isCover)
        {
            // ������ ����
            SaveData(index);

            // ���̺� ���� UI
            SaveLoad_Manager.instance.SaveSuccessUI();
        }
    }

    /// <summary>
    /// ������ �ε� ���
    /// </summary>
    /// <param name="index"></param>
    public void LoadData(int index)
    {
        string data = sd.savePath + sd.fileName[index].filename_PlayerData;
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

            // �������ͽ� ����
            Player_Manager.instance.Status_Setting(playerData);
        }
        catch (IOException ex)
        {
            // �ε带 ������ ���
            Debug.LogError("Load failed: " + ex.Message);
        }
    }
}
