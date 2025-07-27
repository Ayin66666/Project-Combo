using System;
using System.Collections;
using UnityEngine;

public class Attack_Additional_RushSlash : Attack_Base
{
    [Header("---RushSlash Setting---")]
    [SerializeField] private GameObject[] attackVFX;
    [SerializeField] private GameObject rushSlashCollider;


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        PlayerAction_Manager.instance.MovementLock(cancelType, true);
        PlayerAction_Manager.instance.Animation_Reset();
        PlayerAction_Manager.instance.isAttack = true;

        // 데미지 계산
        for (int i = 0; i < value_Normal.Count; i++)
        {
            DamageCal(i);
        }

        // 콜라이더 무시
        PlayerAction_Manager.instance.Collider_Ignore(true);

        // 돌진 공격
        PlayerAction_Manager.instance.LookAt();
        RushCollider(true);
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isAdditonalRush", true);
        while (anim.GetBool("isAttack"))
        {
            yield return null;
        }

        PlayerAction_Manager.instance.isAttack = false;

        // 이동 대기
        float timer = 0;
        while(anim.GetBool("isAdditonalRush"))
        {
            timer += Time.deltaTime;
            if(timer > time && Input_Manager.instance.movementInput.magnitude > 0)
            {
                anim.SetBool("isAdditonalRush", false);
                break;
            }
            yield return null;
        }

        // 콜라이더 무시
        PlayerAction_Manager.instance.Collider_Ignore(false);
        RushCollider(false);

        // 공격 콜라이더 리셋
        Attack_ColliderReset();

        PlayerAction_Manager.instance.MovementLock(cancelType, false);
        PlayerAction_Manager.instance.AttackOver();
    }

    private void RushCollider(bool isOn)
    {
        Attack_Collider_AOE aoe = rushSlashCollider.GetComponent<Attack_Collider_AOE>();

        (bool isCritical, int damage) = PlayerAction_Manager.instance.DamageCalculation(value_Normal[0], skillLevel);
        Skill_Value_SO.Value_Data skillData = value_Normal[0].levelValue.GetData(skillLevel);
        aoe.Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.multipleHit, isCritical, skillData.hitCount, damage, 5f);
        
        rushSlashCollider.SetActive(isOn);
    }

    public override void AttackVFX(int index)
    {
        // 사운드
        if (index == 0)
            Player_Sound.instance.Sound_Skill(Player_Sound.Skill.RushSlash);

        attackVFX[index].SetActive(true);
    }

    public override void DamageCal(int index)
    {
        Skill_Value_SO.Value_Data skillData;
        if (PlayerAction_Manager.instance.isAwakning)
        {
            (bool isCritical, int damage) = PlayerAction_Manager.instance.DamageCalculation(value_Awakening[0], skillLevel);
            skillData = value_Awakening[0].levelValue.GetData(skillLevel);

            if (value_Awakening[0].attackCollider != null)
                value_Awakening[0].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, AttackCollider_Controller.Owner.Player);
        }
        else
        {
            (bool isCritical, int damage) = PlayerAction_Manager.instance.DamageCalculation(value_Normal[0], skillLevel);
            skillData = value_Normal[0].levelValue.GetData(skillLevel);

            if (value_Awakening[0].attackCollider != null)
                value_Normal[0].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, AttackCollider_Controller.Owner.Player);
        }
    }

    public override void Attack_Reset()
    {
        // 동작 종료
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        // 이펙트 종료
        foreach(GameObject vfx in attackVFX)
        {
            vfx.SetActive(false);
        }

        // 리스트 리셋
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
