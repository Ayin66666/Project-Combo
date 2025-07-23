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
        // 사망 체크
        if (curState == State.Groggy || curState == State.Die) return;

        // 플레이어 사망 체크
        if (Player_Manager.instance.action.isDie)
        {
            curState = State.Idle;
            return;
        }

        curState = State.Think;
        LookAt(target, 0.05f);
        Check_Target();

        // 공격
        if (targetRange <= attackRange)
        {
            int ran = Random.Range(0, attackDatas.Count);
            if (targetRange > attackRange * 0.5f)
            {
                // 대쉬 후 공격
                movementCoroutine = StartCoroutine(DashAttack(ran));
            }
            else
            {
                // 바로 공격
                attackDatas[ran].Use();
            }

        }
        // 추적
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

        // 이동
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

        // 딜레이 이동 - 걷기
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

        // 아이템 드랍
        base.Die();

        // 애니메이션
        anim.SetTrigger("Hit");
        anim.SetBool("isDie", true);
        while(anim.GetBool("isDie"))
        {
            yield return null;
        }

        // 복귀 딜레이
        yield return new WaitForSeconds(1f);

        // 풀링 복귀
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

        // 풀링 복귀
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
        // 미사용
        throw new System.NotImplementedException();
    }
}
