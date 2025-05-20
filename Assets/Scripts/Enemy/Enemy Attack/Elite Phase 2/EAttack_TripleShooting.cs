using Easing.Tweening;
using System.Collections;
using UnityEngine;

public class EAttack_TripleShooting : Attack_Base
{
    [Header("---Setting---")]
    [SerializeField] private Transform stepPos;
    [SerializeField] private Transform shootPos;
    [SerializeField] private GameObject attackVFX;


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        enemy.curState = Enemy_Base.State.Attack;

        anim.SetTrigger("Action");
        anim.SetFloat("AnimValue", 0);
        anim.SetBool("isTripleMove", true);
        anim.SetBool("isTriple", true);

        // 이동
        Vector3 startPos = enemy.transform.position;
        Vector3 endPos = stepPos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.55f;
            anim.SetFloat("AnimValue", timer);
            enemy.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetFloat("AnimValue", 1);
        anim.SetBool("isTripleMove", false);


        // 공격
        enemy.LookAt(enemy.target, 0.1f);
        while (anim.GetBool("isTriple"))
        {
            yield return null;
        }

        // 콜라이더 리셋
        for (int i = 0; i < value_Normal.Count; i++)
        {
            value_Normal[i].attackCollider.ListReset();
        }

        enemy.isPatten = false;
    }

    public override void AttackVFX(int index)
    {
        // 공격이펙트
        GameObject obj = Instantiate(attackVFX, shootPos.position, Quaternion.identity);

        // 콜라이더
        (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[index]);
        Skill_Value_SO.Value_Data skillData = value_Normal[index].levelValue.GetData(skillLevel);
        value_Normal[index].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);
        value_Normal[index].attackCollider.AttackColliderOn(0);
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

        attackVFX.SetActive(false);

        // 리스트 리셋
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
