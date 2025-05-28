using System.Collections;
using UnityEngine;
using DG.Tweening;
using Easing.Tweening;
using System.Collections.Generic;


public class Player_Manager : MonoBehaviour, IDamageSysteam
{
    public static Player_Manager instance;

    public int level;

    [Header("---Status---")]
    // Defence Status
    public int curhp;
    public int maxHp;
    public int physicalDefence;
    public int magicalDefence;

    // Attack Status
    public int physicalDamage;
    public int magicalDamage;
    public int attackSpeed;
    public float criticalhit;
    public float critical_multiplier;

    // Other Status
    public float moveSpeed;
    public float curStamina;
    public float maxStamina;
    public float curAwakening;
    public float maxAwakening;


    // 아머 타입 추가해야함! - 그리고 아머타입에 따른 피격효과 셋팅도!
    [Header("---State---")]
    [SerializeField] private IDamageSysteam.HitVFX curHitState;
    public bool canAction;
    public bool useGravity;
    private bool isGoround;

    public bool canMovement;
    public bool canAwakning;
    [SerializeField] private bool canDash;
    [SerializeField] private bool canRushSlash;
    [SerializeField] private bool canSpecial;

    public bool isCursorLock;
    public bool isAttack;
    public bool isSmash;
    public bool isCounter;
    public bool isAwakning;
    public bool isInvincibility;
    public bool isDie;


    [Header("---Movement Setting---")]
    [SerializeField] private LayerMask groundLayer;
    private Vector3 moveDir;
    private Vector3 camDir;
    private float turnSmoothVelocity;


    [Header("---Dash Setting---")]
    private Coroutine dashCoroutine;
    private Vector3 dashDir;
    private bool isDash;


    [Header("--- Attack State ---")]
    public int attackCount = 0;
    [SerializeField] private Attack_Base[] normalAttacks;
    [SerializeField] private Attack_Base[] smashAttacks;
    [SerializeField] private Attack_Base[] otherAttakcs;
    [SerializeField] private Attack_Base specialAttack;


    [Header("---Hit Setting---")]
    [SerializeField] private Collider damagePosCollider;
    [SerializeField] private GameObject hitDamageUI;
    [SerializeField] private Transform downMovePos;
    [SerializeField] private Transform[] hitMovePos;
    [SerializeField] private GameObject[] hitVFX;
    [SerializeField] private string[] animTrigger;
    [SerializeField] private string[] animBool;
    private Coroutine hitEffectCoroutine;


