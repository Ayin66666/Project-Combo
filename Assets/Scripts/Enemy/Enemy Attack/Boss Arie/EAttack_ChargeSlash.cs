using System.Collections;
using UnityEngine;


public class EAttack_ChargeSlash : Attack_Base
{
    [Header("---Setting---")]
    [SerializeField] private GameObject[] attackVFX;
    [SerializeField] private GameObject chargeVFX;
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private Transform[] explosionPos1;
    [SerializeField] private Transform[] explosionPos2;

    [SerializeField] private Transform shootPos;
    [SerializeField] private GameObject shootVFX;
    private Coroutine explosionCoroutine;
    private Coroutine chargeCoroutine;


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    // 차징
    // 1타 - 올려베기
    // 차징
    // 2타 - 전방 강 찌르기
    // 차징
    // 3타 - 360 회전 베기

    private IEnumerator UseCall()
    {
        enemy.curState = Enemy_Base.State.Attack;

        // 데미지 셋팅
        for (int i = 0; i < 3; i++)
        {
            // 데미지 셋팅
            (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[i]);
            Value value = value_Normal[i];
            Skill_Value_SO.Value_Data skillData = value.levelValue.GetData(skillLevel);
            value_Normal[i].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, AttackCollider_Controller.Owner.Enemy);
        }

        int attackIndex = 0;
        for (int i = 0; i < 3; i++)
        {
            // 차징
            anim.SetTrigger("Action");
            anim.SetBool("isChargeSlashCarge", true);
            anim.SetBool("isChargeSlash", true);
            yield return chargeCoroutine = StartCoroutine(Charge(1f));

            // 딜레이
            yield return new WaitForSeconds(0.05f);

            // 공격
            anim.SetTrigger("Action");
            anim.SetBool("isChargeSlashCarge", false);
            anim.SetInteger("AttackCount", attackIndex);
            while (anim.GetBool("isChargeSlash"))
            {
                yield return null;
            }
            attackIndex++;

            // 딜레이
            yield return new WaitForSeconds(0.125f);
        }

        enemy.isPatten = false;
    }

    private IEnumerator Charge(float chargeTime)
    {
        chargeVFX.SetActive(true);

        float timer = 0;
        while (timer < chargeTime)
        {
            enemy.LookAt(enemy.target, 0);
            timer += Time.deltaTime;
            yield return null;
        }

        chargeVFX.SetActive(false);
    }


    public void Shoot()
    {
        // 검기 소환
        GameObject obj = Instantiate(shootVFX, shootPos.position, Quaternion.identity);
        Attack_Collider_AOE shoot = obj.GetComponent<Attack_Collider_AOE>();

        // 바라보기
        Vector3 lookDir = shootPos.transform.forward;
        Quaternion lookRotation = Quaternion.LookRotation(lookDir.normalized);

        // x축 기준으로 90도 회전 추가
        Quaternion xAxisRotation = Quaternion.Euler(90f, 0f, 0f);

        // 두 회전 결합
        obj.transform.rotation = lookRotation * xAxisRotation;

        // 데미지 셋팅
        (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[4]);
        Value value = value_Normal[4];
        Skill_Value_SO.Value_Data skillData = value.levelValue.GetData(skillLevel);
        shoot.Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCritical, skillData.hitCount, damage, 0.05f);
    }


    public void Explosion(int index)
    {
        explosionCoroutine = StartCoroutine(index == 0 ? Explosion_Upper() : Explosion_360());
    }

    private IEnumerator Explosion_Upper()
    {
        for (int i = 0; i < explosionPos1.Length; i++)
        {
            // 이펙트 소환
            GameObject obj = Instantiate(explosionVFX, explosionPos1[i].position, Quaternion.identity);

            // 데미지 셋팅
            (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[3]);
            Skill_Value_SO.Value_Data skillData = value_Normal[3].levelValue.GetData(skillLevel);
            obj.GetComponent<Attack_Collider_AOE>().Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCritical, skillData.hitCount, damage, 0.05f);

            yield return new WaitForSeconds(0.125f);
        }
    }

    private IEnumerator Explosion_360()
    {
        for (int i = 0; i < explosionPos2.Length; i++)
        {
            // 이펙트 소환
            GameObject obj = Instantiate(explosionVFX, explosionPos2[i].position, Quaternion.identity);

            // 데미지 셋팅
            (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[4]);
            Skill_Value_SO.Value_Data skillData = value_Normal[4].levelValue.GetData(skillLevel);
            obj.GetComponent<Attack_Collider_AOE>().Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCritical, skillData.hitCount, damage, 0.05f);
        }

        yield return null;
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

        if (explosionCoroutine != null)
            StopCoroutine(explosionCoroutine);

        if (chargeCoroutine != null)
            StopCoroutine(chargeCoroutine);

        // 이펙트 종료
        chargeVFX.SetActive(false);
        foreach (GameObject vfx in attackVFX)
        {
            vfx.SetActive(false);
        }

        // 리스트 리셋
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
