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
    /// 좌-우-중앙 이동베기 + 검기 발사 3(4)타 / 각성 상태에선 중앙 돌려베기 1회 추가 + 중앙 검기 1회 발사
    /// </summary>
    /// <returns></returns>
    private IEnumerator UseCall()
    {
        PlayerAction_Manager.instance.isAttack = true;
        PlayerAction_Manager.instance.isSmash = true;
        PlayerAction_Manager.instance.MovementLock(cancelType, true);
        PlayerAction_Manager.instance.LookAt();
        TeleportPos_Setting();

        // 데미지 계산
        for (int i = 0; i < value_Normal.Count; i++)
        {
            DamageCal(i);
        }

        // 1,2타 공격
        anim.SetTrigger("Smash");
        anim.SetBool("isAttack", true);
        anim.SetBool("isSmash", true);
        while (anim.GetBool("isAttack"))
        {
            yield return null;
        }

        PlayerAction_Manager.instance.isAttack = false;
        PlayerAction_Manager.instance.RushSlash_Setting(true);

        // 추가타 대기
        float timer = 0;
        while (anim.GetBool("isSmash"))
        {
            timer += Time.deltaTime;

            // 추가타
            if (PlayerAction_Manager.instance.isAwakning && Input_Manager.instance.inputDatas[1].isInput)
            {
                PlayerAction_Manager.instance.RushSlash_Setting(false);
                AdditionalAttack();
                yield break;
            }

            // 이동
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

        // 2타 공격
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
    /// 현 위치 기준 이동 위치 셋팅
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

    // 텔레포트
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
        // 이펙트
        Vector3 ppp = PlayerAction_Manager.instance.shootTarget.transform.position - PlayerAction_Manager.instance.bodyObject.transform.position;
        GameObject obj = Instantiate(auraVFX, shotPos.position, PlayerAction_Manager.instance.transform.localRotation);
        obj.transform.rotation = Quaternion.LookRotation(ppp);

        // 투명 투사체
        GameObject auraObj = Instantiate(auraCollider, shotPos.position, PlayerAction_Manager.instance.transform.localRotation);
        Attack_Collider_Shooting objShot = auraObj.GetComponent<Attack_Collider_Shooting>();
        auraObj.transform.rotation = Quaternion.LookRotation(ppp);

        // 투명 투사체 데미지
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
        // 동작 종료
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        // 위치 정상화 -> 아마 이게 2번으로 잡혀서 리셋할때 움직인듯?
        PlayerAction_Manager.instance.bodyObject.transform.position = moveDatas[3].movePos.position;


        // 이펙트 종료
        foreach (GameObject vfx in verticalSlashVFX)
        {
            vfx.SetActive(false);
        }

        // 리스트 리셋
        Attack_ColliderReset();
    }
}
