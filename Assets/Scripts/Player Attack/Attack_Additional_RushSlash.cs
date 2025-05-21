using System;
using System.Collections;
using UnityEngine;

public class Attack_Additional_RushSlash : Attack_Base
{
    [Header("---RushSlash Setting---")]
    [SerializeField] private GameObject[] attackVFX;
    [SerializeField] private GameObject rushSlashCollider;


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        Player_Manager.instance.MovementLock(cancelType, true);
        Player_Manager.instance.Animation_Reset();
        Player_Manager.instance.isAttack = true;

        // ������ ���
        for (int i = 0; i < value_Normal.Count; i++)
        {
            DamageCal(i);
        }

        // �ݶ��̴� ����
        Player_Manager.instance.Collider_Ignore(true);

        // ���� ����
        Player_Manager.instance.LookAt();
        RushCollider(true);
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isAdditonalRush", true);
        while (anim.GetBool("isAttack"))
        {
            yield return null;
        }

        Player_Manager.instance.isAttack = false;

        // �̵� ���
        float timer = 0;
        while(anim.GetBool("isAdditonalRush"))
        {
            timer += Time.deltaTime;
            if(timer > time && Input_Manager.instance.movementInput.magnitude > 0)
            {
                anim.SetBool("isAdditonalRush", false);
                break;
            }
            yield return null;
        }

        // �ݶ��̴� ����
        Player_Manager.instance.Collider_Ignore(false);
        RushCollider(false);

        // ���� �ݶ��̴� ����
        Attack_ColliderReset();

        Player_Manager.instance.MovementLock(cancelType, false);
        Player_Manager.instance.AttackOver();
    }

    private void RushCollider(bool isOn)
    {
        Attack_Collider_AOE aoe = rushSlashCollider.GetComponent<Attack_Collider_AOE>();

        (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[0]);
        Skill_Value_SO.Value_Data skillData = value_Normal[0].levelValue.GetData(skillLevel);
        aoe.Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.multipleHit, isCritical, skillData.hitCount, damage, 3f);
        
        rushSlashCollider.SetActive(isOn);
    }

    public override void AttackVFX(int index)
    {
        attackVFX[index].SetActive(true);
    }

    public override void DamageCal(int index)
    {
        Skill_Value_SO.Value_Data skillData;
        if (Player_Manager.instance.isAwakning)
        {
            (bool isCritical, int damage) = Player_Manager.instance.DamageCalculation(value_Awakening[0], skillLevel);
            skillData = value_Awakening[0].levelValue.GetData(skillLevel);

            if (value_Awakening[0].attackCollider != null)
                value_Awakening[0].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);
        }
        else
        {
            (bool isCritical, int damage) = Player_Manager.instance.DamageCalculation(value_Normal[0], skillLevel);
            skillData = value_Normal[0].levelValue.GetData(skillLevel);

            if (value_Awakening[0].attackCollider != null)
                value_Normal[0].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);
        }
    }

    public override void Attack_Reset()
    {
        // ���� ����
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        // ����Ʈ ����
        foreach(GameObject vfx in attackVFX)
        {
            vfx.SetActive(false);
        }

        // ����Ʈ ����
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
