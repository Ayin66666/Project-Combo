using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Normal_Second : Attack_Base
{
    [Header("---NormalSalsh Setting---")]
    [SerializeField] private GameObject attackVFX;
    [SerializeField] private Transform vfxPos;


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        Player_Manager.instance.MovementLock(cancelType, true);
        Player_Manager.instance.isAttack = true;
        Player_Manager.instance.LookAt();

        // 다음 공격 UI 호출
        UI_Manager.instance.AttackGuide(nextAttackData);

        // 데미지 계산
        DamageCal(0);

        if (anim != null)
        {
            // 공격 시작
            anim.SetTrigger("Action");
            anim.SetBool("isAttack", true);
            anim.SetBool("isCombo", true);
            while (anim.GetBool("isAttack"))
            {
                yield return null;
            }

            Player_Manager.instance.isAttack = false;

            // 리스트 리셋
            Attack_ColliderReset();

            // 스매쉬 대기? 이동 입력 대기?
            float timer = 0f;
            while (anim.GetBool("isCombo"))
            {
                timer += Time.deltaTime;
                if (timer > time && Input_Manager.instance.movementInput.magnitude > 0)
                {
                    anim.SetBool("isCombo", false);
                    break;
                }
                yield return null;
            }

            Player_Manager.instance.MovementLock(cancelType, false);
            Player_Manager.instance.AttackOver();
        }
    }


    public override void AttackVFX(int index)
    {
        Instantiate(attackVFX, vfxPos.position, vfxPos.rotation);
    }

    public override void DamageCal(int index)
    {
        Skill_Base.Value_Data skillData;
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


        // 리스트 리셋
        Attack_ColliderReset();
    }
}
