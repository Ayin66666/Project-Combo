using System.Collections;
using UnityEngine;

public class EAttack_Normal : Attack_Base
{
    [Header("--- Normal Setting ---")]
    [SerializeField] private GameObject attackVFX;


    public override void Use()
    {
        if(useCoroutine != null) 
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        enemy.curState = Enemy_Base.State.Attack;

        DamageCal(0);
        enemy.LookAt(PlayerAction_Manager.instance.gameObject, 0.1f);
        yield return new WaitForSeconds(0.1f);

        // 찌르기 공격
        anim.SetTrigger("Action");
        anim.SetBool("isNormalAttack", true);
        while(anim.GetBool("isNormalAttack"))
        {
            yield return null;
        }

        value_Normal[0].attackCollider.ListReset();
        enemy.Delay();
    }

    public override void AttackVFX(int index)
    {
        attackVFX.SetActive(true);
    }
    
    public override void DamageCal(int index)
    {
        (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[0]);
        Skill_Value_SO.Value_Data skillData = value_Normal[0].levelValue.GetData(skillLevel);

        value_Normal[0].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);
    }

    public override void Attack_Reset()
    {
        // 동작 종료
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        // 이펙트 리셋
        attackVFX.SetActive(false);

        // 리스트 리셋
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
