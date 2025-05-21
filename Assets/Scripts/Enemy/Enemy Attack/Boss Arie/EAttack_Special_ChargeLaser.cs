using DG.Tweening;
using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


public class EAttack_Special_ChargeLaser : Attack_Base
{
    [Header("---VFX---")]
    [SerializeField] private GameObject chargeVFX;
    [SerializeField] private GameObject jumpVFX;
    [SerializeField] private GameObject fiyVFX;
    [SerializeField] private GameObject subLaserExplosion;
    [SerializeField] private GameObject chargeCoreVFX;
    [SerializeField] private GameObject[] laserVFX;
    [SerializeField] private GameObject swordAuraVFX;
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private GameObject[] attackVFX;


    [Header("---Pos---")]
    [SerializeField] private GameObject centerParent;
    [SerializeField] private Transform centerPos;
    [SerializeField] private Transform fiyVFXPos;
    [SerializeField] private Transform mainExplosionPos;
    [SerializeField] private Transform[] subExplosionPos;


    [Header("---Component---")]
    [SerializeField] private LayerMask ground;
    [SerializeField] private GameObject body;
    [SerializeField] private LineRenderer line;
    [SerializeField] private VideoClip clip;
    private bool isMove;


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
        anim.SetBool("isSpecialCharge", true);
        anim.SetBool("isSpecialJump", true);
        anim.SetBool("isSpecialFlying", true);
        anim.SetBool("isSpecialAttack", true);
        anim.SetBool("isSpecialLanding", true);
        anim.SetBool("isSpecial", true);


        // 날개 & 무기
        ((Enemy_Boss_Arie)enemy).Bosster_Setting(true);
        ((Enemy_Boss_Arie)enemy).Weapon_Setting(true);

        // 차징
        chargeVFX.SetActive(true);
        anim.SetFloat("AnimValue", 0);
        float timer = 0;

        while(timer < 3)
        {
            timer += Time.deltaTime;
            anim.SetFloat("AnimValue", timer < 1 ? EasingFunctions.OutExpo(timer) : 1);
            yield return null;
        }
        chargeVFX.SetActive(false);
        anim.SetBool("isSpecialCharge", false);


        // radius 방향 계산
        Vector3 radiusDir = (enemy.transform.position - centerPos.transform.position).normalized;

        // tangent 방향 계산
        Vector3 tangentDir = Quaternion.Euler(0, 45f, 0) * radiusDir;

        // 현재 enemy가 바라보는 방향
        Quaternion currentRot = enemy.transform.rotation;

        // 목표로 삼을 회전 방향
        Quaternion targetRot = Quaternion.LookRotation(tangentDir, Vector3.up);

        // 부드럽게 회전 (0.3~0.5초 정도 추천)
        Tween preRotateTween = enemy.transform.DORotateQuaternion(targetRot, 0.1f).SetEase(Ease.OutSine);

        // 점프 VFX
        Instantiate(jumpVFX, enemy.transform.position, Quaternion.identity);

        // 상승
        StartCoroutine(Movement(0));
        while (anim.GetBool("isSpecialJump") || isMove)
        {
            yield return null;
        }


        // 비행 시작 이펙트
        fiyVFX.SetActive(true);

        // 선회 중 상승
        StartCoroutine(Movement(1));

        // 선회
        centerPos.transform.parent = null;
        enemy.transform.parent = centerPos;
        var rotateTween = centerPos.transform.DOLocalRotate(centerPos.transform.localEulerAngles + new Vector3(0, 360, 0), 3f, RotateMode.FastBeyond360).SetEase(Ease.InOutSine);
        
        // 선회 중 방향 갱신
        rotateTween.OnUpdate(() => {
            Vector3 radiusDir = (enemy.transform.position - centerPos.transform.position).normalized;
            Vector3 tangentDir = Quaternion.Euler(0, 90f, 0) * radiusDir; // <- 여기가 핵심
            enemy.transform.rotation = Quaternion.LookRotation(tangentDir, Vector3.up);
        });

        // 레이저 & 검기
        StartCoroutine(SwordAura());
        StartCoroutine(Laser());

        // 선회 대기
        yield return new WaitForSeconds(3f);
        enemy.transform.parent = null;
        centerPos.transform.parent = centerParent.transform;

