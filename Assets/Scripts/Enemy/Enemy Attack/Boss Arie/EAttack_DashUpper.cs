using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class EAttack_DashUpper : Attack_Base
{
    [Header("---Setting---")]
    [SerializeField] private float chaneTime;
    [SerializeField] private GameObject[] attackVFX;
    [SerializeField] private GameObject chargeVFX;
    [SerializeField] private GameObject[] explosionVFX;
    [SerializeField] private Transform[] explosionPos_Upper;
    [SerializeField] private Transform[] explosionPos_Strike;
    [SerializeField] private NavMeshAgent nav;
    private Coroutine explosionCoroutine;


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        enemy.curState = Enemy_Base.State.Attack;

        for (int i = 0; i < 2; i++)
        {
            (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[i]);
            Skill_Value_SO.Value_Data skillData = value_Normal[i].levelValue.GetData(skillLevel);
            value_Normal[i].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);
        }

        // 차징
        anim.SetTrigger("Action");  
        anim.SetBool("isDashUpperCharge", true);
        anim.SetBool("isDashUpperMove", true);
        anim.SetBool("isDashUpper", true);
        chargeVFX.SetActive(true);
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            enemy.LookAt(enemy.target, 0);
            yield return null;
        }
        chargeVFX.SetActive(false);

        // 돌진
        anim.SetBool("isDashUpperCharge", false);
        enemy.Check_Target();
        timer = 0;
        nav.enabled = true;
        while (timer < chaneTime && enemy.targetRange > 5)
        {
            nav.SetDestination(enemy.target.transform.position);
            enemy.Check_Target();
            timer += Time.deltaTime;
            yield return null;
        }
        nav.enabled = false;

        // 올려베기 & 내려찍기
        enemy.LookAt(enemy.target, 0);
        anim.SetBool("isDashUpperMove", false);
        while(anim.GetBool("isDashUpper"))
        {
            yield return null;
        }

        for (int i = 0; i < 2; i++)
        {
            value_Normal[i].attackCollider.ListReset();
        }

        enemy.isPatten = false;
    }

    public void Look(float time)
    {
        StartCoroutine(LookCall(time));
    }

    private IEnumerator LookCall(float time)
    {
        float timer = 0;
        while(timer < time)
        {
            timer += Time.deltaTime;
            enemy.LookAt(enemy.target, 0);
            yield return null;
        }
    }


    public void Explosion(int index)
    {
        explosionCoroutine = StartCoroutine(index == 0 ? ExplosionCall_Upper() : ExplosionCall_Strike());
    }

    private IEnumerator ExplosionCall_Upper()
    {
        for (int i = 0; i < explosionPos_Upper.Length; i++)
        {
            // 폭발 소환
            GameObject obj = Instantiate(explosionVFX[0], explosionPos_Upper[i].position, Quaternion.identity);
            Attack_Collider_AOE aoe = obj.GetComponent<Attack_Collider_AOE>();

            // 데미지 셋팅
            (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[2]);
            Skill_Value_SO.Value_Data skillData = value_Normal[2].levelValue.GetData(skillLevel);
            aoe.Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCritical, skillData.hitCount, damage, 0.05f);

            // 딜레이
            yield return new WaitForSeconds(0.065f);
        }
    }

    private IEnumerator ExplosionCall_Strike()
    {
        // 폭발
        int a = 0;
        for (int i = 0; i < 5; i++)
        {
            for (int i1 = a; i1 < a + 4; i1++)
            {
                // 폭발 이펙트
                GameObject obj = Instantiate(explosionVFX[1], explosionPos_Strike[i1].transform.position, Quaternion.identity);

                // 데미지 셋팅
                (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[3]);
                Skill_Value_SO.Value_Data skillData = value_Normal[3].levelValue.GetData(skillLevel);
                obj.GetComponent<Attack_Collider_AOE>().Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCritical, skillData.hitCount, damage, 0.05f);
            }

            a += 4;

            // 폭발 딜레이
            yield return new WaitForSeconds(0.05f);
        }
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

        nav.enabled = false;

        // 이펙트 종료
        chargeVFX.SetActive(false);
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
