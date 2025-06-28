using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Attack_Other_Awankning : Attack_Base
{
    [Header("---Buff Status---")]
    [SerializeField] private List<BuffStatus> buffStatus;
    private int add_PhysicalDam;
    private int add_magcalDam;

    [System.Serializable]
    public struct BuffStatus
    {
        public float damage;
        public float attackSpeed;
        public float criticalChance;
        public float criticalMultiplier;
        public float moveSpeed;
    }


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
        PlayerAction_Manager.instance.MovementLock(cancelType, true);
        PlayerAction_Manager.instance.isAwakning = true;
        PlayerAction_Manager.instance.Special_Setting(true);

        // 사용 가능 UI Off
        UI_Manager.instance.Awakening_Setting(false);

        // 이펙트 On
        awankningVFX.SetActive(true);
        swordVFX.SetActive(true);

        // 능력치 강화
        Buff_Setting();
        Status_Setting(true);

        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isAwakning", true);
        while (anim.GetBool("isAwakning"))
        {
            yield return null;
        }
        PlayerAction_Manager.instance.MovementLock(cancelType, false);

        // 타이머
        while (Player_Manager.instance.status.curAwakening > 0)
        {
            Player_Manager.instance.status.curAwakening -= Time.deltaTime * 5f;
            yield return null;
        }

        // 능력치 초기화
        Status_Setting(false);
        PlayerAction_Manager.instance.Special_Setting(false);

        // 이펙트 Off
        awankningVFX.SetActive(false);
        swordVFX.SetActive(false);

        PlayerAction_Manager.instance.isAwakning = false;
        PlayerAction_Manager.instance.canAwakning = false;
    }

    private void Buff_Setting()
    {
        // 데미지 저장
        add_PhysicalDam = (int)(Player_Manager.instance.status.physicalDamage * buffStatus[skillLevel].damage);
        add_magcalDam = (int)(Player_Manager.instance.status.magicalDamage * buffStatus[skillLevel].damage);
    }

    private void Status_Setting(bool isOn)
    {
        if (isOn)
        {
            Player_Manager.instance.status.physicalDamage += add_PhysicalDam;
            Player_Manager.instance.status.magicalDamage += add_magcalDam;
            Player_Manager.instance.status.criticalhit += buffStatus[skillLevel].criticalChance;
            Player_Manager.instance.status.critical_multiplier += buffStatus[skillLevel].criticalMultiplier;
            Player_Manager.instance.status.moveSpeed += buffStatus[skillLevel].moveSpeed;
            Player_Manager.instance.status.curStamina = Player_Manager.instance.status.maxStamina;
        }
        else
        {
            Player_Manager.instance.status.physicalDamage -= add_PhysicalDam;
            Player_Manager.instance.status.magicalDamage -= add_magcalDam;
            Player_Manager.instance.status.criticalhit -= buffStatus[skillLevel].criticalChance;
            Player_Manager.instance.status.critical_multiplier -= buffStatus[skillLevel].criticalMultiplier;
            Player_Manager.instance.status.moveSpeed -= buffStatus[skillLevel].moveSpeed;
        }
    }

    public override void DamageCal(int index)
    {
        Skill_Value_SO.Value_Data skillData;
        if (PlayerAction_Manager.instance.isAwakning)
        {
            (bool isCritical, int damage) = PlayerAction_Manager.instance.DamageCalculation(value_Awakening[index], skillLevel);
            skillData = value_Awakening[index].levelValue.GetData(skillLevel);
            value_Awakening[index].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, AttackCollider_Controller.Owner.Player);
        }
        else
        {
            (bool isCritical, int damage) = PlayerAction_Manager.instance.DamageCalculation(value_Normal[index], skillLevel);
            skillData = value_Normal[index].levelValue.GetData(skillLevel);
            value_Normal[index].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, AttackCollider_Controller.Owner.Player);
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
