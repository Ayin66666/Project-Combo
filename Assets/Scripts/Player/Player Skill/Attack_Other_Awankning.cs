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
    [SerializeField] private GameObject[] awakeningVFX;
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
        PlayerAction_Manager.instance.Armor_Setting(PlayerAction_Manager.instance.isAwankning ? value_Awakening[0].levelValue.value_List[skillLevel].armor : value_Normal[0].levelValue.value_List[skillLevel].armor);
        PlayerAction_Manager.instance.Special_Setting(true);
        Player_Manager.instance.action.isInvincibility = true;
        PlayerAction_Manager.instance.isAwankning = true;

        // 사용 가능 UI Off
        UI_Manager.instance.Awakening_Setting(false);

        // 사운드
        Player_Sound.instance.Sound_Skill(Player_Sound.Skill.Awakening);

        // 차징 애니메이션
        awakeningVFX[0].SetActive(true);
        anim.SetTrigger("Action");
        anim.SetBool("isAwakning", true);
        anim.SetBool("isAwakningCharge", true);


        // 차징 대기
        Effect_Manager.instance.Camera_Shack(5f, 0.5f);
        yield return new WaitForSeconds(0.5f);

        // 이펙트 On
        Effect_Manager.instance.Camera_Shack(30f, 0.1f);
        awakeningVFX[0].SetActive(false);
        awakeningVFX[1].SetActive(true);
        awakeningVFX[2].SetActive(true);
        swordVFX.SetActive(true);

        // 애니메이션 종료 대기
        anim.SetBool("isAwakningCharge", false);
        while (anim.GetBool("isAwakning"))
        {
            yield return null;
        }

        PlayerAction_Manager.instance.Armor_Setting(IDamageSysteam.ArmorType.None);
        PlayerAction_Manager.instance.MovementLock(cancelType, false);
        Player_Manager.instance.action.isInvincibility = false;

        // 능력치 강화
        Buff_Setting();
        Status_Setting(true);

        // 타이머
        float cur = 200;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / 15f;
            cur = Mathf.Lerp(200, 0, timer);
            Player_Manager.instance.status.curAwakening = cur;
            yield return null;
        }

        // 이펙트 Off
        awakeningVFX[2].SetActive(false);
        swordVFX.SetActive(false);

        // 능력치 초기화
        Status_Setting(false);
        PlayerAction_Manager.instance.Special_Setting(false);
        PlayerAction_Manager.instance.isAwankning = false;
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
            Player_Manager.instance.action.isAwankning = false;
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
        if (PlayerAction_Manager.instance.isAwankning)
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

        Player_Manager.instance.action.isAwankning = false;
        Player_Manager.instance.action.isInvincibility = false;

        // 이펙트 종료
        foreach (GameObject obj in awakeningVFX)
        {
            obj.SetActive(false);
        }
        swordVFX.SetActive(false);

        // 스테이터스 정상화 -> 어웨이크닝 상태일때만 1회 호출되도록 (아마 중복호출 이슈 있는듯?)
        if (Player_Manager.instance.action.isAwankning)
            Status_Setting(false);

        PlayerAction_Manager.instance.Armor_Setting(IDamageSysteam.ArmorType.None);
    }
}
