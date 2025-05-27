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
    /// 해당 경로에 데이터가 있는지 반환
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool CheckData(int index)
    {
        // 이미 저장된 데이터가 있을 경우 대비
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
        // 데이터 저장
        string data = JsonUtility.ToJson(GetPlayerStatus());
        File.WriteAllText(sd.savePath + sd.fileName[index].filename_PlayerData, data);

        // 세이브 성공 UI
        SaveLoad_Manager.instance.SaveSuccessUI();
    }

    /// <summary>
    /// 이미 슬롯에 데이터가 있는 경우 동작
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private IEnumerator CoverDataCall(int index)
    {
        // 조건 초기화
        SaveLoad_Manager.instance.isCover = false;

        // UI 활성화
        SaveLoad_Manager.instance.CoverUI(true);

        // UI 종료 대기
        while (SaveLoad_Manager.instance.coverUISet.activeSelf)
        {
            yield return null;
        }

        // 조건 만족시 저장
        if (SaveLoad_Manager.instance.isCover)
        {
            // 데이터 저장
            SaveData(index);

            // 세이브 성공 UI
            SaveLoad_Manager.instance.SaveSuccessUI();
        }
    }

    /// <summary>
    /// 데이터 로드 기능
    /// </summary>
    /// <param name="index"></param>
    public void LoadData(int index)
    {
        string data = sd.savePath + sd.fileName[index].filename_PlayerData;
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

            // 스테이터스 전달
            Player_Manager.instance.Status_Setting(playerData);
        }
        catch (IOException ex)
        {
            // 로드를 실패할 경우
            Debug.LogError("Load failed: " + ex.Message);
        }
    }
}
