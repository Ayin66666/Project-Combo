using System.Collections;
using UnityEngine;
using UnityEngine.Video;


public class Enemy_Elite_Phase2 : Enemy_Base
{
    [Header("---Setting---")]
    [SerializeField] private GameObject[] weapons;

    // ���� ����
    // 1. �ǽ� - �齺�� - ���Ϳ���
    // 2. �齺�� - �̻����� - ���Ϳ���
    // 3. Ʈ���� - �ǽ� - �̻��� ��
    // 4. �ǽ� - Ʈ���� - �齺��
    // 5. Ʈ���� - �齺�� - ���Ϳ���
    private int[,] pattens_Melee = new int[5, 3]
    {
        { 0,1,3 },{ 1,4,3 },{ 2,0,4 },{ 0,2,1 },{ 2,1,3 }
    };

    // ���Ÿ� ����
    // 1. ���Ϳ��� - �̻����� - �ǽ�
    // 2. �̻����� - �ǽ� - �齺��
    // 3. �齺�� - ���Ϳ��� - �̻�����
    private int[,] pattens_Range = new int[3, 3]
    {
        { 3,4,0 },{ 4,0,1 },{ 2,3,4 }
    };

    public enum SoundKey 
    {
        // �̵�
        Backstep_Move, Forward_Move,

        // �齺�� ����
        BackstepShooting_Charge, BackstepShooting_Shoot,

        // Ʈ����
        TripeShooting_Shoot12, TripeShooting_Shoot3,

        // ���Ϳ���
        AlterEgo_Charge, AlterEgo_Shoot,

        // �̻��� ȣ��
        Misslie_Charge, Misslie_GunShoot, Misslie_Shoot, Misslie_Off,

        // �� ������
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
            // ���� ����
            StartCoroutine(Patten_Use(pattens_Melee, ran));
        }
        else
        {
            // ���Ÿ� ����
            StartCoroutine(Patten_Use(pattens_Range, ran));
        }
    }

    /// <summary>
    /// ���� ���
    /// </summary>
    /// <param name="pattens">����, ���Ÿ� ���� ������ �迭</param>
    /// <param name="type">������ �迭 ���� ���� �ε�����</param>
    /// <returns></returns>
    private IEnumerator Patten_Use(int[,] pattens, int type)
    {
        curState = State.Attack;

        // ���� ����
        for (int i = 0; i < pattens.GetLength(1); i++)
        {
            // -1�� ���� = ���� ���� ��
            if (pattens[type, i] > -1)
            {
                // ���� ����
                isPatten = true;
                attackDatas[pattens[type, i]].Use();

                while (isPatten)
                {
                    yield return null;
                }

                yield return new WaitForSeconds(0.15f);
            }
        }

        // ������ �ൿ
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
                Debug.Log("����");

                // �ʹ� �ֶ� - ����
                z = Mathf.MoveTowards(z, 1f, Time.deltaTime * 2f);
                x = Mathf.MoveTowards(x, 0f, Time.deltaTime * 2f);
                moveDir = Vector3.zero;

                if (!nav.enabled)
                    nav.enabled = true;

                nav.SetDestination(target.transform.position);
            }
            else if (targetRange >= 3 && targetRange <= 5)
            {
                Debug.Log("�ֽ� �̵�");

                // ������ �Ÿ��϶� - �ֽ� & ���� �̵�
                z = Mathf.MoveTowards(z, 0f, Time.deltaTime * 2f);
                x = Mathf.MoveTowards(x, 1f, Time.deltaTime * 2f);

                if (nav.enabled)
                    nav.enabled = false;

                LookAt(target, 0);
                moveDir = transform.right;
            }
            else if (targetRange < 3)
            {
                Debug.Log("����");

                // �ʹ� ����� �� - ���� & ���� �̵�
                z = Mathf.MoveTowards(z, -1f, Time.deltaTime * 2f);
                x = Mathf.MoveTowards(x, -1f, Time.deltaTime * 2f);

                if (nav.enabled)
                    nav.enabled = false;

                LookAt(target, 0);
                moveDir = -transform.forward + -transform.right;
            }

            if (moveDir != Vector3.zero)
                controller.Move(3f * Time.deltaTime * moveDir.normalized);

            // �ִϸ��̼�
            anim.SetFloat("Movement X", x);
            anim.SetFloat("Movement Z", z);

            timer += Time.deltaTime;
            yield return null;
        }

        // ���� ���� - �ִϸ��̼�
        nav.enabled = false;
        while (z != 0 || x != 0)
        {
            Debug.Log("���� �ִ�");
            z = Mathf.MoveTowards(z, 0, Time.deltaTime * 5f);
            x = Mathf.MoveTowards(x, 0, Time.deltaTime * 5f);
            anim.SetFloat("Movement X", x);
            anim.SetFloat("Movement Z", z);
            yield return null;
        }
        Debug.Log("����");

        // 
        Think();
    }

    protected override IEnumerator DelayMovement()
    {
        curState = State.Delay;

        // ������ �̵� - �ȱ�
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
        // 2������� ���� �ƽ� X
        curState = State.Idle;
        yield return null;
    }

    public override void Die()
    {
        StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isDie", true);
        while(anim.GetBool("isDie"))
        {
            yield return null;
        }

        // ��ü ��Ȱ��ȭ
        body.SetActive(false);

        // ��� �ƽ�
        video.clip = dieClip;
        yield return new WaitForSeconds(0.1f);
        while (video.isPlaying)
        {
            yield return null;
        }

        // ������ ���
        Item_Drop();

        // ��� ����
        Destroy(gameObject);
    }
}
