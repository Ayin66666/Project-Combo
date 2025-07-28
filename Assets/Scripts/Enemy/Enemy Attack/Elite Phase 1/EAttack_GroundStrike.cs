using System.Collections;
using System.ComponentModel;
using UnityEngine;


public class EAttack_GroundStrike : Attack_Base
{
    [Header("---Setting---")]
    [SerializeField] private Enemy_Elite_Phase1 elite;
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

        // 데미지 계산
        for (int i = 0; i < 2; i++)
        {
            DamageCal(i);
        }

        enemy.LookAt(enemy.target, 0.1f);

        // 애니메이션 호출
        anim.SetTrigger("Action");
        anim.SetBool("isGroundStrike", true);
        while (anim.GetBool("isGroundStrike"))
        {
            yield return null;
        }

        for (int i = 0; i < 2; i++)
        {
            value_Normal[i].attackCollider.ListReset();
        }

        enemy.Delay();
    }

    public override void AttackVFX(int index)
    {
        string key = (index == 0 ? Enemy_Elite_Phase1.SoundKey.GroundStrike1_1 : Enemy_Elite_Phase1.SoundKey.GroundStrike1_2).ToString();
        elite.sound.Sound(key);

        attackVFX[index].SetActive(true);
    }

    public override void DamageCal(int index)
    {
        (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[index]);
        Skill_Value_SO.Value_Data skillData = value_Normal[5].levelValue.GetData(skillLevel);
        value_Normal[index].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, AttackCollider_Controller.Owner.Enemy);
    }

    public override void Attack_Reset()
    {
        // 동작 종료
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        // 이펙트 종료
        foreach (GameObject obj in attackVFX)
        {
            obj.SetActive(false);
        }

        // 리스트 리셋
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
