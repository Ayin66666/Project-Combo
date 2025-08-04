using System.Collections;
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
        PlayerAction_Manager.instance.hitAction += Hit_Check;
    }


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        PlayerAction_Manager.instance.MovementLock(cancelType, true);
        PlayerAction_Manager.instance.isCounter = true;
        PlayerAction_Manager.instance.isAttack = true;
        isHit = false;

        // ������ ���
        for (int i = 0; i < value_Normal.Count; i++)
        {
            DamageCal(i);
        }

        // �ִϸ��̼� ����
        PlayerAction_Manager.instance.Animation_Reset();
        PlayerAction_Manager.instance.LookAt();

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isCounterReady", true);
        anim.SetBool("isCounter", true);


        // ����
        Player_Sound.instance.Sound_Skill(Player_Sound.Skill.Counter_Start);

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

        PlayerAction_Manager.instance.isCounter = false;
        PlayerAction_Manager.instance.MovementLock(cancelType, false);
        PlayerAction_Manager.instance.AttackOver();
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
        PlayerAction_Manager.instance.isAttack = true;
        PlayerAction_Manager.instance.LookAt();

        // ���� ���� UI ȣ��
        UI_Manager.instance.AttackGuide(nextAttackData);

        // ������ ���
        Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Awakening, 20);

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isCounter", true);
        anim.SetBool("isCounterReady", false);
        while (anim.GetBool("isAttack"))
        {
            yield return null;
        }

        PlayerAction_Manager.instance.isInvincibility = false;
        PlayerAction_Manager.instance.isAttack = false;

        // �߰�Ÿ �Է� ���
        float timer = 0;
        while (anim.GetBool("isCounter"))
        {
            timer += Time.deltaTime;
            if (timer > time && Input_Manager.instance.movementInput.magnitude > 0)
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

        PlayerAction_Manager.instance.MovementLock(cancelType, false);
        PlayerAction_Manager.instance.isCounter = false;
        PlayerAction_Manager.instance.AttackOver();
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
        PlayerAction_Manager.instance.MovementLock(cancelType, true);
        PlayerAction_Manager.instance.isAttack = true;
        PlayerAction_Manager.instance.LookAt();


        // �ݶ��̴� ����
        PlayerAction_Manager.instance.Collider_Ignore(true);

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isCounter", true);
        while (anim.GetBool("isAttack"))
        {
            yield return null;
        }

        PlayerAction_Manager.instance.isAttack = false;
        float timer = 0;
        while (anim.GetBool("isCounter"))
        {
            timer += Time.deltaTime;
            if (timer > time && Input_Manager.instance.movementInput.magnitude > 0)
            {
                anim.SetBool("isCounter", false);
                break;
            }

            yield return null;
        }

        // �ݶ��̴� ����
        PlayerAction_Manager.instance.Collider_Ignore(false);

        // ���� �ݶ��̴� ����
        Attack_ColliderReset();

        // ���� ����
        PlayerAction_Manager.instance.MovementLock(cancelType, false);
        PlayerAction_Manager.instance.isCounter = false;
        PlayerAction_Manager.instance.AttackOver();
    }
    #endregion


    public void Hit_Check()
    {
        isHit = true;
    }

    public override void AttackVFX(int index)
    {
        // ����
        switch (index)
        {
            case 0:
                Player_Sound.instance.Sound_Skill(Player_Sound.Skill.Counter_Success);
                break;

            case 1:
                Player_Sound.instance.Sound_Skill(Player_Sound.Skill.Counter_Add);
                break;
        }

        // ����Ʈ
        counterVFX[index].SetActive(true);
    }

    public void ImpactVFX()
    {
        Instantiate(counterVFX[2], impactPos.position, Quaternion.identity);
    }

    public override void DamageCal(int index)
    {
        (bool isCritical, int damage) = PlayerAction_Manager.instance.DamageCalculation(value_Normal[index], skillLevel);
        Skill_Value_SO.Value_Data skillData = value_Normal[index].levelValue.GetData(skillLevel);
        value_Normal[index].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, AttackCollider_Controller.Owner.Player);
    }

    public override void Attack_Reset()
    {
        // ���� ����
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        PlayerAction_Manager.instance.isCounter = false;
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
