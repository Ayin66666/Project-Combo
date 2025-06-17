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
    /// ���� ���� �� ������ �ε� �� ����
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
    #endregion


    /// <summary>
    /// ���׹̳� �ڿ� ȸ��
    /// </summary>
    private void Recovery()
    {
        if (curStamina < maxStamina)
            curStamina += Time.deltaTime * staminaRecovery;
    }


    #region Item Recovery & Equipment
    /// <summary>
    /// ü�� ȸ��
    /// </summary>
    /// <param name="value"></param>
    public void HpAdd(int value)
    {
        curhp += value;
        if (curhp >= maxHp)
        {
            curhp = maxHp;
        }
    }

    /// <summary>
    /// ���׹̳� ȸ��
    /// </summary>
    /// <param name="value"></param>
    public void StaminaAdd(int value)
    {
        curStamina += value;
        if (curStamina >= maxStamina)
        {
            curStamina = maxStamina;
        }
    }

    /// <summary>
    /// ���� ������ ȸ��
    /// </summary>
    /// <param name="value"></param>
    public void AwankingAdd(int value)
    {
        curAwakening += value;
        if (curAwakening >= maxAwakening)
        {
            // �ִ�ġ�� ���� ���
            curAwakening = maxAwakening;

            // ���� Ȱ��ȭ
            if (!PlayerAction_Manager.instance.canAwakning)
                PlayerAction_Manager.instance.canAwakning = true;
        }
    }

    /// <summary>
    /// ��� ���� & ���� �� �������ͽ� ���� ����
    /// </summary>
    /// <param name="isOn"></param>
    /// <param name="status"></param>
    public void Equipment_Status_Setting(bool isEquip, Equipment_Status_SO status)
    {
        int equip = isEquip ? 1 : -1;

        // �������ͽ� ����
        physicalDamage += status.PhysicalDamage * equip;
        magicalDamage += status.MagicalDamage * equip;
        criticalhit += status.CriticalHit * equip;
        critical_multiplier += status.CriticalMultiplier * equip;
        attackSpeed += status.AttackSpeed * equip;

        maxHp += status.MaxHp * equip;
        physicalDefence += status.PhysicalDefence * equip;
        magicalDefence += status.MagicalDefence * equip;

        moveSpeed += status.MoveSpeed * equip;
        maxStamina += status.MaxStamina * equip;
        maxAwakening += status.MaxAwakening * equip;
        staminaRecovery += status.StaminaRecovery * equip;
    }
    #endregion


    #region Level Up
    /// <summary>
    /// ���� ���� �� ����ġ �䱸�� ����
    /// </summary>
    public void Level_Setting()
    {
        // ���� ����
        maxLevel = 25;

        // ����ġ �䱸���� �ش� ������� 1.5 �����͸� ���
        for (int i = 1; i < maxLevel; i++)
        {
            int a = (int)(baseExp * Mathf.Pow(i, growth));
            expList.Add(a);
        }

        // ���� ����ġ
        maxExp = expList[curLevel - 1];
    }

    /// <summary>
    /// ����ġ ���� ����
    /// </summary>
    /// <param name="exp"></param>
    public void ExpAdd(int exp)
    {
        curExp += exp;
        while (curExp >= maxExp && curLevel < expList.Count - 1)
        {
            curExp -= maxExp;
            LevelUp();

            maxExp = expList[curLevel - 1];
        }
    }

    /// <summary>
    /// ���� �� �� ���� �������ͽ� ����
    /// </summary>
    public void LevelUp()
    {
        curLevel += 1;

        // �⺻ ����
        curhp += 25;
        maxHp += 25;
        physicalDefence += 2;
        magicalDefence += 2;
        physicalDamage += 5;
        magicalDamage += 5;

        // ��ų����Ʈ ����
        Player_Manager.instance.skill.Skill_PointAdd(5);

        // 5���� �� ����
        if (curLevel % 5 == 0)
        {
            critical_multiplier += 0.05f;
            staminaRecovery += 0.01f;
        }

        // UI �ֽ�ȭ
        UI_Manager.instance.Level(curLevel);

        // ������ ����Ʈ UI
        UI_Manager.instance.LevelUpUI(curLevel);
    }
    #endregion
}
