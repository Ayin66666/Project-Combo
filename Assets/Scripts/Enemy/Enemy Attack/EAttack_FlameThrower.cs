using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAttack_Swing : Attack_Base
{
    [Header("---Setting---")]
    [SerializeField] private GameObject attackVFX;
    [SerializeField] private Attack_Collider_AOE flameCollider;


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
        enemy.LookAt(Player_Manager.instance.gameObject, 0.1f);
        yield return new WaitForSeconds(0.1f);

        anim.SetTrigger("Action");
        anim.SetBool("isFlame", true);
        while (anim.GetBool("isFlame"))
        {
            yield return null;
        }

        enemy.Delay();
    }

    public override void AttackVFX(int index)
    {
        attackVFX.SetActive(index == 0 ? true : false);
    }

    public override void DamageCal(int index)
    {
        (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[index]);
        Skill_Value_SO.Value_Data skillData = value_Normal[index].levelValue.GetData(skillLevel);

        flameCollider.Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.multipleHit, isCritical, skillData.hitCount, damage, 0.25f);
    }

    public override void Attack_Reset()
    {
        // 동작 종료
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        // 리스트 리셋
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (value_Normal[i].attackCollider != null)
                value_Normal[i].attackCollider.ListReset();
        }

        // 이펙트 종료
        attackVFX.SetActive(false);
    }
}
