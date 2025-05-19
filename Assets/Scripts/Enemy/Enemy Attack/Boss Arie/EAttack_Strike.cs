using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing.Tweening;
using MagicaCloth2;


public class EAttack_Strike : Attack_Base
{
    [Header("---Setting---")]
    [SerializeField] private GameObject[] attackVFX;
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private GameObject chargeVFX;
    [SerializeField] private GameObject teleportVFX;
    [SerializeField] private GameObject[] rushMovePos;

    [SerializeField] private Transform[] explosionPos_First;
    [SerializeField] private Transform[] explosionPos_Second;
    private Coroutine explosionCoroutine;
    Vector3 targetPos = Vector3.zero;


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        enemy.curState = Enemy_Base.State.Attack;

        // 데미지 셋팅
        for (int i = 0; i < 3; i++)
        {
            (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[i]);
            Skill_Base.Value_Data skillData = value_Normal[i].levelValue.GetData(skillLevel);
            value_Normal[i].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage); ;
        }

        // 차징
        anim.SetTrigger("Action");
        anim.SetFloat("AnimValue", 0);
        anim.SetBool("isStrikeCharge", true);
        anim.SetBool("isStrikeAttack", true);
        anim.SetBool("isStrike", true);

        chargeVFX.SetActive(true);
        targetPos = Vector3.zero;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            enemy.LookAt(enemy.target, 0);
            targetPos = enemy.target.transform.position;
            yield return null;
        }
        anim.SetBool("isStrikeCharge", false);
        chargeVFX.SetActive(false);

        Vector3 dirToTarget = (enemy.target.transform.position - enemy.transform.position).normalized;
        float offsetDistance = 1.5f; // 플레이어로부터 1.5유닛 앞에 착지
        targetPos = enemy.target.transform.position - dirToTarget * offsetDistance;


        // 1. 내려찍기 1 - (내려찍기 폭발1)
        Vector3 startPos;
        Vector3 endPos;
        while(anim.GetBool("isStrikeAttack"))
        {
            yield return null;
        }
        Explosion(0);

        // 딜레이
        yield return new WaitForSeconds(0.5f);


        // 2. 돌진 공격 x 3 
        for (int i = 0; i < 3; i++)
        {
            // 텔레포트
            Instantiate(teleportVFX, enemy.transform.position, Quaternion.identity);
            rushMovePos[0].transform.position = enemy.target.transform.position;
            enemy.transform.position = rushMovePos[i + 1].transform.position;
            Instantiate(teleportVFX, enemy.transform.position, Quaternion.identity);
            Debug.Log("Call Rush");

            // 주시 딜레이
            timer = 0;
            while (timer < 0.15f)
            {
                enemy.LookAt(enemy.target, 0);
                timer += Time.deltaTime;
                yield return null;
            }

            // 돌진
            anim.SetTrigger("Action");
            anim.SetFloat("AnimValue", 0);

            // 이동
            startPos = enemy.transform.position;
            endPos = moveDatas[0].movePos.position;
            timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime * 1.5f;
                anim.SetFloat("AnimValue", timer);
                enemy.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
                yield return null;
            }
            anim.SetFloat("AnimValue", 1);

            // 딜레이
            if(i < 2)
            {
                // 주시 딜레이
                timer = 0;
                while (timer < 0.05f)
                {
                    enemy.LookAt(enemy.target, 0);
                    timer += Time.deltaTime;
                    yield return null;
                }
            }
        }

        // 3. 내려찍기 2 - (내려찍기 폭발2)
        enemy.LookAt(enemy.target, 0);
        anim.SetTrigger("Action");
        anim.SetFloat("AnimValue", 0);

        dirToTarget = (enemy.target.transform.position - enemy.transform.position).normalized;
        offsetDistance = 1.5f; // 플레이어로부터 1.5유닛 앞에 착지
        targetPos = enemy.target.transform.position - dirToTarget * offsetDistance;
        startPos = enemy.transform.position;
        endPos = targetPos;
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            anim.SetFloat("AnimValue", timer);
            enemy.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        while(anim.GetBool("isStrike"))
        {
            yield return null;
        }

        // 리스트 초기화
        for (int i = 0; i < 3; i++)
        {
            value_Normal[i].attackCollider.ListReset();
        }

        enemy.isPatten = false;
    }

    public void StrikeMovement()
    {
        StartCoroutine(StrikeMovementCall());
    }

    private IEnumerator StrikeMovementCall()
    {
        Vector3 startPos = enemy.transform.position;
        Vector3 endPos = targetPos;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 2f;
            anim.SetFloat("AnimValue", timer);
            enemy.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetFloat("AnimValue", 1);
        anim.SetBool("isStrikeAttack", false);
    }



    public void Explosion(int index)
    {   
        explosionCoroutine = StartCoroutine(index == 0 ? Explosion_First() : Explosion_Second());
    }

    private IEnumerator Explosion_First() // -> 정면 폭발
    {
        // 포지션 저장
        List<Vector3> explosionPos = new List<Vector3>();
        for (int i = 0; i < explosionPos_First.Length; i++)
        {
            explosionPos.Add(explosionPos_First[i].position);
        }

        // 폭발
        int a = 0;
        for (int i = 0; i < 5; i++)
        {
            Debug.Log("Call Ex 1");
            // 이펙트 소환
            for (int j = a; j < a + 4; j++)
            {
                // 이펙트 소환
                GameObject obj = Instantiate(explosionVFX, explosionPos[j], Quaternion.identity);
                Attack_Collider_AOE ex = obj.GetComponent<Attack_Collider_AOE>();

                // 데미지 셋팅
                (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[3]);
                Skill_Base.Value_Data skillData = value_Normal[3].levelValue.GetData(skillLevel);
                ex.Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCritical, skillData.hitCount, damage, 0.05f);
            }
            a += 4;

            // 딜레이
            yield return new WaitForSeconds(0.15f);
        }
    }

    private IEnumerator Explosion_Second() // 원형 폭발
    {
        // 포지션 저장
        List<Vector3> explosionPos = new List<Vector3>();
        for (int i = 0; i < explosionPos_Second.Length; i++)
        {
            explosionPos.Add(explosionPos_Second[i].position);
        }

        // 폭발
        int a = 0;
        for (int i = 0; i < 3; i++)
        {
            // 이펙트 소환
            for (int j = a; j < a + 8; j++)
            {
                // 이펙트 소환
                GameObject obj = Instantiate(explosionVFX, explosionPos[j], Quaternion.identity);
                Attack_Collider_AOE ex = obj.GetComponent<Attack_Collider_AOE>();

                // 데미지 셋팅
                (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[4]);
                Skill_Base.Value_Data skillData = value_Normal[4].levelValue.GetData(skillLevel);
                ex.Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCritical, skillData.hitCount, damage, 0.05f);
            }
            a += 8;

            // 딜레이
            yield return new WaitForSeconds(0.15f);
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

        // 추가 공격 종료
        if (explosionCoroutine != null)
            StopCoroutine(explosionCoroutine);

        // 이펙트 종료
        chargeVFX.SetActive(false);
        foreach (var item in attackVFX)
        {
            item.SetActive(false);
        }

        // 리스트 리셋
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
