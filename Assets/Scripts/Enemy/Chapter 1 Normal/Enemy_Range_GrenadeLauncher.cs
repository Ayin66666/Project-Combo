using System.Collections;
using UnityEngine;

public class Enemy_Range_GrenadeLauncher : Enemy_Base
{
    private void OnEnable()
    {
        Spawn();
    }


    protected override void Think()
    {
        // »ç¸Á Ã¼Å©
        if (curState == State.Groggy || curState == State.Die) return;

        // ÇÃ·¹ÀÌ¾î »ç¸Á Ã¼Å©
        if (Player_Manager.instance.action.isDie)
        {
            curState = State.Idle;
            return;
        }

        curState = State.Think;
        LookAt(target, 0.05f);
        Check_Target();

        // ÀÏ¹Ý °ø°Ý
        if (targetRange <= attackRange)
        {
            int ran = Random.Range(0, attackDatas.Count);
            attackDatas[ran].Use();
        }
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

    protected override IEnumerator DelayMovement()
    {
        curState = State.Delay;

        // µô·¹ÀÌ ÀÌµ¿ - °È±â
        float ranDelay = Random.Range(0.75f, 1.25f);
        int ran = Random.Range(0, 1);
        Vector3 moveDir = ran == 0 ? transform.right : -transform.right;

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
        nav.enabled = false;

        // ¾ÆÀÌÅÛ µå¶ø
        base.Die();

        anim.SetTrigger("Hit");
        anim.SetBool("isDie", true);
        while(anim.GetBool("isDie"))
        {
            yield return null;
        }

        // º¹±Í µô·¹ÀÌ
        yield return new WaitForSeconds(1f);

        // Ç®¸µ º¹±Í
        if (Stage_Manager.instance != null)
        {
            Stage_Manager.instance.enemy_Container.Return_Enemy(Enemy_Container.Enemy.GrenadeLauncher, gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void Reset_Enemy()
    {
        base.Reset_Enemy();

        // Ç®¸µ º¹±Í
        if (Stage_Manager.instance != null)
        {
            Stage_Manager.instance.enemy_Container.Return_Enemy(Enemy_Container.Enemy.GrenadeLauncher, gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected override IEnumerator Spawn_CutScene()
    {
        throw new System.NotImplementedException();
    }
}