        // 선회 끝나면: 정면 고정
        rotateTween.OnComplete(() => {
            // 마지막 tangent 방향을 한번 더 계산
            Vector3 radiusDir = (enemy.transform.position - centerPos.transform.position).normalized;
            Vector3 tangentDir = Quaternion.Euler(0, 90f, 0) * radiusDir;

            // 자연스럽게 고정시키고 싶으면 DOQuaternion 써서 0.3초 정도 부드럽게 맞춰줌
            enemy.transform.DORotateQuaternion(Quaternion.LookRotation(tangentDir, Vector3.up), 0.3f)
                .SetEase(Ease.OutSine);
        });


        // 중앙 차징 컷씬
        enemy.enemyUI.CutScene(clip);
        while (enemy.enemyUI.isCutScene)
        {
            yield return null;
        }
        
        anim.SetBool("isSpecialCharge", true);
        anim.SetBool("isSpecialFlying", false);
        anim.SetFloat("AnimValue", 0);


        // 중앙 대형 레이저 차징
        Vector3 exDir = (mainExplosionPos.position - enemy.transform.position).normalized;
        RaycastHit hit;
        Vector3 pos = Vector3.zero;
        line.enabled = true;
        chargeVFX.SetActive(true);
        chargeCoreVFX.SetActive(true);
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 0.35f;

            // 애니메이션 셋팅
            anim.SetFloat("AnimValue", EasingFunctions.OutExpo(timer));

            // 타겟 추적
            enemy.LookAt(enemy.target, 0);

            // 레이 셋팅
            exDir = (enemy.target.transform.position - chargeCoreVFX.transform.position).normalized;
            Physics.Raycast(enemy.transform.position, exDir, out hit, 100, ground);
            pos = hit.point;

            // 폭발 위치 셋팅
            mainExplosionPos.transform.position = pos;

