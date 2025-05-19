using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Other_Counter : Attack_Base
{
    [Header("--- Counter Setting ---")]
    [SerializeField] private float counterTime;
    [SerializeField] private GameObject[] counterVFX;
    [SerializeField] private Transform impactPos;
    private bool isHit;


    private void Start()
    {
        Player_Manager.instance.hitAction += Hit_Check;
    }


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        Player_Manager.instance.MovementLock(cancelType, true);
        Player_Manager.instance.isCounter = true;
        Player_Manager.instance.isAttack = true;
        isHit = false;

        // 데미지 계산
        for (int i = 0; i < value_Normal.Count; i++)
        {
            DamageCal(i);
        }

        // 애니메이션 리셋
        Player_Manager.instance.Animation_Reset();
        Player_Manager.instance.LookAt();

        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isCounterReady", true);
        anim.SetBool("isCounter", true);

        // 카운터 대기
        float timer = 0;
        while (timer < counterTime)
        {
            if (isHit)
            {
                Counter_Success();
            }
            timer += Time.deltaTime;
            yield return null;
        }

        // 카운터 실패 - 애니메이션 대기
        anim.SetBool("isCounterReady", false);
        while (anim.GetBool("isCounter"))
        {
            yield return null;
        }

        // 공격 콜라이더 리셋
        Attack_ColliderReset();

        Player_Manager.instance.isCounter = false;
        Player_Manager.instance.MovementLock(cancelType, false);
        Player_Manager.instance.AttackOver();
    }


    #region 성공
    /// <summary>
    /// 카운터 성공 - 가로베기
    /// </summary>
    /// <returns></returns>
    public void Counter_Success()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(Attack_HorizontalSalsh());
    }

    private IEnumerator Attack_HorizontalSalsh()
    {
        Player_Manager.instance.isAttack = true;
        Player_Manager.instance.LookAt();

        // 게이지 상승
        Player_Manager.instance.AwankingAdd(20);

        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isCounter", true);
        anim.SetBool("isCounterReady", false);
        while (anim.GetBool("isAttack"))
        {
            yield return null;
        }

        Player_Manager.instance.isInvincibility = false;
        Player_Manager.instance.isAttack = false;

        // 추가타 입력 대기
        float timer = 0;
        while (anim.GetBool("isCounter"))
        {
            timer += Time.deltaTime;
            if(timer > time && Input_Manager.instance.movementInput.magnitude > 0)
            {
                anim.SetBool("isCounter", false);
                break;
            }

            if (Input_Manager.instance.inputDatas[2].isInput)
            {
                Counter_Success2();
            }
            yield return null;
        }

        // 공격 콜라이더 리셋
        Attack_ColliderReset();

        Player_Manager.instance.MovementLock(cancelType, false);
        Player_Manager.instance.isCounter = false;
        Player_Manager.instance.AttackOver();
    }

    /// <summary>
    /// 카운터 성공 - 내려찍기
    /// </summary>
    /// <returns></returns>
    private void Counter_Success2()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(Attack_VerticalStrike());
    }

    private IEnumerator Attack_VerticalStrike()
    {
        Player_Manager.instance.MovementLock(cancelType, true);
        Player_Manager.instance.isAttack = true;
        Player_Manager.instance.LookAt();


        // 콜라이더 무시
        Player_Manager.instance.Collider_Ignore(true);

        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isCounter", true);
        while (anim.GetBool("isAttack"))
        {
            yield return null;
        }

        Player_Manager.instance.isAttack = false;
        float timer = 0;
        while(anim.GetBool("isCounter"))
        {
            timer += Time.deltaTime;
            if(timer > time && Input_Manager.instance.movementInput.magnitude > 0)
            {
                anim.SetBool("isCounter", false);
                break;
            }

            yield return null;
        }

        // 콜라이더 무시
        Player_Manager.instance.Collider_Ignore(false);

        // 공격 콜라이더 리셋
        Attack_ColliderReset();

        // 공격 종료
        Player_Manager.instance.MovementLock(cancelType, false);
        Player_Manager.instance.isCounter = false;
        Player_Manager.instance.AttackOver();
    }
    #endregion


    public void Hit_Check()
    {
        isHit = true;
    }

    public override void AttackVFX(int index)
    {
        counterVFX[index].SetActive(true);
    }

    public void ImpactVFX()
    {
        Instantiate(counterVFX[2], impactPos.position, Quaternion.identity);
    }

    public override void DamageCal(int index)
    {
        (bool isCritical, int damage) = Player_Manager.instance.DamageCalculation(value_Normal[index], skillLevel);
        Skill_Base.Value_Data skillData = value_Normal[index].levelValue.GetData(skillLevel);
        value_Normal[index].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);
    }

    public override void Attack_Reset()
    {
        // 동작 종료
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        Player_Manager.instance.isCounter = false;
        isHit = false;

        // 이펙트 종료
        foreach (GameObject vfx in counterVFX)
        {
            vfx.SetActive(false);
        }

        // 리스트 리셋
        Attack_ColliderReset();
    }
}
