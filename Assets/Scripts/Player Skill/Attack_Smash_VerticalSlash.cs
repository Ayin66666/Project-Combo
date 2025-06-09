using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Attack_Smash_VerticalSlash : Attack_Base
{
    [Header("--- VFX ---")]
    [SerializeField] private GameObject[] verticalSlashVFX;
    [SerializeField] private GameObject auraVFX;
    [SerializeField] private GameObject auraCollider;
    [SerializeField] private Transform shotPos;

    [SerializeField] private List<Vector3> teleportPos;

    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    /// <summary>
    /// ��-��-�߾� �̵����� + �˱� �߻� 3(4)Ÿ / ���� ���¿��� �߾� �������� 1ȸ �߰� + �߾� �˱� 1ȸ �߻�
    /// </summary>
    /// <returns></returns>
    private IEnumerator UseCall()
    {
        PlayerAction_Manager.instance.isAttack = true;
        PlayerAction_Manager.instance.isSmash = true;
        PlayerAction_Manager.instance.MovementLock(cancelType, true);
        PlayerAction_Manager.instance.LookAt();
        TeleportPos_Setting();

        // ������ ���
        for (int i = 0; i < value_Normal.Count; i++)
        {
            DamageCal(i);
        }

        // 1,2Ÿ ����
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
        PlayerAction_Manager.instance.isAttack = true;
        PlayerAction_Manager.instance.MovementLock(cancelType, true);
        PlayerAction_Manager.instance.LookAt();

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
                break;
            }

            yield return null;
        }
        PlayerAction_Manager.instance.MovementLock(cancelType, false);
        PlayerAction_Manager.instance.AttackOver();
    }

    /// <summary>
    /// �� ��ġ ���� �̵� ��ġ ����
    /// </summary>
    private void TeleportPos_Setting()
    {
        teleportPos.Clear();
        teleportPos = new List<Vector3>();
        for (int i = 0; i < moveDatas.Length; i++)
        {
            teleportPos.Add(moveDatas[i].movePos.position);
        }
    }

    // �ڷ���Ʈ
    public void Movement(int index)
    {
        PlayerAction_Manager.instance.bodyObject.transform.position = moveDatas[index].movePos.position;
    }


    public override void AttackVFX(int index)
    {
        verticalSlashVFX[index].SetActive(true);
    }

    public void Veritcal_Aura(int index)
    {
        // ����Ʈ
        Vector3 ppp = PlayerAction_Manager.instance.shootTarget.transform.position - PlayerAction_Manager.instance.bodyObject.transform.position;
        GameObject obj = Instantiate(auraVFX, shotPos.position, PlayerAction_Manager.instance.transform.localRotation);
        obj.transform.rotation = Quaternion.LookRotation(ppp);

        // ���� ����ü
        GameObject auraObj = Instantiate(auraCollider, shotPos.position, PlayerAction_Manager.instance.transform.localRotation);
        Attack_Collider_Shooting objShot = auraObj.GetComponent<Attack_Collider_Shooting>();
        auraObj.transform.rotation = Quaternion.LookRotation(ppp);

        // ���� ����ü ������
        Value val = PlayerAction_Manager.instance.isAwakning ? value_Normal[index] : value_Normal[index];
        (bool isCritical, int damage) = PlayerAction_Manager.instance.DamageCalculation(val, skillLevel);

        Skill_Value_SO.Value_Data skillData = val.levelValue.GetData(skillLevel);
        objShot.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);
        objShot.Movement_Setting(ppp, 3f, 0.5f);
    }

    public override void DamageCal(int index)
    {
        Skill_Value_SO.Value_Data skillData;
        if (PlayerAction_Manager.instance.isAwakning)
        {
            (bool isCritical, int damage) = PlayerAction_Manager.instance.DamageCalculation(value_Awakening[index], skillLevel);
            skillData = value_Awakening[index].levelValue.GetData(skillLevel);

            if (value_Awakening[index].attackCollider != null)
                value_Awakening[index].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);
        }
        else
        {
            (bool isCritical, int damage) = PlayerAction_Manager.instance.DamageCalculation(value_Normal[index], skillLevel);
            skillData = value_Normal[index].levelValue.GetData(skillLevel);

            if (value_Awakening[index].attackCollider != null)
                value_Normal[index].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);
        }
    }

    public override void Attack_Reset()
    {
        // ���� ����
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        // ��ġ ����ȭ -> �Ƹ� �̰� 2������ ������ �����Ҷ� �����ε�?
        PlayerAction_Manager.instance.bodyObject.transform.position = moveDatas[3].movePos.position;


        // ����Ʈ ����
        foreach (GameObject vfx in verticalSlashVFX)
        {
            vfx.SetActive(false);
        }

        // ����Ʈ ����
        Attack_ColliderReset();
    }
}
