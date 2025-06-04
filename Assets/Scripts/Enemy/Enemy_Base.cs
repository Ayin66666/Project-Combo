using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Easing.Tweening;


public abstract class Enemy_Base : MonoBehaviour, IDamageSysteam
{
    [Header("---State---")]
    [SerializeField] private EnemyType enemyType;
    [SerializeField] protected SpawnType spawnType;
    [SerializeField] public State curState;
    [SerializeField] private IDamageSysteam.HitVFX curHit;
    [SerializeField] protected bool canAction;
    [SerializeField] protected bool isAttack;
    public bool isPatten;
    public bool isGroggy;
    private enum EnemyType { Normal, Elite, Boss }
    protected enum SpawnType { None, Movement, CutScene }
    public enum State { None, Spawn, Idle, Think, Chase, Attack, Delay, Groggy, Die }


    [Header("---Status---")]
    public Character_Status_SO statusData;
    public string enemyName;

    // Defence Status
    public int curHp;
    public int maxHp;
    public float curGroggy;
    public float maxGroggy;
    public int physicalDefence;
    public int magicalDefence;

    // Attack Status
    public int physcialDamage;
    public int magicalDamage;
    public float attackSpeed;
    public float criticalhit;
    public float critical_multiplier;

    // Other Status
    public float moveSpeed;
    public float groggyTime;


    [Header("---Chase---")]
    [SerializeField] protected float attackRange;
    public GameObject target;
    [SerializeField] public Vector3 targetDir;
    [SerializeField] public float targetRange;


    [Header("---Attack---")]
    public List<Attack_Base> attackDatas;


    [Header("---Animation---")]
    [SerializeField] private string[] animTrigger;
    [SerializeField] private string[] animBool;


    [Header("---Component---")]
    public Enemy_UI enemyUI;
    [SerializeField] protected CharacterController controller;
    [SerializeField] protected NavMeshAgent nav;
    [SerializeField] protected Animator anim;
    [SerializeField] protected GameObject body;
    [SerializeField] private GameObject[] hitVFX;
    [SerializeField] private Transform[] knockBackPos;
    [SerializeField] private Transform downPos;
    [SerializeField] protected Transform[] spawnMovePos;
    protected Coroutine movementCoroutine;
    protected Coroutine hitCoroutine;
    private Coroutine attackMovementCoroutine;

    private Tween attackMoveTween;
    private List<System.Func<IEnumerator>> spawnList;


    #region Spawn
    public void Spawn()
    {
        Status_Setting();
        target = PlayerAction_Manager.instance.gameObject;

        spawnList = new List<System.Func<IEnumerator>>()
        {
            Spawn_None, Spawn_Movement, Spawn_CutScene
        };

        StartCoroutine(spawnList[(int)spawnType]());
    }

    private void Status_Setting()
    {
        enemyName = statusData.ObjectName;
        moveSpeed = statusData.MoveSpeed;
        nav.speed = moveSpeed;

        physcialDamage = statusData.PhyScial_Damage;
        magicalDamage = statusData.Magical_Damage;
        criticalhit = statusData.Critical_hit;
        critical_multiplier = statusData.Critical_multiplier;
        attackSpeed = statusData.AttackSpeed;
        anim.SetFloat("AttackSpeed", attackSpeed);

        curHp = statusData.Hp;
        maxHp = statusData.Hp;
        physicalDefence = statusData.Physical_Defence;
        magicalDefence = statusData.Magical_Defence;
        curGroggy = statusData.Groggy;
        maxGroggy = statusData.Groggy;
        groggyTime = statusData.GroggyTime;
    }

    protected IEnumerator Spawn_None()
    {
        curState = State.Spawn;
        enemyUI.UI_Setting();
        enemyUI.UI_OnOff(true);

        anim.SetTrigger("Action");
        anim.SetBool("isSpawn", true);
        while (anim.GetBool("isSpawn"))
        {
            yield return null;
        }

        enemyUI.UI_Setting();
        enemyUI.UI_OnOff(true);
        curState = State.Idle;
    }

    protected IEnumerator Spawn_Movement()
    {
        curState = State.Spawn;

        for (int i = 0; i < spawnMovePos.Length; i++)
        {
            // 점프 지점 바라보기
            LookAt(spawnMovePos[i].gameObject, 0.15f);

            // 점프 대기
            yield return spawnMovePos[i].DOJump(spawnMovePos[i].position, 3, 1, 1.25f).SetEase(Ease.OutQuad).WaitForCompletion();

            // 다음 점프 딜레이
            yield return new WaitForSeconds(0.15f);
        }

        enemyUI.UI_Setting();
        enemyUI.UI_OnOff(true);
        curState = State.Idle;
    }