    [Header("---Lock On---")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private List<GameObject> lockOnEnemyList;
    [SerializeField] private GameObject lockOnEnemy;
    [SerializeField] private GameObject lockOnFront; 
    private int curLookIndex;
    public bool isLockOn;


    [Header("---Component---")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject[] cinemachineCam;
    [SerializeField] private Transform camHolder;
    public Transform cam;
    public GameObject shootTarget;
    public GameObject bodyObject;
    public System.Action hitAction;
    private Coroutine attackMovementCoroutine;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        KeyActionSetting();
    }

    private void Update()
    {
        if (canAction)
        {
            if (!isLockOn)
            {
                Look();
            }

            Collider_Ignore(true);
            Movement();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            LockOn_EnemySearch();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Cursor_Setting(Cursor.lockState != CursorLockMode.Locked);
        }


        Gravity();
        Recovery();
    }

    public void Status_Setting(Data data)
    {
        curhp = data.curhp;
        maxHp = data.maxHp;
        physicalDefence = data.physicalDefence;
        magicalDefence = data.magicalDefence;

        physicalDamage = data.physicalDamage;
        magicalDamage = data.magicalDamage;
        criticalhit = data.criticalhit;
        critical_multiplier = data.critical_multiplier;
        attackSpeed = data.attackSpeed;

        moveSpeed = data.moveSpeed;
        curStamina = data.curStamina;
        maxStamina = data.maxStamina;
        curAwakening = data.curAwakening;
        maxAwakening = data.maxAwakening;
    }

    public void Cursor_Setting(bool isOn)
    {
        Cursor.lockState = isOn ? CursorLockMode.Locked : CursorLockMode.None;
        canAction = isOn;

        if(!isOn)
        {
            anim.SetBool("isMove", false);
            anim.SetBool("isStop", true);
        }
    }

    private void LockOn_EnemySearch()
    {
        // On 상태일때 - Off로 전환
        if (isLockOn)
        {
            isLockOn = false;
            lockOnEnemy.GetComponent<Enemy_Base>().enemyUI.LockOn(false);
            return;
        }

        // 리스트 초기화
        lockOnEnemyList.Clear();
        curLookIndex = 0;

        // 정면 적 서치
        Vector3 boxCenter = lockOnFront.transform.position;
        Quaternion boxRotation = lockOnFront.transform.rotation;
        Vector3 boxSize = lockOnFront.transform.lossyScale * 0.5f;
        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxSize, boxRotation, enemyLayer);
        if(hitColliders.Length > 0 )
        {
            float minDistance = int.MaxValue;
            for (int i = 0; i < hitColliders.Length; i++)
            {
                // 거리 체크
                float targetRange = (hitColliders[i].transform.position - transform.position).sqrMagnitude;
                if (targetRange < minDistance)
                {
                    // 거리가 더 짧을 경우 최우선 타겟으로 셋팅
                    minDistance = targetRange;
                    lockOnEnemy = hitColliders[i].gameObject;
                }
            }
        }
        else
        {
            // 현재 범위 내 적들 탐색
            Collider[] enemys = Physics.OverlapSphere(transform.position, 50f, enemyLayer);
            foreach (Collider enemy in enemys)
            {
                lockOnEnemyList.Add(enemy.gameObject);
            }

            // 탐색한 몬스터 중 가장 가까운 몬스터 락온
            float minDistance = int.MaxValue;
            lockOnEnemy = null;
            for (int i = 0; i < enemys.Length; i++)
            {
                // 거리 체크
                float targetRange = (enemys[i].transform.position - transform.position).sqrMagnitude;
                if (targetRange < minDistance)
                {
                    // 거리가 더 짧을 경우 최우선 타겟으로 셋팅
                    minDistance = targetRange;
                    lockOnEnemy = enemys[i].gameObject;
                }
            }
        }

        // 타겟 주시 기능 동작
        if (lockOnEnemy != null)
        {
            StartCoroutine(LockOn());
        }
    }