            // 라인 셋팅
            line.SetPosition(0, chargeCoreVFX.transform.position);
            line.SetPosition(1, enemy.target.transform.position);
            yield return null;
        }
        line.enabled = false;
        chargeVFX.SetActive(false);
        anim.SetFloat("AnimValue", 1);
        anim.SetBool("isSpecialCharge", false);


        // 중앙 대형 레이저 발사
        (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[3]);
        Skill_Value_SO.Value_Data skillData = value_Normal[3].levelValue.GetData(skillLevel);
        laserVFX[1].GetComponent<Attack_Collider_AOE>().Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.multipleHit, isCritical, skillData.hitCount, damage, 0.15f);

        // 방향 지정
        Vector3 laserLookDir = pos - laserVFX[1].transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(laserLookDir.normalized);
        laserVFX[1].transform.rotation = lookRotation;

        // 레이저 활성화
        laserVFX[1].SetActive(true);

        yield return new WaitForSeconds(1.25f);

        // 중앙 폭발 소환
        GameObject explosion = Instantiate(explosionVFX, mainExplosionPos.position, Quaternion.identity);
        Attack_Collider_AOE aoe = explosion.GetComponent<Attack_Collider_AOE>();

        // 중앙 폭발 데미지 셋팅
        (isCritical, damage) = enemy.DamageCalculation(value_Normal[4]);
        skillData = value_Normal[4].levelValue.GetData(skillLevel);
        aoe.Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCritical, skillData.hitCount, damage, 0.1f);

        // 폭발 대기
        yield return new WaitForSeconds(0.1f);

        // 추가 폭발
        StartCoroutine(SubExplosion());

        // 폭발 대기
        yield return new WaitForSeconds(1f);


        laserVFX[1].SetActive(false);
        chargeCoreVFX.SetActive(false);
        anim.SetBool("isSpecialAttack", false);
        

        // 착륙
        Physics.Raycast(enemy.transform.position, (moveDatas[2].movePos.position - enemy.transform.position).normalized, out hit, 100, ground);
        Vector3 startPos = enemy.transform.position;
        Vector3 endPos = hit.point;
        anim.SetFloat("AnimValue", 0);
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * moveDatas[2].moveSpeed;
            enemy.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            anim.SetFloat("AnimValue", timer);
            enemy.LookAt(enemy.target, 0);
            yield return null;
        }
        anim.SetFloat("AnimValue", 1);
        anim.SetBool("isSpecialLanding", false);

        // 착륙 딜레이 애니메이션
        while (anim.GetBool("isSpecial"))
        {
            yield return null;
        }

        enemy.isPatten = false;
    }

    private IEnumerator Laser()
    {
        for (int i = 0; i < 5; i++)
        {
            // 레이저 소환
            Vector3 spawnPos = new Vector3(enemy.transform.position.x, 7, enemy.transform.position.z);
            GameObject laser = Instantiate(laserVFX[0], spawnPos, Quaternion.identity);
            Attack_Collider_AOE aoe = laser.GetComponent<Attack_Collider_AOE>();

            // 바라보기
            Vector3 moveDir = enemy.target.transform.position - spawnPos;
            Quaternion lookRotation = Quaternion.LookRotation(moveDir.normalized);
            laser.transform.rotation = lookRotation;

            // 데미지 셋팅
            (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[0]);
            Skill_Value_SO.Value_Data skillData = value_Normal[1].levelValue.GetData(skillLevel);
            aoe.Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.multipleHit, isCritical, skillData.hitCount, damage, 0.15f);

            // 폭발 딜레이
            yield return new WaitForSeconds(0.1f);

            // 지점 폭발 소환
            RaycastHit hit;
            Physics.Raycast(spawnPos, moveDir, out hit, 100, ground);
            GameObject obj =  Instantiate(subLaserExplosion, hit.point, Quaternion.identity);
            aoe = obj.GetComponent<Attack_Collider_AOE>();

            // 지점 폭발 데미지 셋팅
            (isCritical, damage) = enemy.DamageCalculation(value_Normal[1]);
            skillData = value_Normal[1].levelValue.GetData(skillLevel);
            aoe.Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCritical, skillData.hitCount, damage, 0.05f);
            
            // 딜레이
            yield return new WaitForSeconds(0.4f);
        }
    }

    private IEnumerator SwordAura()
    {
        for (int i = 0; i < 15; i++)
        {
            // 검기 소환
            GameObject obj = Instantiate(swordAuraVFX, enemy.transform.position, Quaternion.identity);
            Attack_Collider_Shooting shoot = obj.GetComponent<Attack_Collider_Shooting>();

            // 데미지 셋팅
            (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[2]);
            Skill_Value_SO.Value_Data skillData = value_Normal[2].levelValue.GetData(skillLevel);
            shoot.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);

            // 이동 셋팅
            Vector3 moveDir = enemy.target.transform.position - enemy.transform.position;
            moveDir.y = moveDir.y += 0.5f;
            shoot.Movement_Setting(moveDir.normalized, 30f, 30f);

            // 바라보기
            Quaternion lookRotation = Quaternion.LookRotation(moveDir.normalized);
            float z = Random.Range(-25, 25f);
            Vector3 euler = lookRotation.eulerAngles;
            euler.z += z;
            obj.transform.rotation = Quaternion.Euler(euler);

            // 딜레이
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator SubExplosion()
    {
        // 포지션 저장
        List<Vector3> explosionPos = new List<Vector3>();
        for (int i = 0; i < subExplosionPos.Length; i++)
        {
            explosionPos.Add(subExplosionPos[i].position);
        }

        // 폭발
        int a = 0;
        for (int i = 0; i < 3; i++)
        {
            // 이펙트 소환
            for (int j = a; j < a + 8; j++)
            {
                // 이펙트 소환
                GameObject obj = Instantiate(subLaserExplosion, explosionPos[j], Quaternion.identity);
                Attack_Collider_AOE ex = obj.GetComponent<Attack_Collider_AOE>();

                // 데미지 셋팅
                (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[5]);
                Skill_Value_SO.Value_Data skillData = value_Normal[5].levelValue.GetData(skillLevel);
                ex.Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCritical, skillData.hitCount, damage, 0.05f);
            }
            a += 8;

            // 딜레이
            yield return new WaitForSeconds(0.2f);
        }
    }


    private IEnumerator Movement(int index)
    {
        isMove = true;

        Vector3 startPos = enemy.transform.position;
        Vector3 endPos = moveDatas[index].movePos.position;
        float newY;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * moveDatas[index].moveSpeed;
            newY = Mathf.Lerp(startPos.y, endPos.y, EasingFunctions.OutExpo(timer));
            enemy.transform.position = new Vector3(enemy.transform.position.x, newY, enemy.transform.position.z);
            yield return null;
        }
        enemy.transform.position = new Vector3(enemy.transform.position.x, endPos.y, enemy.transform.position.z);

        isMove = false;
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
        // 이때 무적 상태라 필요 없음!

        // 동작 종료
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        // 이펙트 종료
        ((Enemy_Boss_Arie)enemy).Weapon_Setting(false);
        ((Enemy_Boss_Arie)enemy).Bosster_Setting(false);
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
