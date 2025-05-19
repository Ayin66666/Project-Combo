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

        // ������ ���
        for (int i = 0; i < value_Normal.Count; i++)
        {
            DamageCal(i);
        }

        // �ִϸ��̼� ����
        Player_Manager.instance.Animation_Reset();
        Player_Manager.instance.LookAt();

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isCounterReady", true);
        anim.SetBool("isCounter", true);

        // ī���� ���
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

        // ī���� ���� - �ִϸ��̼� ���
        anim.SetBool("isCounterReady", false);
        while (anim.GetBool("isCounter"))
        {
            yield return null;
        }

        // ���� �ݶ��̴� ����
        Attack_ColliderReset();

        Player_Manager.instance.isCounter = false;
        Player_Manager.instance.MovementLock(cancelType, false);
        Player_Manager.instance.AttackOver();
    }


    #region ����
    /// <summary>
    /// ī���� ���� - ���κ���
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

        // ������ ���
        Player_Manager.instance.AwankingAdd(20);

        // �ִϸ��̼�
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

        // �߰�Ÿ �Է� ���
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

        // ���� �ݶ��̴� ����
        Attack_ColliderReset();

        Player_Manager.instance.MovementLock(cancelType, false);
        Player_Manager.instance.isCounter = false;
        Player_Manager.instance.AttackOver();
    }

    /// <summary>
    /// ī���� ���� - �������
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


        // �ݶ��̴� ����
        Player_Manager.instance.Collider_Ignore(true);

        // �ִϸ��̼�
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

        // �ݶ��̴� ����
        Player_Manager.instance.Collider_Ignore(false);

        // ���� �ݶ��̴� ����
        Attack_ColliderReset();

        // ���� ����
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
        // ���� ����
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        Player_Manager.instance.isCounter = false;
        isHit = false;

        // ����Ʈ ����
        foreach (GameObject vfx in counterVFX)
        {
            vfx.SetActive(false);
        }

        // ����Ʈ ����
        Attack_ColliderReset();
    }
}
