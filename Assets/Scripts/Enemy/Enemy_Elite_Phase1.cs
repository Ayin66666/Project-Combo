using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy_Elite_Phase1 : Enemy_Base
{
    [Header("---Component---")]
    [SerializeField] private Enemy_Elite_Controller phaseController;


    [Header("---Dead Setting---")]
    [SerializeField] private List<DeadExplosion> explosions;
    public int a;
    [System.Serializable]
    public struct DeadExplosion
    {
        public GameObject explosion;
        public float delayTime;
    }


    private void Start()
    {
        Spawn();
    }

    private void Update()
    {
        if(curState == State.Idle)
        {
            Think();
        }
        
        /*
        if(Input.GetKeyUp(KeyCode.U))
        {
            attackDatas[a].Use();
        }
        */
    }

    protected override void Think()
    {
        curState = State.Think;

        Check_Target();
        if(targetRange <= 5)
        {
            int ran = Random.Range(0, attackDatas.Count);
            attackDatas[0].Use();
        }

        if(targetRange > 5)
        {
            int ran = Random.Range(1, attackDatas.Count);
            attackDatas[ran].Use();
        }
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
        yield return new WaitForSeconds(0.5f);
        curState = State.Idle;
    }

    public override void Die()
    {
        base.Die();
        StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        curState = State.Die;

        enemyUI.gameObject.SetActive(false);

        // 애니메이션 - 이거 애니메이션으로 컷씬 찍는게?
        anim.SetTrigger("Hit");
        anim.SetBool("isDie", true);
        yield return new WaitUntil(() => anim.GetBool("isDie"));

        // 컷신

        // 2페이즈 전환
        phaseController.Stage_Spawn(1);
    }
}
