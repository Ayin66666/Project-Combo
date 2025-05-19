using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Samsh_DoubleSlash : Attack_Base
{
    [Header("---DoubleSlash Setting---")]
    [SerializeField] private GameObject[] doubleSlashVFX;
    [SerializeField] private GameObject groundVFX;
    [SerializeField] private Transform goundPos;

    [SerializeField] private GameObject slashAura;
    [SerializeField] private Transform auraPos;


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }


    /// <summary>
    /// 좌우베기 2(3)타 / 각성 상태에선 3타 찌르기 추가 & 설치형 추가타
    /// </summary>
    /// <returns></returns>
    private IEnumerator UseCall()
    {
        Player_Manager.instance.MovementLock(cancelType, true);
        Player_Manager.instance.isAttack = true;
        Player_Manager.instance.isSmash = true;
        Player_Manager.instance.LookAt();

        // 데미지 계산
        for (int i = 0; i < value_Normal.Count; i++)
        {
            DamageCal(i);
        }

        // 애니메이션
        anim.SetTrigger("Smash");
        anim.SetBool("isAttack", true);
        anim.SetBool("isSmash", true);
        while (anim.GetBool("isAttack"))
        {
            yield return null;
        }

        Player_Manager.instance.isAttack = false;
        Player_Manager.instance.RushSlash_Setting(true);

        // 추가타 대기
        float timer = 0;
        while (anim.GetBool("isSmash"))
        {
            timer += Time.deltaTime;

            // 추가타
            if (Player_Manager.instance.isAwakning && Input_Manager.instance.inputDatas[1].isInput)
            {
                Player_Manager.instance.RushSlash_Setting(false);
                AdditionalAttack();
                yield break;
            }

            // 이동
            if (timer > time && Input_Manager.instance.movementInput.magnitude > 0)
            {
                anim.SetBool("isSmash", false);
                break;
            }

            yield return null;
        }

        // 데미지 리셋
        Attack_ColliderReset();

        Player_Manager.instance.MovementLock(cancelType, false);
        Player_Manager.instance.RushSlash_Setting(false);
        Player_Manager.instance.AttackOver();
    }


    private void AdditionalAttack()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(AdditionalAttackCall());
    }

    private IEnumerator AdditionalAttackCall()
    {
        Player_Manager.instance.isAttack = true;
        Player_Manager.instance.MovementLock(cancelType, true);
        Player_Manager.instance.LookAt();

        // 데미지 계산
        for (int i = 0; i < value_Normal.Count; i++)
        {
            DamageCal(i);
        }


        // 콜라이더 무시
        Player_Manager.instance.Collider_Ignore(true);

        // 2타 공격
        anim.SetTrigger("Smash");
        anim.SetBool("isAttack", true);
        anim.SetBool("isAdditionalSmash", true);
        while (anim.GetBool("isAttack"))
        {
            yield return null;
        }

        Player_Manager.instance.isAttack = false;

        float timer = 0;
        while (anim.GetBool("isAdditionalSmash"))
        {
            timer += Time.deltaTime;

            if (timer > time && Input_Manager.instance.movementInput.magnitude > 0)
            {
                anim.SetBool("isAdditionalSmash", false);
                Player_Manager.instance.MovementLock(cancelType, false);
                break;
            }

            yield return null;
        }

        // 콜라이더 무시
        Player_Manager.instance.Collider_Ignore(false);

        // 데미지 리셋
        Attack_ColliderReset();
        Player_Manager.instance.AttackOver();
    }


    public override void AttackVFX(int index)
    {
        doubleSlashVFX[index].SetActive(true);
    }

    /// <summary>
    /// 1타 스메쉬 지상 이펙트
    /// </summary>
    public void Samsh2_GourndVFX()
    {
        Instantiate(groundVFX, goundPos.position, goundPos.transform.localRotation);
    }

    /// <summary>
    /// 2타 공격 후 설치형 추가타
    /// </summary>
    public void Smash2_Aura()
    {
        // 검기 잔상 소환
        GameObject obj = Instantiate(slashAura, auraPos.position, Quaternion.identity);
        Attack_Collider_AOE aoe = obj.GetComponent<Attack_Collider_AOE>();

        // 데미지 셋팅
        (bool isCritical, int damage) = Player_Manager.instance.DamageCalculation(value_Awakening[3], skillLevel);
        Skill_Base.Value_Data skillData = value_Normal[3].levelValue.GetData(skillLevel);
        aoe.Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.multipleHit, isCritical, skillData.hitCount, damage, 0.15f);
    }

    public override void DamageCal(int index)
    {
        Skill_Base.Value_Data skillData;
        if (Player_Manager.instance.isAwakning)
        {
            (bool isCritical, int damage) = Player_Manager.instance.DamageCalculation(value_Awakening[index], skillLevel);
            skillData = value_Awakening[index].levelValue.GetData(skillLevel);

            if(value_Awakening[index].attackCollider != null)
                value_Awakening[index].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);
        }
        else
        {
            (bool isCritical, int damage) = Player_Manager.instance.DamageCalculation(value_Normal[index], skillLevel);
            skillData = value_Normal[index].levelValue.GetData(skillLevel);

            if (value_Awakening[index].attackCollider != null)
                value_Normal[index].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);
        }
    }

    public override void Attack_Reset()
    {
        // 동작 종료
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        // 이펙트 종료
        foreach (GameObject vfx in doubleSlashVFX)
        {
            vfx.SetActive(false);
        }

        // 리스트 리셋
        Attack_ColliderReset();
    }
}