    protected abstract IEnumerator Spawn_CutScene();
    #endregion


    #region Attack
    protected abstract void Think();

    public (bool isCritical, int damage) DamageCalculation(Attack_Base.Value attackData)
    {
        // 크리티컬 계산
        bool isCirtical = criticalhit >= Random.Range(0, 100) ? true : false;

        // 데미지 계산
        Skill_Value_SO.Value_Data sBase = attackData.levelValue.GetData(0);
        int calDamage = (int)((sBase.type == IDamageSysteam.DamageType.Physical ? physcialDamage : magicalDamage) * Random.Range(sBase.motionValue.x, sBase.motionValue.y) * (isCirtical ? critical_multiplier : 1));

        return (isCirtical, calDamage);
    }

    public void Attack_Movement(Transform movePos, float speed)
    {
        if (attackMovementCoroutine != null)
            StopCoroutine(attackMovementCoroutine);

        StartCoroutine(AttackMovementCall(movePos, speed));
    }

    private IEnumerator AttackMovementCall(Transform movePos, float speed)
    {
        float duration = 1f / speed;
        float elapsed = 0f;

        Vector3 start = transform.position;
        Vector3 target = movePos.position;

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            float easedT = EasingFunctions.OutExpo(t);

            Vector3 current = Vector3.Lerp(start, target, easedT);
            Vector3 delta = current - transform.position;

            controller.Move(delta);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 마지막 위치 보정
        Vector3 finalOffset = target - transform.position;
        controller.Move(finalOffset);
    }

    public void Check_Target()
    {
        targetDir = PlayerAction_Manager.instance.transform.position - transform.position;
        targetRange = targetDir.magnitude;
    }

    public void LookAt(GameObject target, float lookSpeed)
    {
        Vector3 lookDir = (target.transform.position - transform.position).normalized;
        lookDir.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(lookDir);

        transform.DOKill();  // 기존 트위닝 중단
        transform.DORotateQuaternion(lookRotation, lookSpeed);  // Quaternion을 사용하여 부드럽게 회전
    }

    public void Delay()
    {
        StartCoroutine(DelayMovement());
    }

    protected abstract IEnumerator DelayMovement();
    #endregion


    #region Hit
    public void Take_Damage(GameObject attackObj, IDamageSysteam.DamageType type, IDamageSysteam.HitVFX hit, bool isCirtical, int hitCount, int damage)
    {
        if (curState == State.Die)
        {
            return;
        }

        // 데미지 계산
        for (int i = 0; i < hitCount; i++)
        {
            int calDamage = damage - (type == IDamageSysteam.DamageType.Physical ? physicalDefence : magicalDefence);
            if (calDamage > 0)
            {
                // 데미지 & 그로기
                curHp -= calDamage;
                curGroggy -= (calDamage / 10);
                HitEffect(type, isCirtical, damage);

                // 카메라 흔들림
                CameraEffect_Manager.instance.Camera_Shack(1, 0.1f);

                // 플레이어 각성 게이지
                Player_Manager.instance.status.AwankingAdd((int)(calDamage * 0.25f));

                // 사망 체크
                if (curHp <= 0)
                {
                    curHp = 0;
                    Hit_Reset();
                    Die();
                    return;
                }
            }
        }

        // 상태이상 체크 - 일반 몬스터는 즉시 / 엘리트 & 보스 몬스터는 게이지 소모 시
        if (enemyType == EnemyType.Normal)
        {
            // 일반 동작 체크
            switch (hit)
            {
                case IDamageSysteam.HitVFX.None:
                    CameraEffect_Manager.instance.Camera_Shack(1.5f, 0.05f);
                    break;

                case IDamageSysteam.HitVFX.KnockBack:
                    if (isGroggy) return;
                    CameraEffect_Manager.instance.Camera_Shack(3, 0.05f);
                    if (hitCoroutine != null) StopCoroutine(hitCoroutine);
                    hitCoroutine = StartCoroutine(Hit_KnockBack(attackObj));
                    break;

                case IDamageSysteam.HitVFX.Down:
                    CameraEffect_Manager.instance.Camera_Shack(5, 0.05f);
                    if (hitCoroutine != null) StopCoroutine(hitCoroutine);
                    hitCoroutine = StartCoroutine(Hit_Down(attackObj));
                    break;
            }
        }
        else
        {
            // 엘리트 & 보스 동작 체크
            if (curGroggy <= 0 && !isGroggy)
            {
                StartCoroutine(Hit_Groggy());
            }
        }
    }

