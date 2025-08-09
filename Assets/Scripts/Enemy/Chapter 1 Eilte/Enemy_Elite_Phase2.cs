using System.Collections;
using UnityEngine;
using UnityEngine.Video;


public class Enemy_Elite_Phase2 : Enemy_Base
{
    [Header("---Setting---")]
    [SerializeField] private GameObject[] weapons;

    // 근접 패턴
    // 1. 건슬 - 백스텝 - 얼터에고
    // 2. 백스텝 - 미사일콜 - 얼터에고
    // 3. 트리플 - 건슬 - 미사일 콜
    // 4. 건슬 - 트리플 - 백스탭
    // 5. 트리플 - 백스탭 - 얼터에고
    private int[,] pattens_Melee = new int[5, 3]
    {
        { 0,1,3 },{ 1,4,3 },{ 2,0,4 },{ 0,2,1 },{ 2,1,3 }
    };

    // 원거리 패턴
    // 1. 얼터에고 - 미사일콜 - 건슬
    // 2. 미사일콜 - 건슬 - 백스텝
    // 3. 백스텝 - 얼터에고 - 미사일콜
    private int[,] pattens_Range = new int[3, 3]
    {
        { 3,4,0 },{ 4,0,1 },{ 2,3,4 }
    };

    public enum SoundKey 
    {
        // 이동
        Backstep_Move, Forward_Move,

        // 백스탭 슈팅
        BackstepShooting_Charge, BackstepShooting_Shoot,

        // 트리플
        TripeShooting_Shoot12, TripeShooting_Shoot3,

        // 얼터에고
        AlterEgo_Charge, AlterEgo_Shoot,

        // 미사일 호출
        Misslie_Charge, Misslie_GunShoot, Misslie_Shoot, Misslie_Off,

        // 건 슬래쉬
        GunSlash_Slash12, GunSlash_Slash3
    }


    [Header("---Compoment---")]
    private VideoPlayer video;
    [SerializeField] private VideoClip dieClip;


    private void Start()
    {
        video = GetComponent<VideoPlayer>();
        Spawn();
    }

    private void Update()
    {
        if(curState == State.Die)
        {
            return;
        }

        if (curState == State.Idle)
        {
            Think();
        }
    }


    protected override void Think()
    {
        curState = State.Think;

        Check_Target();
        int ran = Random.Range(0, targetRange <= 5 ? pattens_Melee.GetLength(0) : pattens_Range.GetLength(0));
        if (targetRange <= 5)
        {
            // 근접 패턴
            StartCoroutine(Patten_Use(pattens_Melee, ran));
        }
        else
        {
            // 원거리 패턴
            StartCoroutine(Patten_Use(pattens_Range, ran));
        }
    }

    /// <summary>
    /// 공격 기능
    /// </summary>
    /// <param name="pattens">근접, 원거리 패턴 이차원 배열</param>
    /// <param name="type">이차원 배열 내의 패턴 인덱스값</param>
    /// <returns></returns>
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

                yield return new WaitForSeconds(0.15f);
            }
        }

        // 딜레이 행동
        float ran = Random.Range(2.35f, 3.1f);
        StartCoroutine(Chase_Delay(ran));
    }

    private IEnumerator Chase_Delay(float chaseTime)
    {
        curState = State.Chase;
        Debug.Log("Delay Movement Call");

        nav.enabled = true;
        Vector3 moveDir = Vector3.zero;
        float timer = 0;
        float z = 0;
        float x = 0;
        while (timer < chaseTime)
        {
            Check_Target();
            if (targetRange > 5)
            {
                Debug.Log("추적");

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
                Debug.Log("주시 이동");

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
                Debug.Log("후퇴");

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
        Debug.Log("종료");

        // 
        Think();
    }

    protected override IEnumerator DelayMovement()
    {
        curState = State.Delay;

        // 딜레이 이동 - 걷기
        float ranDelay = Random.Range(1.3f, 1.6f);
        int ran = Random.Range(0, 1);
        Vector3 moveDir = ((ran == 0 ? transform.right : -transform.right) + -transform.forward);

        float timer = 0;
        float animValue = 0;
        while (timer < ranDelay)
        {
            if (animValue > -1)
            {
                animValue -= Time.deltaTime * 2.5f;
                anim.SetFloat("Movement", animValue);
            }

            timer += Time.deltaTime;
            LookAt(target, 0);
            controller.Move(3f * Time.deltaTime * moveDir.normalized);
            yield return null;
        }
        anim.SetFloat("Movement", 0);

        curState = State.Idle;
    }

    protected override IEnumerator Spawn_CutScene()
    {
        // 2페이즈는 스폰 컷신 X
        curState = State.Idle;
        yield return null;
    }

    public override void Die()
    {
        StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isDie", true);
        while(anim.GetBool("isDie"))
        {
            yield return null;
        }

        // 신체 비활성화
        body.SetActive(false);

        // 사망 컷신
        video.clip = dieClip;
        yield return new WaitForSeconds(0.1f);
        while (video.isPlaying)
        {
            yield return null;
        }

        // 아이템 드랍
        Item_Drop();

        // 사망 전달
        Destroy(gameObject);
    }
}
