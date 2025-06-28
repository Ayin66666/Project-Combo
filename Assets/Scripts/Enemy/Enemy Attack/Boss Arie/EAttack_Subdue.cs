using System.Collections;
using UnityEngine;
using Easing.Tweening;


public class EAttack_Subdue : Attack_Base
{
    [Header("---Settting---")]
    [SerializeField] private GameObject[] attackVFX;
    [SerializeField] private Transform subduePos;
    [SerializeField] private bool isSubdue;
    [SerializeField] private Attack_Collider_Subdue subdueCollider;

    /*
     * 차징
     * 돌진 잡기 공격
     * 콜라이더 내 플레이어가 있을 시 - 무적 아닐 경우
     * 잡기 공격 동작
     * 실패 시
     * 360도 돌려베기
    */
    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        enemy.curState = Enemy_Base.State.Attack;
        isSubdue = false;

        // 데미지 셋팅
        for (int i = 1; i < value_Normal.Count; i++)
        {
            (bool isCrit, int dam) = enemy.DamageCalculation(value_Normal[i]);
            Skill_Value_SO.Value_Data skillData1 = value_Normal[i].levelValue.GetData(skillLevel);
            value_Normal[i].attackCollider.Damage_Setting(skillData1.type, skillData1.attackEffect, isCrit, skillData1.hitCount, dam, AttackCollider_Controller.Owner.Enemy);
        }

        // 차징 애니메이션
        anim.SetTrigger("Action");
        anim.SetFloat("AnimValue", 0);
        anim.SetBool("isSubdueCharge", true);
        anim.SetBool("isSubdue", true);

        // 차징
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 0.75f;
            anim.SetFloat("AnimValue", timer);
            enemy.LookAt(enemy.target, 0);
            yield return null;
        }
        anim.SetFloat("AnimValue", 1);

        // 돌진 딜레이
        yield return new WaitForSeconds(0.15f);


        // 잡기 콜라이더 활성화
        subdueCollider.gameObject.SetActive(true);
        subdueCollider.hitAction += Hit;
        (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[0]);
        Skill_Value_SO.Value_Data skillData = value_Normal[0].levelValue.GetData(skillLevel);
        subdueCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, 0.65f);

        // 돌진 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isSubdueCharge", false);

        // 돌진 잡기
        anim.SetFloat("AnimValue", 0);
        Vector3 startPos = enemy.transform.position;
        Vector3 endPos = moveDatas[0].movePos.position;
        timer = 0;
        while (timer < 1 && !isSubdue)
        {
            timer += Time.deltaTime * 1.5f;
            anim.SetFloat("AnimValue", timer);
            enemy.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetFloat("AnimValue", 1);
        subdueCollider.Collider_Reset();
        subdueCollider.gameObject.SetActive(false);

        // 성공 시
        if (isSubdue)
        {
            // 애니메이션
            anim.SetTrigger("Action");
            anim.SetBool("isSubdue", false);
            anim.SetBool("isSubdueAttack", true);

            // 플레이어 잡힘 애니메이션 호출
            PlayerAction_Manager.instance.Subdue(true, subduePos, this.transform);

            //애니메이션 대기
            while (anim.GetBool("isSubdueAttack"))
            {
                yield return null;
            }
            PlayerAction_Manager.instance.Subdue(false, null, null);
        }
        else
        {
            // 실패 시
            anim.SetTrigger("Action");
            while (anim.GetBool("isSubdue"))
            {
                yield return null;
            }
        }

        // 콜라이더 리스트 리셋
        for (int i = 1; i < value_Normal.Count; i++)
        {
            value_Normal[i].attackCollider.ListReset();
        }

        enemy.isPatten = false;
    }

    private void Hit()
    {
        isSubdue = true;
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

        // 카메라 종료
        PlayerAction_Manager.instance.Subdue(false, null, null);

        // 이펙트 종료
        foreach (GameObject obj in attackVFX)
        {
            obj.SetActive(false);
        }

        // 잡기 콜라이더 종료
        isSubdue = false;
        subdueCollider.Collider_Reset();
        subdueCollider.gameObject.SetActive(false);

        // 리스트 리셋
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
