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

        // ��� ���� UI Off
        UI_Manager.instance.Awakening_Setting(false);

        // ����Ʈ On
        awankningVFX.SetActive(true);
        swordVFX.SetActive(true);

        // �ɷ�ġ ��ȭ
        Buff_Setting();
        Status_Setting(true);

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isAwakning", true);
        while (anim.GetBool("isAwakning"))
        {
            yield return null;
        }
        PlayerAction_Manager.instance.MovementLock(cancelType, false);

        // Ÿ�̸�
        while (Player_Manager.instance.status.curAwakening > 0)
        {
            Player_Manager.instance.status.curAwakening -= Time.deltaTime * 5f;
            yield return null;
        }

        // �ɷ�ġ �ʱ�ȭ
        Status_Setting(false);
        PlayerAction_Manager.instance.Special_Setting(false);

        // ����Ʈ Off
        awankningVFX.SetActive(false);
        swordVFX.SetActive(false);

        PlayerAction_Manager.instance.isAwakning = false;
        PlayerAction_Manager.instance.canAwakning = false;
    }

    private void Buff_Setting()
    {
        // ������ ����
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
