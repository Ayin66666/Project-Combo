using System.Collections;
using UnityEngine;
using Easing.Tweening;


public class Enemy_Melee_Axe : Enemy_Base
{
    [Header("---Tutorial Setting---")]
    [SerializeField] private Transform dashPos;
    public enum SoundKey { Dash, Swing }


    private void OnEnable()
    {
        Spawn();
    }

    protected override void Think()
    {
        // ��� üũ
        if (curState == State.Groggy || curState == State.Die) return;

        // �÷��̾� ��� üũ
        if (Player_Manager.instance.action.isDie)
        {
            curState = State.Idle;
            return;
        }

        // �Ǵ�
        curState = State.Think;
        LookAt(target, 0.05f);
        Check_Target();

        // �Ϲ� ����
        if(targetRange <= 2)
        {
            attackDatas[0].Use();
        }
        // �뽬 �� ����
        else if (targetRange < 6)
        {
            movementCoroutine = StartCoroutine(DashAttack());
        }
        // ����
        else
        {
            movementCoroutine = StartCoroutine(Chase());
        }
    }

    private IEnumerator Chase()
    {
        curState = State.Chase;

        nav.enabled = true;
        float timer = 0;
        anim.SetFloat("Movement", timer);
        while (targetRange > attackRange)
        {
            if(timer < 1)
            {
                timer += Time.deltaTime * 2.5f;
                anim.SetFloat("Movement", timer);
            }

            Check_Target();
            nav.SetDestination(PlayerAction_Manager.instance.gameObject.transform.position);
            yield return null;
        }
        nav.enabled = false;
        anim.SetFloat("Movement", 0);

        Think();
    }

    private IEnumerator DashAttack()
    {
        LookAt(target, 0);

        // ����
        sound.Sound(SoundKey.Dash.ToString());

        anim.SetTrigger("Action");
        anim.SetBool("isDash", true);

        // �̵�
        Vector3 moveDir = (target.transform.position - transform.position).normalized;
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + moveDir * 2f;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 2.5f;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        while(anim.GetBool("isDash"))
        {
            yield return null;
        }

        attackDatas[0].Use();
    }

    protected override IEnumerator DelayMovement()
    {
        curState = State.Delay;

        // ������ �̵� - �ȱ�
        float ranDelay = Random.Range(0.75f, 1.25f);
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

        Think();
    }


    public override void Die()
    {
        Hit_Reset();
        StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        curState = State.Die;
        enemyUI.UI_OnOff(false);

        // ����
        sound.Sound(Enemy_Sound.SoundKey.Die.ToString());

        // ������ ���
        base.Die();

        // ��� �ִϸ��̼�
        anim.SetTrigger("Hit");
        anim.SetBool("isDie",true);
        while(anim.GetBool("isDie"))
        {
            yield return null;
        }

        // ���� ������
        yield return new WaitForSeconds(1f);

        // Ǯ�� ����
        if (Stage_Manager.instance != null)
        {
            Stage_Manager.instance.enemy_Container.Return_Enemy(Enemy_Container.Enemy.Axe, gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void Reset_Enemy()
    {
        base.Reset_Enemy();

        // Ǯ�� ����
        if (Stage_Manager.instance != null)
        {
            Stage_Manager.instance.enemy_Container.Return_Enemy(Enemy_Container.Enemy.Axe, gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected override IEnumerator Spawn_CutScene()
    {
        yield return null;
    }
}