    private void HitEffect(IDamageSysteam.DamageType type, bool isCirtial, int damage)
    {
        // 피격 이펙트
        // Instantiate((type == IDamageSysteam.DamageType.Physical ? hitVFX[0] : hitVFX[1]), transform.position, Quaternion.identity);

        // 피격 시 바디 진동
        ShakeEffect(0.1f, isCirtial ? 0.3f : 0.15f);

        // 카메라 쉐이킹

        // UI 최신화
        enemyUI.Hp();
        enemyUI.Groggy();

        // 데미지 UI - 예정
        // 몬헌의 치명타 표기처럼 데미지 표기
        // 물리 : 흰색 + 붉은색 아웃라인 / 마법 : 흰색 + 파란색 아웃라인
        // 치명타 : 위 색상조합 + (볼드체 & 테두리)
        enemyUI.DamageUI(type, isCirtial, damage);
    }

    private void ShakeEffect(float duration, float strength)
    {
        body.transform.DOKill(); // 기존 흔들림을 제거하고 새로운 흔들림 적용
        body.transform.DOShakePosition(duration, strength, 10, 90, false, true);
    }

    private void Hit_Reset()
    {
        nav.enabled = false;

        // 이동 코루틴 종료
        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        // 이동 초기화
        if (attackMovementCoroutine != null)
            StopCoroutine(attackMovementCoroutine);

        // 공격 종료
        for (int i = 0; i < attackDatas.Count; i++)
        {
            attackDatas[i].Attack_Reset();
        }

        // 애니메이션 트리거 종료
        for (int i = 0; i < animTrigger.Length; i++)
        {
            anim.ResetTrigger(animTrigger[i]);
        }

        // 애니메이션 종료
        for (int i = 0; i < animBool.Length; i++)
        {
            anim.SetBool(animBool[i], false);
        }
    }

    public IEnumerator Hit_KnockBack(GameObject attackObj)
    {
        curState = State.Groggy;

        // 행동 강제 종료 -> 코루틴을 종료시키는 무언가
        Hit_Reset();

        // 넉백 애니메이션
        anim.SetTrigger("Hit");
        anim.SetBool("isKnockBack", true);
        anim.SetFloat("AnimValue", 0);
        int ran = Random.Range(0, knockBackPos.Length);
        anim.SetInteger("HitType", ran);

        // 넉백 이동
        transform.DOKill();
        Vector3 startPos = transform.position;
        Vector3 endPos = knockBackPos[ran].position;
        float timer = 0;
        float moveTime = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / groggyTime;
            anim.SetFloat("AnimValue", timer);

            moveTime = moveTime < 1 ? moveTime += Time.deltaTime * 2f : 1;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(moveTime));

            yield return null;
        }
        anim.SetFloat("AnimValue", 1);
        anim.SetBool("isKnockBack", false);

        // 재동작
        curState = State.Idle;
    }

    public IEnumerator Hit_Down(GameObject attackObj)
    {
        curState = State.Groggy;
        isGroggy = true;

        // 행동 강제 종료 -> 코루틴을 종료시키는 무언가
        Hit_Reset();

        // 다운 애니메이션
        anim.SetTrigger("Hit");
        anim.SetBool("isDown", true);

        // 다운 이동
        transform.DOKill();
        Vector3 startPos = transform.position;
        Vector3 endPos = downPos.position;
        float timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime / groggyTime;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        // 다운 대기 -> 다운 관련 시간 필요 -> 스테이터스에 추가
        yield return new WaitForSeconds(statusData.GroggyTime);

        // 기상 애니메이션
        anim.SetTrigger("Action");
        while (anim.GetBool("isDown"))
        {
            yield return null;
        }

        // 재동작
        isGroggy = false;
        curState = State.Idle;
    }

    public IEnumerator Hit_Groggy()
    {
        curState = State.Groggy;
        isGroggy = true;

        // 행동 강제 종료 -> 코루틴을 종료시키는 무언가
        Hit_Reset();

        // 다운 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isDown", true);

        // 다운 대기 -> 다운 관련 시간 필요 -> 스테이터스에 추가
        yield return new WaitForSeconds(statusData.GroggyTime);

        // 기상 애니메이션
        anim.SetTrigger("Action");
        while (anim.GetBool("isDown"))
        {
            yield return null;
        }

        // 재동작
        isGroggy = false;
        curGroggy = maxGroggy;
        curState = State.Idle;
    }

    public abstract void Die();
    #endregion
}
