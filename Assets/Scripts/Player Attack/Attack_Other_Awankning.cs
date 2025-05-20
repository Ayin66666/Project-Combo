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

        // 이펙트 On
        awankningVFX.SetActive(true);
        swordVFX.SetActive(true);

        // 능력치 강화
        Buff_Setting();
        Status_Setting(true);

        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isAwakning", true);
        while(anim.GetBool("isAwakning"))
        {
            yield return null;
        }
        Player_Manager.instance.MovementLock(cancelType, false);

        // 타이머
        while(Player_Manager.instance.curAwankning > 0)
        {
            Player_Manager.instance.curAwankning -= Time.deltaTime * 5f;
            yield return null;
        }

        // 능력치 초기화
        Status_Setting(false);
        Player_Manager.instance.Special_Setting(false);

        // 이펙트 Off
        awankningVFX.SetActive(false);
        swordVFX.SetActive(false);

        Player_Manager.instance.isAwakning = false;
        Player_Manager.instance.canAwakning = false;
    }

    private void Buff_Setting()
    {
        // 데미지 저장
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
        // 동작 종료
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        // 이펙트 종료
        awankningVFX.SetActive(false);
        swordVFX.SetActive(false);

        // 스테이터스 정상화
        Status_Setting(false);
    }
}
