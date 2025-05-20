using System.Collections;
using UnityEngine;

public class EAttack_Flame : Attack_Base
{
    [Header("---Setting---")]
    [SerializeField] private GameObject chargeVFX;
    [SerializeField] private GameObject attackVFX;
    [SerializeField] private Attack_Collider_AOE attackCollider;


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        enemy.curState = Enemy_Base.State.Attack;

        DamageCal(0);

        // 차징
        anim.SetTrigger("Action");
        anim.SetBool("isFlameCharge", true);
        anim.SetBool("isFlame", true);
        float timer = 0;
        while(timer < 1.5f)
        {
            enemy.LookAt(enemy.target, 0f);
            timer += Time.deltaTime;
            yield return null;
        }

        // 공격 딜레이
        yield return new WaitForSeconds(0.1f);

        // 공격
        anim.SetBool("isFlameCharge", false);
        while(anim.GetBool("isFlame"))
        {
            yield return null;
        }

        enemy.Delay();
    }

    public void ChargeVFX(int index)
    {
        chargeVFX.SetActive(index == 0);
    }

    public override void AttackVFX(int index)
    {
        (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[0]);
        Skill_Value_SO.Value_Data skillData = value_Normal[0].levelValue.GetData(skillLevel);
        attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.multipleHit, isCritical, skillData.hitCount, damage, 0.25f);
        attackVFX.SetActive(index == 0 ? true : false);
    }

    public override void DamageCal(int index)
    {
        // 미사용
    }

    public override void Attack_Reset()
    {
        // 동작 종료
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        attackVFX.SetActive(false);

        // 리스트 리셋
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
