using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using Easing.Tweening;


public class Enemy_Boss_Arie : Enemy_Base
{
    [Header("---Setting---")]
    [SerializeField] private Phase curPhase;
    [SerializeField] private Character_Status_SO phase2_Status;
    [SerializeField] private int chargePattenCount;
    [SerializeField] private float chaseTime;
    [SerializeField] private float pattenDelaytime;
    [SerializeField] private Vector2 delayTime;
    [SerializeField] private int attackUse;
    private enum Phase { Phase1, Phase2 };
    public enum SoundKey
    {
        // 이동
        Movement_FrontStep, Movement_Backstep,

        // 콤보
        Combo12, Combo3,

        // 콤보 어퍼
        ComboUpper1, ComboUpper2, ComboUpperExplosion,

        // 대쉬 어퍼
        DashUpper_Upper, DashUpper_360, DashUpper_Explosion,

        // 얼터에고
        AlterEgo_Slash1, AlterEgo_Slash2, AlterEgo_Upper, AlterEgo_360,

        // 러쉬
        Rush_Sting, Rush_Slash, Rush_360,

        // 차지 슬래쉬
        ChargeSlash_Charge, ChargeSlash_Upper, ChargeSlash_Sting, ChargeSlash_360,

        // 에너지 오브
        EnergyOrb_Charge, EnergyOrb_Shoot, EnergyOrb_Drop,

        // 스트라이크
        Strike_1, Strike_Rush, Strike_3,

        // 잡기
        Subdue_Slash12, Subdue_Strike, Subdue_Slash3, Subdue_Slash4,

        // 스페셜
        Special_Charge, Special_Jump, Special_SwordAura, Special_AirCharge, Special_Laser1, Special_Explosion
    }


    [Header("---Cut Scene---")]
    [SerializeField] private VideoClip[] clips;
    [SerializeField] private Dialog_Data_SO phase2Dialog;

    [Header("---Attack Setting---")]
    [SerializeField] private int attackCount;
    private enum AttackType { Melee, Range, Special }
    // 근거리 패턴
    // 1. 콤보 - 러쉬 - 콤보어퍼 - 스트라이크
    // 2. 콤보어퍼 - 얼터에고 - 러쉬 - 잡기
    // 3. 콤보 - 얼터에고 - 대쉬어퍼 - 차지슬래쉬
    // 4. 얼터에고 - 차지오브 - 러쉬 - 스트라이크
    private int[,] pattens_Melee = new int[4, 4]
    {
        { 0,4,1,7 },{ 1,3,4,8 },{ 0,3,2,5 },{ 3,6,4,7 }
    };

    // 원거리 패턴
    // 1. 대쉬어퍼 - 얼터에고 - 차지오브 - 스트라이크
    // 2. 러쉬 - 콤보 - 잡기 - 차지슬래쉬
    // 3. 스트라이크 - 대쉬어퍼 - 차지오브 - 얼터에고
    // 4. 러쉬 - 콤보어퍼 - 대쉬어퍼 - 차지슬래쉬
    private int[,] pattens_Range = new int[4, 4]
    {
        { 2,3,6,7 },{ 4,0,8,5 },{ 7,2,6,3 },{ 4,1,2,5 }
    };

    // 강화 패턴
    // 1. 러쉬 - 얼터에고 - 대쉬어퍼 - 차지오브 - 레이저
    // 2. 대쉬어퍼 - 스트라이크 - 러쉬 - 차지슬래쉬 - 레이저
    private int[,] pattens_Special = new int[2, 5]
    {
        { 4,3,2,6,9 },{ 2,7,4,5,9 },
    };

    // 공격 패턴
    // 1. 콤보 슬래쉬 - 3회 공격
    // 2. 콤보어퍼 - 2회 어퍼
    // 3. 대쉬어퍼 - 플레이어 방향 돌진 후 어퍼
    // 4. 얼터에고 & 소드 오러 - 분신소환 검기발사
    // 5. 러쉬 - 돌진 공격
    // 6. 차지슬래쉬 - 차징 후 범위 공격
    // 7. 차지오브 - 대형 구체소환 후 탄막 발사 - 오브 발사
    // 8. 스트라이크 - 돌진 내려찍기 후 제자리 내려찍기
    // 9. 잡기 - 돌진 후 잡기 공격
    // 필살기 - 플라잉 레이저 - 공중 선회 비행 - 소형 드론 소환 - (컷씬 : 대형 레이져) - 중앙 폭발


