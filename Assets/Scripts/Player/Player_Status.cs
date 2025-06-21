using System.Collections.Generic;
using UnityEngine;
using static Item_Equipment;


public class Player_Status : MonoBehaviour
{
    public static Player_Status instacne;
    public enum RecoveryType { Hp, Stamina, Awakening }


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
    /// ������ & ȿ���� ���� ȸ�� ���
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public void Recovery(RecoveryType type, int value)
    {
        // ȸ�� ����
        switch (type)
        {
            case RecoveryType.Hp:
                curhp += value;
                if (curhp >= maxHp)
                {
                    // �������ͽ� ����
                    curhp = maxHp;
                }
                break;

            case RecoveryType.Stamina:
                curStamina += value;
                if (curStamina >= maxStamina)
                {
                    // �������ͽ� ����
                    curStamina = maxStamina;
                }
                break;

            case RecoveryType.Awakening:
                curAwakening += value;
                if (curAwakening >= maxAwakening)
                {
                    // �ִ�ġ�� ���� ���
                    curAwakening = maxAwakening;

                    // ���� Ȱ��ȭ
                    if (!PlayerAction_Manager.instance.canAwakning)
                    {
                        // ���� Ȱ��ȭ
                        PlayerAction_Manager.instance.canAwakning = true;

                        // ���� UI ǥ��
                        UI_Manager.instance.Awakening_Setting(true);
                    }
                }
                break;
        }

        // UI ǥ��
        UI_Manager.instance.PlayerUI_Recovery(type, value);
    }

    /// <summary>
    /// ��� ���� & ���� �� �������ͽ� ���� ����
    /// </summary>
    /// <param name="isOn"></param>
    /// <param name="status"></param>
    public void Equipment_Status_Setting(bool isEquip, ItemStatus status)
    {
        int equip = isEquip ? 1 : -1;

        // �������ͽ� ����
        physicalDamage += status.physicalDamage * equip;
        magicalDamage += status.magicalDamage * equip;
        criticalhit += status.criticalhit * equip;
        critical_multiplier += status.critical_multiplier * equip;
        attackSpeed += status.attackSpeed * equip;

        maxHp += status.maxHp * equip;
        physicalDefence += status.physicalDefence * equip;
        magicalDefence += status.magicalDefence * equip;

        moveSpeed += status.moveSpeed * equip;
        maxStamina += status.maxStamina * equip;
        maxAwakening += status.maxAwakening * equip;
        staminaRecovery += status.staminaRecovery * equip;
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
