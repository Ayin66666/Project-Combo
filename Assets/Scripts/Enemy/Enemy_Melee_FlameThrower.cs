using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing.Tweening;


public class Enemy_Melee_FlameThrower : Enemy_Base
{
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

        curState = State.Think;
        LookAt(target, 0.05f);
        Check_Target();

        // ����
        if (targetRange <= attackRange)
        {
            int ran = Random.Range(0, attackDatas.Count);
            if (targetRange > attackRange * 0.5f)
            {
                // �뽬 �� ����
                movementCoroutine = StartCoroutine(DashAttack(ran));
            }
            else
            {
                // �ٷ� ����
                attackDatas[ran].Use();
            }

        }
        // ����
        else if(targetRange > attackRange)
        {
            movementCoroutine = StartCoroutine(Chase());
        }
    }

    private IEnumerator Chase()
    {
        curState = State.Chase;

        nav.enabled = true;
        float timer = 0;
        anim.SetFloat("Movement", 0);
        while (targetRange > attackRange)
        {
            if (timer < 1)
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

    private IEnumerator DashAttack(int ran)
    {
        LookAt(target, 0);

        anim.SetTrigger("Action");
        anim.SetBool("isDash", true);

        // �̵�
        Vector3 moveDir = (target.transform.position - transform.position).normalized;
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + moveDir * 2f;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 2.5f;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        while (anim.GetBool("isDash"))
        {
            yield return null;
        }

        attackDatas[ran].Use();
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
        movementCoroutine = StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        curState = State.Die;
        enemyUI.UI_OnOff(false);
        nav.enabled = false;

        // ������ ���
        base.Die();

        // �ִϸ��̼�
        anim.SetTrigger("Hit");
        anim.SetBool("isDie", true);
        while(anim.GetBool("isDie"))
        {
            yield return null;
        }

        // ���� ������
        yield return new WaitForSeconds(1f);

        // Ǯ�� ����
        if (Stage_Manager.instance != null)
        {
            Stage_Manager.instance.enemy_Container.Return_Enemy(Enemy_Container.Enemy.Flame, gameObject);
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
            Stage_Manager.instance.enemy_Container.Return_Enemy(Enemy_Container.Enemy.Flame, gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected override IEnumerator Spawn_CutScene()
    {
        // �̻��
        throw new System.NotImplementedException();
    }
}
