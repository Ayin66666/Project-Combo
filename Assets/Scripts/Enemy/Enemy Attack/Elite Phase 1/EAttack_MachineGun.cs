using DG.Tweening;
using System.Collections;
using UnityEngine;

public class EAttack_MachineGun : Attack_Base
{
    [Header("---Setting---")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject shootVFX;
    [SerializeField] private Transform[] shotPos;
    [SerializeField] private CharacterController controller;


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        enemy.curState = Enemy_Base.State.Attack;

        enemy.LookAt(Player_Manager.instance.gameObject, 0.15f);
        yield return new WaitForSeconds(0.15f);

        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isMachineGunReady", true);
        anim.SetBool("isMachineGun", true);
        anim.SetFloat("MachineGunMovement", 0);

        // 장전 애니메이션 대기
        while (anim.GetBool("isMachineGunReady"))
        {
            yield return null;
        }

        // 이동 + 사격
        float timer = 0;
        float shootTimer = 0;
        while (timer < 5)
        {
            enemy.LookAt(enemy.target, 0);

            // 이동 로직
            anim.SetFloat("MachineGunMovement", timer < 1 ? timer : 1);
            controller.Move(3f * Time.deltaTime * transform.right);

            // 발사
            if (shootTimer >= 0.15f)
            {
                shootTimer = 0;
                Shooting();
            }

            shootTimer += Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        // 이동 애니메이션 종료
        timer = 1;
        while (timer < 1)
        {
            timer -= Time.deltaTime * 2f;
            anim.SetFloat("MachineGunMovement", timer);
            yield return null;
        }

        anim.SetTrigger("Action");
        anim.SetFloat("MachineGunMovement", 0);
        while (anim.GetBool("isMachineGun"))
        {
            yield return null;
        }

        enemy.Delay();
    }


    private void Shooting()
    {
        for (int i = 0; i < shotPos.Length; i++)
        {
            // 발사 이펙트
            Instantiate(shootVFX, shotPos[i].position, Quaternion.identity);

            // 탄환 소환
            GameObject obj = Instantiate(bullet, shotPos[i].position, Quaternion.identity);
            Vector3 lookDir = (enemy.target.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(lookDir);
            obj.transform.DORotateQuaternion(lookRotation, 0);

            // 데미지 셋팅
            Attack_Collider_Shooting obj_Setting = obj.GetComponent<Attack_Collider_Shooting>();
            (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[0]);
            Skill_Value_SO.Value_Data skillData = value_Normal[0].levelValue.GetData(skillLevel);
            obj_Setting.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);

            // 이동 셋팅
            Vector3 shootPos = Player_Manager.instance.transform.position - shotPos[i].position;
            shootPos.y += 1;
            obj_Setting.Movement_Setting(shootPos, 5f, 10);
        }
    }


    public override void AttackVFX(int index)
    {
        throw new System.NotImplementedException();
    }

    public override void DamageCal(int index)
    {
        throw new System.NotImplementedException();
    }

    public override void Attack_Reset()
    {
        // 동작 종료
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        // 이펙트 리셋
        shootVFX.SetActive(false);

        // 리스트 리셋
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
