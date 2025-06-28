using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EAttack_ComboSlash : Attack_Base
{
    [Header("---Setting---")]
    [SerializeField] private GameObject[] attackVFX;


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        enemy.curState = Enemy_Base.State.Attack;
        enemy.LookAt(enemy.target, 0);

        // 무기 이펙트
        ((Enemy_Boss_Arie)enemy).Weapon_Setting(true);

        // 데미지 셋팅
        for (int i = 0; i < value_Normal.Count; i++)
        {
            (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[i]);
            Skill_Value_SO.Value_Data skillData = value_Normal[i].levelValue.GetData(skillLevel);
            value_Normal[i].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, AttackCollider_Controller.Owner.Enemy);
        }

        // 애니메이션 호출
        anim.SetTrigger("Action");
        anim.SetBool("isComboSlash", true);
        while (anim.GetBool("isComboSlash"))
        {
            yield return null;
        }

        // 리스트 초기화
        for (int i = 0; i < value_Normal.Count; i++)
        {
            value_Normal[i].attackCollider.ListReset();
        }

        enemy.isPatten = false;
    }

    public void Movement(int index)
    {
        enemy.Attack_Movement(moveDatas[index].movePos, moveDatas[index].moveSpeed);
    }

    public override void AttackVFX(int index)
    {
        attackVFX[index].SetActive(true);
    }

    public override void DamageCal(int index)
    {
        throw new System.NotImplementedException();
    }

    public override void Attack_Reset()
    {
        // 동작 종료
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        // 이펙트 리셋
        ((Enemy_Boss_Arie)enemy).Weapon_Setting(false);
        for (int i = 0; i < attackVFX.Length; i++)
        {
            attackVFX[i].SetActive(false);
        }

        // 리스트 리셋
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