    [Header("---Skin Objects---")]
    [SerializeField] private SkinnedMeshRenderer bosster;
    [SerializeField] private SkinnedMeshRenderer weapon;
    [SerializeField] private GameObject bossterVFX;
    [SerializeField] private GameObject weaponVFX;
    private Coroutine bossterCoroutine;
    private Coroutine weaponCoroutine;


    private void Update()
    {
        if (curState == State.Die)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            attackDatas[attackUse].Use();
            //Bosster_Setting(true);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            //Bosster_Setting(false);
        }

        // 페이즈 전환
        if (curHp <= 1000 && curPhase == Phase.Phase1)
        {
            if (movementCoroutine != null)
                StopCoroutine(movementCoroutine);

            movementCoroutine = StartCoroutine(Phase_Change());
        }
    }


    #region Wepaon & Booster Effect
    public void Bosster_Setting(bool isOn)
    {
        if (bossterCoroutine != null)
            StopCoroutine(bossterCoroutine);

        bossterCoroutine = StartCoroutine(BossterCall(isOn));
    }

    private IEnumerator BossterCall(bool isOn)
    {
        bossterVFX.SetActive(isOn);

        float start = isOn ? 0 : 100;
        float end = isOn ? 100 : 0;
        float timer = 0;
        float t = isOn ? 0 : 100;
        while (timer < 1)
        {
            timer += Time.deltaTime * (isOn ? 2f : 1f);
            t = Mathf.Lerp(start, end, EasingFunctions.OutExpo(timer));
            bosster.SetBlendShapeWeight(0, t);
            yield return null;
        }
    }


    public void Weapon_Setting(bool isOn)
    {
        if (weaponCoroutine != null)
            StopCoroutine(weaponCoroutine);

        weaponCoroutine = StartCoroutine(WeaponCall(isOn));
    }

    private IEnumerator WeaponCall(bool isOn)
    {
        if (!isOn) weaponVFX.SetActive(false);

        float start = isOn ? 0 : 100;
        float end = isOn ? 100 : 0;
        float timer = 0;
        float t = isOn ? 0 : 100;
        while (timer < 1)
        {
            timer += Time.deltaTime * 2f;
            t = Mathf.Lerp(start, end, EasingFunctions.OutExpo(timer));
            weapon.SetBlendShapeWeight(2, t);
            yield return null;
        }

        if (isOn) weaponVFX.SetActive(true);
    }
    #endregion


    #region Attack
    protected override void Think()
    {
        Check_Target();
        /*
        // 일반 공격
        int ran = Random.Range(0, 4);
        if (targetRange <= 5)
        {
            // 근거리
            attackCount++;
            movementCoroutine = StartCoroutine(Patten_Use(pattens_Melee, ran));
        }
        else
        {
            // 추격 - 공격 로직
            attackCount++;
            movementCoroutine = StartCoroutine(ChaseMovement(ran));
        }
        */
        
        if (attackCount <= 5)
        {
            // 강화 공격
            attackCount = 0;
            int ran = Random.Range(0, 2);
            movementCoroutine = StartCoroutine(Patten_Use(pattens_Special, ran));
        }
        else
        {
            // 일반 공격
            int ran = Random.Range(0, 4);
            if (targetRange <= 5)
            {
                // 근거리
                attackCount++;
                movementCoroutine = StartCoroutine(Patten_Use(pattens_Melee, ran));
            }
            else
            {
                // 추격 - 공격 로직
                attackCount++;
                movementCoroutine = StartCoroutine(ChaseMovement(ran));
            }
        }
    }

    private IEnumerator Patten_Use(int[,] pattens, int type)
    {
        curState = State.Attack;

        // 패턴 공격
        for (int i = 0; i < pattens.GetLength(1); i++)
        {
            // -1은 무시 = 없는 패턴 값
            if (pattens[type, i] > -1)
            {
                // 패턴 동작
                isPatten = true;
                attackDatas[pattens[type, i]].Use();

                while (isPatten)
                {
                    yield return null;
                }

                yield return new WaitForSeconds(pattenDelaytime);
            }
        }

        // 딜레이 행동
        float ran = Random.Range(2.35f, 3.1f);
        movementCoroutine = StartCoroutine(DelayMovement());

    }

    private IEnumerator ChaseMovement(int ran)
    {
        curState = State.Chase;

        float tiemr = 0;
        nav.enabled = true;
        while (targetRange <= 5 && tiemr < chaseTime)
        {
            nav.SetDestination(target.transform.position);
            yield return null;
        }
        nav.enabled = false;

        // 공격 호출
        movementCoroutine = StartCoroutine(Patten_Use(pattens_Range, ran));
    }

    protected override IEnumerator DelayMovement()
    {
        curState = State.Delay;

        nav.enabled = true;
        Vector3 moveDir = Vector3.zero;
        float timer = 0;
        float z = 0;
        float x = 0;
        float time = Random.Range(delayTime.x, delayTime.y);
        while (timer < time)
        {
            Check_Target();
            if (targetRange > 5)
            {
                // 너무 멀때 - 추적
                z = Mathf.MoveTowards(z, 1f, Time.deltaTime * 2f);
                x = Mathf.MoveTowards(x, 0f, Time.deltaTime * 2f);
                moveDir = Vector3.zero;

                if (!nav.enabled)
                    nav.enabled = true;

                nav.SetDestination(target.transform.position);
            }
            else if (targetRange >= 3 && targetRange <= 5)
            {
                // 적당한 거리일때 - 주시 & 우측 이동
                z = Mathf.MoveTowards(z, 0f, Time.deltaTime * 2f);
                x = Mathf.MoveTowards(x, 1f, Time.deltaTime * 2f);

                if (nav.enabled)
                    nav.enabled = false;

                LookAt(target, 0);
                moveDir = transform.right;
            }
            else if (targetRange < 3)
            {
                // 너무 가까울 때 - 후퇴 & 좌측 이동
                z = Mathf.MoveTowards(z, -1f, Time.deltaTime * 2f);
                x = Mathf.MoveTowards(x, -1f, Time.deltaTime * 2f);

                if (nav.enabled)
                    nav.enabled = false;

                LookAt(target, 0);
                moveDir = -transform.forward + -transform.right;
            }

            if (moveDir != Vector3.zero)
                controller.Move(3f * Time.deltaTime * moveDir.normalized);

            // 애니메이션
            anim.SetFloat("Movement X", x);
            anim.SetFloat("Movement Z", z);

            timer += Time.deltaTime;
            yield return null;
        }

        // 추적 종료 - 애니메이션
        nav.enabled = false;
        while (z != 0 || x != 0)
        {
            Debug.Log("종료 애니");
            z = Mathf.MoveTowards(z, 0, Time.deltaTime * 5f);
            x = Mathf.MoveTowards(x, 0, Time.deltaTime * 5f);
            anim.SetFloat("Movement X", x);
            anim.SetFloat("Movement Z", z);
            yield return null;
        }

        curState = State.Idle;
    }
    #endregion


    #region Spawn & Phase Change & Dead
    protected override IEnumerator Spawn_CutScene()
    {
        curState = State.Spawn;
        LookAt(target, 0);

        // 스폰 컷신
        enemyUI.CutScene(clips[0]);
        while (enemyUI.isCutScene)
        {
            yield return null;
        }

        // 2 페이즈 시작 대사
        UI_Manager.instance.Dialog_Fight(phase2Dialog);


        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isSpawn", true);
        while (anim.GetBool("isSpawn"))
        {
            yield return null;
        }

        curState = State.Idle;
        Think();
    }

    private IEnumerator Phase_Change()
    {
        curPhase = Phase.Phase2;
        curState = State.Spawn;

        // 보스 강화
        enemyName = phase2_Status.ObjectName;
        moveSpeed = phase2_Status.MoveSpeed;
        nav.speed = moveSpeed;
        pattenDelaytime = 0.1f;

        physcialDamage = phase2_Status.PhyScial_Damage;
        magicalDamage = phase2_Status.Magical_Damage;
        criticalhit = phase2_Status.Critical_hit;
        critical_multiplier = phase2_Status.Critical_multiplier;

        physicalDefence = phase2_Status.Physical_Defence;
        magicalDefence = phase2_Status.Magical_Defence;
        anim.SetFloat("AttackSpeed", 1.15f);

        // 2페이즈 컷씬
        enemyUI.CutScene(clips[1]);
        while (enemyUI.isCutScene)
        {
            yield return null;
        }

        // 페이즈 전환 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isPhaseAnim", true);
        while (anim.GetBool("isPhaseAnim"))
        {
            yield return null;
        }

        curState = State.Idle;
        Think();
    }

    public override void Die()
    {
        base.Die();
        StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        curState = State.Die;

        anim.SetTrigger("Hit");
        anim.SetBool("isDie", true);
        while (anim.GetBool("isDie"))
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.25f);

        // 종료 컷씬
        enemyUI.CutScene(clips[1]);
        while (enemyUI.isCutScene)
        {
            yield return null;
        }

        Destroy(gameObject);
    }
    #endregion
}
