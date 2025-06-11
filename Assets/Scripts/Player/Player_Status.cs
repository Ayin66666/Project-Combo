using System.Collections.Generic;
using UnityEngine;


public class Player_Status : MonoBehaviour
{
    public static Player_Status instacne;


    [Header("---Status---")]
    public int curLevel;
    public int maxLevel;

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

    // Level Status
    public int curExp;
    public int maxExp;

    [Header("---Level---")]
    public List<int> expList;
    private int baseExp = 500;
    private float growth = 1.5f;


    private void Awake()
    {
        if (instacne == null)
        {
            instacne = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        Recovery();
    }


    #region Start Setting
    /// <summary>
    /// 게임 시작 시 데이터 로드 후 셋팅
    /// </summary>
    /// <param name="data"></param>
    public void Status_Setting(Data data)
    {
        curLevel = data.curLevel;
        curExp = data.curExp;

        curhp = data.curhp;
        maxHp = data.maxHp;
        physicalDefence = data.physicalDefence;
        magicalDefence = data.magicalDefence;

        physicalDamage = data.physicalDamage;
        magicalDamage = data.magicalDamage;
        criticalhit = data.criticalhit;
        critical_multiplier = data.critical_multiplier;
        attackSpeed = data.attackSpeed;

        moveSpeed = data.moveSpeed;
        curStamina = data.curStamina;
        maxStamina = data.maxStamina;
        curAwakening = data.curAwakening;
        maxAwakening = data.maxAwakening;
        staminaRecovery = data.staminaRecovery;
    }

    /// <summary>
    /// 게임 시작 시 경험치 요구량 셋팅
    /// </summary>
    public void Level_Setting()
    {
        // 경험치 요구량은 해당 지수계수 1.5 데이터를 기반
        for (int i = 1; i < maxLevel; i++)
        {
            int a = (int)(baseExp * Mathf.Pow(i, growth));
            expList.Add(a);
        }
    }
    #endregion


    /// <summary>
    /// 스테미너 회복
    /// </summary>
    private void Recovery()
    {
        if (curStamina < maxStamina)
            curStamina += Time.deltaTime * staminaRecovery;
    }

    /// <summary>
    /// 각성 게이지 회복
    /// </summary>
    /// <param name="index"></param>
    public void AwankingAdd(int index)
    {
        curAwakening += index;
        if (curAwakening >= maxAwakening)
        {
            // 최대치를 넘을 경우
            curAwakening = maxAwakening;

            // 각성 활성화
            if (!PlayerAction_Manager.instance.canAwakning)
                PlayerAction_Manager.instance.canAwakning = true;
        }
    }


    #region Level Up
    /// <summary>
    /// 경험치 증가 로직
    /// </summary>
    /// <param name="exp"></param>
    public void ExpAdd(int exp)
    {
        curExp += exp;
        while (curExp >= maxExp && curLevel < expList.Count - 1)
        {
            curExp -= maxExp;
            LevelUp();

            maxExp = expList[curLevel];
        }
    }

    /// <summary>
    /// 레벨 업 시 기초 스테이터스 증가
    /// </summary>
    public void LevelUp()
    {
        curLevel += 1;

        // 기본 증가
        curhp += 25;
        maxHp += 25;
        physicalDefence += 2;
        magicalDefence += 2;
        physicalDamage += 5;
        magicalDamage += 5;

        // 스킬포인트 증가
        Player_Manager.instance.skill.Skill_PointAdd(5);

        // 5레벨 당 증가
        if (curLevel % 5 == 0)
        {
            critical_multiplier += 0.05f;
            staminaRecovery += 0.01f;
        }

        // UI 최신화
        UI_Manager.instance.Level(curLevel);

        // 레벨업 이펙트 UI
        UI_Manager.instance.LevelUpUI(curLevel);
    }
    #endregion
}
