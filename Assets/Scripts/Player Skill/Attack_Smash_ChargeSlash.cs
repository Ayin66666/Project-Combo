using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Smash_ChargeSlash : Attack_Base
{
    [Header("---ChargeSlash Setting---")]
    public float chargeCount;
    [SerializeField] private GameObject[] chargeVFX;
    [SerializeField] private GameObject[] slashVFX;
    [SerializeField] private GameObject[] slashExplosionVFX;


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    /// <summary>
    /// 차지 후 정면 베기 공격 / 각성 상태에선 이펙트 & 범위 강화 & 차징 속도 1.5배
    /// </summary>
    /// <returns></returns>
    private IEnumerator UseCall()
    {
        PlayerAction_Manager.instance.MovementLock(cancelType, true);
        PlayerAction_Manager.instance.isAttack = true;

        anim.SetTrigger("Smash");
        anim.SetBool("isAttack", true);
        anim.SetBool("isSmash", true);
        anim.SetBool("isCharge", true);

        // 0단계
        chargeVFX[0].SetActive(true);

        // 차징
        float timer = 0;
        chargeCount = 0;
        while (Input_Manager.instance.inputDatas[1].isInput && timer < 3)
        {
            PlayerAction_Manager.instance.ChargeLookAt();
            timer += Time.deltaTime;
            // 차징
            if(chargeCount < 1)
            {
                chargeCount += Time.deltaTime * (PlayerAction_Manager.instance.isAwakning ? 1f : 0.5f);
            }

            // 2단계
            if (chargeCount >= 0.5f && chargeVFX[0].activeSelf)
            {
                chargeVFX[0].SetActive(false);
                chargeVFX[1].SetActive(true);
            }

            // 3단계
            if(chargeCount >= 1f && chargeVFX[1].activeSelf)
            {
                chargeVFX[1].SetActive(false);
                chargeVFX[2].SetActive(true);
            }

            yield return null;
        }
        anim.SetBool("isCharge", false);


        // 차지 이펙트 Off
        for (int i = 0; i < chargeVFX.Length; i++)
        {
            chargeVFX[i].SetActive(false);
        }

        // 공격 대기
        while (anim.GetBool("isAttack"))
        {
            yield return null;
        }

        PlayerAction_Manager.instance.isAttack = false;
        PlayerAction_Manager.instance.RushSlash_Setting(true);

        timer = 0;
        while(anim.GetBool("isSmash"))
        {
            timer += Time.deltaTime;
            if(timer > time && Input_Manager.instance.movementInput.magnitude > 0)
            {
                anim.SetBool("isSmash", false);
                break;
            }
            yield return null;
        }

        PlayerAction_Manager.instance.MovementLock(cancelType, false);
        PlayerAction_Manager.instance.RushSlash_Setting(false);
        PlayerAction_Manager.instance.AttackOver();
    }


    public List<Value> GetValues()
    {
        return value_Normal;
    }

    public override void AttackVFX(int index)
    {
        int a = chargeCount < 0.5f ? 0 : (chargeCount < 1f ? 1 : 2);
        if (index == 0)
        {
            slashVFX[a].SetActive(true);
        }
        else
        {
            slashExplosionVFX[a].SetActive(true);
        }
    }

    public override void DamageCal(int index)
    {
        Skill_Value_SO.Value_Data skillData;
        if (PlayerAction_Manager.instance.isAwakning)
        {
            (bool isCritical, int damage) = PlayerAction_Manager.instance.DamageCalculation(value_Awakening[index], skillLevel);
            skillData = value_Awakening[index].levelValue.GetData(skillLevel);

            if (value_Awakening[index].attackCollider != null)
                value_Awakening[index].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, AttackCollider_Controller.Owner.Player);
        }
        else
        {
            (bool isCritical, int damage) = PlayerAction_Manager.instance.DamageCalculation(value_Normal[index], skillLevel);
            skillData = value_Normal[index].levelValue.GetData(skillLevel);

            if (value_Awakening[index].attackCollider != null)
                value_Normal[index].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, AttackCollider_Controller.Owner.Player);
        }
    }

    public override void Attack_Reset()
    {
        // 동작 종료
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        chargeCount = 0;

        // 이펙트 종료
        foreach (GameObject vfx in chargeVFX)
        {
            vfx.SetActive(false);
        }
        foreach (GameObject vfx in slashVFX)
        {
            vfx.SetActive(false);
        }
        foreach (GameObject vfx in slashExplosionVFX)
        {
            vfx.SetActive(false);
        }

        // 리스트 리셋
        Attack_ColliderReset();
    }
}
