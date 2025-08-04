using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Smash_UpperSlash : Attack_Base
{
    [Header("---UpperSlash Setting---")]
    [SerializeField] private GameObject[] upperSlashVFX;
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private Transform[] explosionPos;
    private Coroutine explosionCoroutine;


    public override void Use()
    {
        if (PlayerAction_Manager.instance.isSmash)
        {
            return;
        }

        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }


    /// <summary>
    /// ������� - �÷����� - (ȸ�� �������) 2(3)Ÿ / ���� ���¿��� ������� �߰� + 3Ÿ ȸ�� ������⿡ ���� ����
    /// </summary>
    /// <returns></returns>
    private IEnumerator UseCall()
    {
        PlayerAction_Manager.instance.MovementLock(cancelType, true);
        PlayerAction_Manager.instance.isAttack = true;
        PlayerAction_Manager.instance.isSmash = true;

        // ���� ���� UI ȣ��
        UI_Manager.instance.AttackGuide(nextAttackData);

        // ������ ���
        for (int i = 0; i < value_Normal.Count; i++)
        {
            DamageCal(i);
        }

        // 1Ÿ ����
        PlayerAction_Manager.instance.LookAt();
        anim.SetTrigger("Smash");
        anim.SetBool("isAttack", true);
        anim.SetBool("isSmash", true);
        while (anim.GetBool("isAttack"))
        {
            yield return null;
        }

        PlayerAction_Manager.instance.isAttack = false;
        PlayerAction_Manager.instance.RushSlash_Setting(true);

        // �߰�Ÿ ���
        float timer = 0;
        while (anim.GetBool("isSmash"))
        {
            timer += Time.deltaTime;

            // �߰�Ÿ
            if (PlayerAction_Manager.instance.isAwakning && Input_Manager.instance.inputDatas[1].isInput)
            {
                PlayerAction_Manager.instance.RushSlash_Setting(false);
                AdditionalAttack();
                yield break;
            }

            // �̵�
            if (timer > time && Input_Manager.instance.movementInput.magnitude > 0)
            {
                anim.SetBool("isSmash", false);
                break;
            }

            yield return null;
        }

        // ������ ����
        Attack_ColliderReset();

        PlayerAction_Manager.instance.MovementLock(cancelType, false);
        PlayerAction_Manager.instance.RushSlash_Setting(false);
        PlayerAction_Manager.instance.AttackOver();
    }


    private void AdditionalAttack()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(AdditionalAttackCall());
    }

    private IEnumerator AdditionalAttackCall()
    {
        PlayerAction_Manager.instance.MovementLock(cancelType, true);
        PlayerAction_Manager.instance.isAttack = true;
        PlayerAction_Manager.instance.LookAt();

        // ������ ���
        for (int i = 0; i < value_Normal.Count; i++)
        {
            DamageCal(i);
        }

        // 2Ÿ ����
        anim.SetTrigger("Smash");
        anim.SetBool("isAttack", true);
        anim.SetBool("isAdditionalSmash", true);
        while (anim.GetBool("isAttack"))
        {
            yield return null;
        }

        PlayerAction_Manager.instance.isAttack = false;

        float timer = 0;
        while (anim.GetBool("isAdditionalSmash"))
        {
            timer += Time.deltaTime;

            if (timer > time && Input_Manager.instance.movementInput.magnitude > 0)
            {
                anim.SetBool("isAdditionalSmash", false);
                PlayerAction_Manager.instance.MovementLock(cancelType, false);
                break;
            }

            yield return null;
        }

        // ������ ����
        Attack_ColliderReset();
        PlayerAction_Manager.instance.AttackOver();
    }


    public override void AttackVFX(int index)
    {
        // ����
        switch (index)
        {
            case 0:
                Player_Sound.instance.Sound_Smash(Player_Sound.Smash.Smash1_1);
                break;

            case 1:
                Player_Sound.instance.Sound_Smash(Player_Sound.Smash.Samsh1_2);
                break;

            case 2:
                Player_Sound.instance.Sound_Smash(Player_Sound.Smash.Samsh1_3);
                break;
        }

        // ����Ʈ
        upperSlashVFX[index].SetActive(true);
    }

    /// <summary>
    /// 2Ÿ ���� ���� ����
    /// </summary>
    public void Explosion()
    {
        if (explosionCoroutine != null)
            StopCoroutine(explosionCoroutine);

        explosionCoroutine = StartCoroutine(ExposionCall());
    }

    private IEnumerator ExposionCall()
    {
        // ������ ����
        Vector3[] pos = new Vector3[explosionPos.Length];
        for (int i = 0; i < explosionPos.Length; i++)
        {
            pos[i] = explosionPos[i].position;
        }

        // ����
        for (int i = 0; i < pos.Length; i++)
        {
            Instantiate(explosionVFX, pos[i], Quaternion.identity);
            yield return new WaitForSeconds(0.15f);
        }
    }

    public override void DamageCal(int index)
    {
        Skill_Value_SO.Value_Data skillData;
        if (PlayerAction_Manager.instance.isAwakning)
        {
            (bool isCritical, int damage) = PlayerAction_Manager.instance.DamageCalculation(value_Awakening[index], skillLevel);
            skillData = value_Awakening[index].levelValue.GetData(skillLevel);

            if (value_Awakening[index].attackCollider != null)
                value_Awakening[index].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, AttackCollider_Controller.Owner.Player);
        }
        else
        {
            (bool isCritical, int damage) = PlayerAction_Manager.instance.DamageCalculation(value_Normal[index], skillLevel);
            skillData = value_Normal[index].levelValue.GetData(skillLevel);

            if (value_Awakening[index].attackCollider != null)
                value_Normal[index].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, AttackCollider_Controller.Owner.Player);
        }
    }

    public override void Attack_Reset()
    {
        // ���� ����
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        if (explosionCoroutine != null)
            StopCoroutine(explosionCoroutine);

        // ����Ʈ ����
        foreach (GameObject vfx in upperSlashVFX)
        {
            vfx.SetActive(false);
        }

        // ����Ʈ ����
        Attack_ColliderReset();
    }
}
