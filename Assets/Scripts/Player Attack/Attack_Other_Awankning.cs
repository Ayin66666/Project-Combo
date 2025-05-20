using System.Collections;
using UnityEngine;

public class Attack_Other_Awankning : Attack_Base
{
    [Header("---Buff Status---")]
    [SerializeField] private float damage;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float criticalChance;
    [SerializeField] private float criticalMultiplier;
    [SerializeField] private float moveSpeed;

    private int add_PhysicalDam;
    private int add_magcalDam;


    [Header("--- Awankning VFX ---")]
    [SerializeField] private GameObject awankningVFX;
    [SerializeField] private GameObject swordVFX;


    public override void AttackVFX(int index)
    {
        throw new System.NotImplementedException();
    }

    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        Player_Manager.instance.MovementLock(cancelType, true);
        Player_Manager.instance.isAwakning = true;
        Player_Manager.instance.Special_Setting(true);

        // ����Ʈ On
        awankningVFX.SetActive(true);
        swordVFX.SetActive(true);

        // �ɷ�ġ ��ȭ
        Buff_Setting();
        Status_Setting(true);

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isAwakning", true);
        while(anim.GetBool("isAwakning"))
        {
            yield return null;
        }
        Player_Manager.instance.MovementLock(cancelType, false);

        // Ÿ�̸�
        while(Player_Manager.instance.curAwankning > 0)
        {
            Player_Manager.instance.curAwankning -= Time.deltaTime * 5f;
            yield return null;
        }

        // �ɷ�ġ �ʱ�ȭ
        Status_Setting(false);
        Player_Manager.instance.Special_Setting(false);

        // ����Ʈ Off
        awankningVFX.SetActive(false);
        swordVFX.SetActive(false);

        Player_Manager.instance.isAwakning = false;
        Player_Manager.instance.canAwakning = false;
    }

    private void Buff_Setting()
    {
        // ������ ����
        add_PhysicalDam = (int)(Player_Manager.instance.physcialDamage * damage);
        add_magcalDam = (int)(Player_Manager.instance.magicalDamage * damage);
    }

    private void Status_Setting(bool isOn)
    {
        if (isOn)
        {
            Player_Manager.instance.physcialDamage += add_PhysicalDam;
            Player_Manager.instance.magicalDamage += add_magcalDam;
            Player_Manager.instance.criticalhit += criticalChance;
            Player_Manager.instance.critical_multiplier += criticalMultiplier;
            Player_Manager.instance.moveSpeed += moveSpeed;
            Player_Manager.instance.curSteamina = Player_Manager.instance.maxSteamina;
        }
        else
        {
            Player_Manager.instance.physcialDamage -= add_PhysicalDam;
            Player_Manager.instance.magicalDamage -= add_magcalDam;
            Player_Manager.instance.criticalhit -= criticalChance;
            Player_Manager.instance.critical_multiplier -= criticalMultiplier;
            Player_Manager.instance.moveSpeed -= moveSpeed;
        }
    }

    public override void DamageCal(int index)
    {
        Skill_Value_SO.Value_Data skillData;
        if (Player_Manager.instance.isAwakning)
        {
            (bool isCritical, int damage) = Player_Manager.instance.DamageCalculation(value_Awakening[index], skillLevel);
            skillData = value_Awakening[index].levelValue.GetData(skillLevel);
            value_Awakening[index].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);
        }
        else
        {
            (bool isCritical, int damage) = Player_Manager.instance.DamageCalculation(value_Normal[index], skillLevel);
            skillData = value_Normal[index].levelValue.GetData(skillLevel);
            value_Normal[index].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);
        }
    }

    public override void Attack_Reset()
    {
        // ���� ����
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        // ����Ʈ ����
        awankningVFX.SetActive(false);
        swordVFX.SetActive(false);

        // �������ͽ� ����ȭ
        Status_Setting(false);
    }
}