    private IEnumerator LockOn()
    {
        isLockOn = true;

        // 애너미 락온 UI
        lockOnEnemy.GetComponent<Enemy_Base>().enemyUI.LockOn(true);

        // 카메라 회전
        float easeDuration = 1.5f;
        float elapsed = 0f;
        while (isLockOn)
        {
            // 락온 대상 사망 시 해제
            if (lockOnEnemy == null)
            {
                isLockOn = false;
                break;
            }

            // 속도 제어 - 초반에 빠르게 -> 이후 느리게
            float t = Mathf.Clamp01(elapsed / easeDuration);
            float currentSpeed = Mathf.Lerp(720f, 360f, EasingFunctions.OutExpo(t));

            // 회전 제어
            Vector3 lookDir = (lockOnEnemy.transform.position - camHolder.transform.position).normalized;
            lookDir.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);

            // 회전 보간
            camHolder.transform.rotation = Quaternion.RotateTowards(camHolder.transform.rotation, targetRotation, currentSpeed * Time.deltaTime);

            /*
            Vector3 lookDir = (target.transform.position - camHolder.transform.position).normalized;
            lookDir.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(lookDir);

            camHolder.transform.DOKill();  // 기존 트위닝 중단
            camHolder.transform.DORotateQuaternion(lookRotation, 0);
            */
            yield return null;
        }
    }

    private void Recovery()
    {
        if (curStamina < maxStamina)
            curStamina += Time.deltaTime * 5f;
    }

    public void Collider_Ignore(bool isOn)
    {
        // 충돌 무시 시작
        Physics.IgnoreLayerCollision(6, 7, isOn);
    }

    /// <summary>
    /// 초기 값 셋팅
    /// </summary>
    private void KeyActionSetting()
    {
        Input_Manager.instance.Action_Setting(0, Attack_Normal);
        Input_Manager.instance.Action_Setting(1, Attack_Smash);
        Input_Manager.instance.Action_Setting(2, Attack_Counter);
        Input_Manager.instance.Action_Setting(3, Buff_Awanking);
        Input_Manager.instance.Action_Setting(4, Dash);
        Input_Manager.instance.Action_Setting(5, Attack_RushSalsh);
        Input_Manager.instance.Action_Setting(6, Attack_Speical);
    }


    #region Movement
    /// <summary>
    /// 즉시 카메라 위치 바라보기
    /// </summary>
    public void LookAt()
    {
        transform.DOKill();

        // camDir 방향을 기준으로 Y축 회전 각도 계산
        float targetAngle = Mathf.Atan2(camDir.x, camDir.z) * Mathf.Rad2Deg;

        // 회전
        transform.DORotate(new Vector3(0, targetAngle, 0), 0.1f, RotateMode.FastBeyond360).SetEase(Ease.OutExpo);
    }

    public void ChargeLookAt()
    {
        // camDir 방향을 기준으로 Y축 회전 각도 계산
        float targetAngle = Mathf.Atan2(camDir.x, camDir.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, targetAngle, 0));
    }

    private void Look()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = camHolder.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }

        camHolder.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }

    private void Movement()
    {
        moveDir = Input_Manager.instance.movementInput.normalized;

        float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, 0.1f);

        camDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        dashDir = moveDir.magnitude == 0 ? dashDir = transform.forward : camDir;

        if (canMovement)
        {
            //플레이어 정면 조절
            if (moveDir.magnitude != 0)
            {
                anim.SetBool("isMove", true);
                anim.SetBool("isStop", false);

                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                Vector3 mDir = new Vector3(camDir.x, 0, camDir.z).normalized;
                controller.Move(moveSpeed * Time.deltaTime * mDir);
            }
            else
            {
                anim.SetBool("isMove", false);
                anim.SetBool("isStop", true);
            }
        }
    }

    private void Dash()
    {
        if (dashCoroutine != null)
            StopCoroutine(dashCoroutine);

        dashCoroutine = StartCoroutine(DashCall());
    }

    private IEnumerator DashCall()
    {
        if (!canAction || !canDash || curStamina < 30)
        {
            yield break;
        }
        else
        {
            canDash = false;
            isDash = true;

            // 스테미너 소모
            curStamina -= 30f;

            // 애니메이션 리셋
            Animation_Reset();

            anim.ResetTrigger("Action");
            anim.SetTrigger("Action");
            anim.SetBool("isDodge", true);
            anim.SetFloat("DashMotion", 0);

            // 대쉬
            Vector3 dir = dashDir;
            float timer = 0;
            float dashTime;
            while (timer < 0.25f)
            {
                timer += Time.deltaTime;
                dashTime = Mathf.Clamp01(timer / 0.25f);

                controller.Move(EasingFunctions.OutExpo(dashTime) * 30f * Time.deltaTime * dir);
                anim.SetFloat("DashMotion", timer);
                yield return null;
            }

            anim.SetBool("isDodge", false);
            anim.SetFloat("DashMotion", 0);

            canDash = true;
            isDash = false;
        }
    }

    public void RideMovement(Vector3 pos)
    {
        controller.Move(pos);
    }

    private void Gravity()
    {
        // 중력 활성화 여부 체크
        if (!useGravity)
        {
            return;
        }

        // 지상 체크
        isGoround = Physics.Raycast(transform.position, Vector3.down, 0.1f, groundLayer);
        Debug.DrawRay(transform.position, Vector3.down * 0.1f, Color.red);
        if (!isGoround)
        {
            Vector3 dir = new Vector3(0, (isDash ? -20f : -9.81f), 0);
            controller.Move(dir * Time.deltaTime);
        }
    }

    public void Pos_Setting(Vector3 pos, Quaternion rotation)
    {
        controller.enabled = false;
        transform.SetPositionAndRotation(pos, rotation);
        controller.enabled = true;

        Debug.Log($"인풋 값 : {pos} / {rotation}");
        Debug.Log($"플레이어 값 : {transform.position} / {transform.rotation}");
    }
    #endregion


    #region Hit

    /// <summary>
    /// 피격 시 데미지 계산 / hitCount 만큼 데미지를 나눠서 표기함!
    /// </summary>
    /// <param name="type"></param>
    /// <param name="isCritical"></param>
    /// <param name="hitCount"></param>
    /// <param name="damage"></param>
    public void Take_Damage(GameObject attackObj, IDamageSysteam.DamageType type, IDamageSysteam.HitVFX hitType, bool isCritical, int hitCount, int damage)
    {
        // 사망 체크
        if (isDie)
        {
            return;
        }

        // 피격 액션 - 카운터
        hitAction?.Invoke();

        // 무적 상태 체크
        if (isInvincibility)
        {
            return;
        }

        // 데미지 계산
        int calDamage = damage - (type == IDamageSysteam.DamageType.Physical ? physicalDefence : magicalDefence);
        if (calDamage > 0)
        {
            // 타격 횟수만큼 동작
            for (int i = 0; i < hitCount; i++)
            {
                // 체력 감소
                int calendDamage = calDamage / hitCount;
                curhp -= calendDamage;

                // 사망 체크
                if (curhp < 0)
                {
                    Die();
                    break;
                }

                // UI 최신화
                UI_Manager.instance.Hp();

                // 피격 데미지 UI
                GameObject obj = Instantiate(hitDamageUI, HitVFXPos(), transform.rotation);
                obj.GetComponent<DamageUI>().Setting(type, isCritical, calendDamage);

                // 피격 이펙트
                // Instantiate((type == IDamageSysteam.DamageType.Physical ? hitVFX[0] : hitVFX[1]), transform.position, Quaternion.identity);
            }

            // 피격 효과 - 다운 상태에서는 무효화
            if (isDie || curHitState == IDamageSysteam.HitVFX.Down)
                return;

            switch (hitType)
            {
                case IDamageSysteam.HitVFX.None:
                    CameraEffect_Manager.instance.Camera_Shack(3, 0.05f);
                    break;

                case IDamageSysteam.HitVFX.KnockBack:
                    CameraEffect_Manager.instance.Camera_Shack(8, 0.075f);

                    if (hitEffectCoroutine != null)
                        StopCoroutine(hitEffectCoroutine);

                    hitEffectCoroutine = StartCoroutine(Hit_KnockBack(attackObj));
                    break;

                case IDamageSysteam.HitVFX.Down:
                    CameraEffect_Manager.instance.Camera_Shack(10, 0.1f);

                    if (hitEffectCoroutine != null)
                        StopCoroutine(hitEffectCoroutine);

                    hitEffectCoroutine = StartCoroutine(Hit_Down(attackObj));
                    break;
            }
        }
    }

    private void Die()
    {
        Animation_Reset();
        Hit_Reset();

        StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        canAction = false;
        isDie = true;

        // 애니메이션 초기화
        Animation_Reset();

        // 이동 초기화
        if (attackMovementCoroutine != null)
            StopCoroutine(attackMovementCoroutine);

        // 일반공격 리셋
        foreach (Attack_Base attack in normalAttacks)
        {
            attack.Attack_Reset();
        }

        // 스메쉬 리셋
        foreach (Attack_Base attack in smashAttacks)
        {
            attack.Attack_Reset();
        }

        // 스킬 리셋
        foreach (Attack_Base attack in otherAttakcs)
        {
            attack.Attack_Reset();
        }

        // 기능 초기화
        AttackOver();

        // 애니메이션
        anim.SetTrigger("Hit");
        anim.SetBool("isDead", true);
        while (anim.GetBool("isDead"))
        {
            yield return null;
        }

        // 사망 UI
        UI_Manager.instance.DieUI(true);
        while (!UI_Manager.instance.isDieUI)
        {
            yield return null;
        }

        // 입력 대기
        while (!Input.GetKeyDown(KeyCode.F))
        {
            yield return null;
        }


        // UI 종료
        UI_Manager.instance.DieUI(false);

        // 체크포인트로 회귀
        Stage_Manager.instance.CheckPoint_Call();

        // 공격 초기화

        // 리스폰 애니메이션 
        anim.SetTrigger("Action");
        anim.SetBool("isRespawn", true);
        while (anim.GetBool("isRespawn"))
        {
            yield return null;
        }

        canAction = true;
        isDie = false;
    }

    public void Subdue(bool isOn, Transform movePos, Transform enemy)
    {
        switch (isOn)
        {
            case true:

                canAction = false;
                Hit_Reset();

                // 카메라 전환
                SubdueCam(isOn);

                // 바라보기
                Vector3 lookDir = (enemy.position - transform.position).normalized;
                lookDir.y = 0;
                Quaternion lookRotation = Quaternion.LookRotation(lookDir);
                transform.DOKill();
                transform.DORotateQuaternion(lookRotation, 0);

                // 애니메이션 동작
                anim.SetTrigger("Hit");
                anim.SetBool("isSubdue", true);

                // 포지션 셋팅
                StartCoroutine(SubdueMovement(movePos));
                break;

            case false:
                canAction = true;
                SubdueCam(false);
                anim.SetBool("isSubdue", false);
                break;
        }
    }

    private IEnumerator SubdueMovement(Transform movePos)
    {
        while (anim.GetBool("isSubdue"))
        {
            transform.position = movePos.position;
            yield return null;
        }
    }

    public void SubdueCam(bool isOn)
    {
        cinemachineCam[0].SetActive(!isOn);
        cinemachineCam[1].SetActive(isOn);
    }

    private Vector3 HitVFXPos()
    {
        Vector3 originPosition = damagePosCollider.transform.position;

        // 콜라이더의 사이즈를 가져오는 bound.size 사용
        float range_X = damagePosCollider.bounds.size.x;
        float range_Y = damagePosCollider.bounds.size.y;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Y = Random.Range((range_Y / 2) * -1, range_Y / 2);
        Vector3 RandomPostion = new Vector3(range_X, range_Y);

        Vector3 respawnPosition = originPosition + RandomPostion;
        return respawnPosition;
    }

    /// <summary>
    /// 피격 효과로 동작을 강제종료 시키는 무언가
    /// </summary>
    private void Hit_Reset()
    {
        // 공격 기능 초기화
        for (int i = 0; i < normalAttacks.Length; i++)
        {
            normalAttacks[i].Attack_Reset();
        }

        for (int i = 0; i < smashAttacks.Length; i++)
        {
            smashAttacks[i].Attack_Reset();
        }

        for (int i = 0; i < otherAttakcs.Length; i++)
        {
            otherAttakcs[i].Attack_Reset();
        }

        // 애니메이션 초기화
        Animation_Reset();
    }

    public IEnumerator Hit_KnockBack(GameObject attackObj)
    {
        curHitState = IDamageSysteam.HitVFX.KnockBack;
        canAction = false;

        // 행동 강제 종료 -> 코루틴을 종료시키는 무언가
        Hit_Reset();


        // 넉백 애니메이션
        int ran = Random.Range(0, hitMovePos.Length);
        anim.SetTrigger("Hit");
        anim.SetBool("isKnockBack", true);
        anim.SetInteger("HitType", ran);

        // 넉백 이동
        transform.DOKill();
        Vector3 startPos = transform.position;
        Vector3 endPos = hitMovePos[ran].position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 10f;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        // 넉백 애니메이션 대기
        while (anim.GetBool("isKnockBack"))
        {
            yield return null;
        }

        // 재동작
        curHitState = IDamageSysteam.HitVFX.None;
        canAction = true;
    }

    public IEnumerator Hit_Down(GameObject attackObj)
    {
        curHitState = IDamageSysteam.HitVFX.Down;
        canAction = false;

        // 행동 강제 종료 -> 코루틴을 종료시키는 무언가
        Hit_Reset();


        // 다운 애니메이션
        anim.SetTrigger("Hit");
        anim.SetBool("isDownLoop", true);
        anim.SetBool("isDown", true);

        // 다운 이동
        transform.DOKill();
        Vector3 startPos = transform.position;
        Vector3 endPos = downMovePos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        //anim.SetFloat("DownMotion", 1);

        // 다운 대기 -> 다운 관련 시간 필요
        yield return new WaitForSeconds(2f);

        // 기상 애니메이션
        anim.SetBool("isDownLoop", false);
        while (anim.GetBool("isDown"))
        {
            yield return null;
        }

        // 재동작
        curHitState = IDamageSysteam.HitVFX.None;
        canAction = true;
    }

    public void Animation_Reset()
    {
        // 트리거
        for (int i = 0; i < animTrigger.Length; i++)
        {
            anim.ResetTrigger(animTrigger[i]);
        }

        // Bool
        for (int i = 0; i < animBool.Length; i++)
        {
            anim.SetBool(animBool[i], false);
        }

        anim.SetInteger("AttackCount", 0);
    }
    #endregion


    #region Attack
    /// <summary>
    /// 데미지 계산 / 방어력 제외 계산임!
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="motionValue"></param>
    /// <param name="cirtical_hit"></param>
    /// <param name="critical_multiplier"></param>
    /// <returns></returns>
    public (bool isCritical, int damage) DamageCalculation(Attack_Base.Value attackData, int skillLevel)
    {
        // 크리티컬 계산
        bool isCirtical = criticalhit >= Random.Range(0, 100) ? true : false;

        // 데미지 계산 - 여기 레벨 벨류 어케할거 - 그냥 인덱스 추가해서 값 받아옴!
        Skill_Value_SO.Value_Data sBase = attackData.levelValue.GetData(skillLevel);
        int calDamage = (int)((sBase.type == IDamageSysteam.DamageType.Physical ? physicalDamage : magicalDamage) * Random.Range(sBase.motionValue.x, sBase.motionValue.y) * (isCirtical ? critical_multiplier : 1));

        return (isCirtical, calDamage);
    }

    public void MovementLock(Attack_Base.CancelType type, bool isOn)
    {
        if (isOn)
        {
            switch (type)
            {
                case Attack_Base.CancelType.Free:
                    canMovement = false;
                    canDash = true;
                    break;

                case Attack_Base.CancelType.Lock:
                    canMovement = false;
                    canDash = false;
                    break;
            }
        }
        else
        {
            canMovement = true;
            canDash = true;
        }
    }

    /// <summary>
    /// 공격 종료 후 수치 초기화
    /// </summary>
    public void AttackOver()
    {
        // 공격 카운트 초기화
        isAttack = false;
        isSmash = false;
        canRushSlash = false;
        canMovement = true;
        canDash = true;
        attackCount = 0;
        anim.SetInteger("AttackCount", attackCount);
    }

    private void Attack_Normal()
    {
        if (!canAction || isAttack || isSmash)
        {
            return;
        }
        else
        {
            // 최대 공격 횟수 체크
            if (attackCount < 4)
            {
                // 기존에 동작중이던 코루틴 종료
                foreach (Attack_Base attack in normalAttacks)
                {
                    attack.Attack_Reset();
                }

                isAttack = true;
                attackCount++;
                anim.SetInteger("AttackCount", attackCount);

                normalAttacks[attackCount - 1].Use();
            }
        }
    }

    private void Attack_Smash()
    {
        if (!canAction || isAttack || isSmash)
        {
            return;
        }
        else
        {
            if (attackCount > 0)
            {
                foreach (Attack_Base attack in normalAttacks)
                {
                    attack.Attack_Reset();
                }

                smashAttacks[attackCount - 1].Use();
            }
        }
    }

    public void RushSlash_Setting(bool isOn)
    {
        canRushSlash = isOn;
    }

    private void Attack_RushSalsh()
    {
        if (canRushSlash)
        {
            canRushSlash = false;

            // 일반공격 리셋
            foreach (Attack_Base attack in normalAttacks)
            {
                attack.Attack_Reset();
            }

            // 스메쉬 리셋
            foreach (Attack_Base attack in smashAttacks)
            {
                attack.Attack_Reset();
            }

            // 공격 동작
            otherAttakcs[0].Use();
        }
    }

    private void Attack_Counter()
    {
        if (!canAction || isAttack || isSmash || isCounter)
        {
            return;
        }

        otherAttakcs[1].Use();
    }

    private void Buff_Awanking()
    {
        if (!canAction || isAttack || isSmash)
        {
            return;
        }

        if (canAwakning && !isAwakning)
        {
            otherAttakcs[2].Use();
        }
    }

    public void Special_Setting(bool isOn)
    {
        canSpecial = isOn;
    }

    private void Attack_Speical()
    {
        if (!canAction || !canSpecial || !isAwakning || isAttack || isSmash || isCounter)
        {
            return;
        }

        specialAttack.Use();
    }

    public void AwankingAdd(int index)
    {
        curAwakening += index;
        if (curAwakening >= maxAwakening)
        {
            // 최대치를 넘을 경우
            curAwakening = maxAwakening;

            // 각성 활성화
            if (!canAwakning)
                canAwakning = true;
        }
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
    #endregion


    #region Buff
    public void Healing(int heal)
    {
        curhp += heal;
        if (curhp + heal > maxHp)
        {
            curhp = maxHp;
        }
    }
    #endregion
}
