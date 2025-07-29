using Easing.Tweening;
using System.Collections;
using UnityEngine;


public class EAttack_MissileCall : Attack_Base
{
    [Header("---Setting---")]
    [SerializeField] private Enemy_Elite_Phase2 elite;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject shootVFX;
    [SerializeField] private Transform shootPos;

    [SerializeField] private GameObject mechSpawnVFX;
    [SerializeField] private Transform mechSpawnPos;
    [SerializeField] private GameObject mech;
    private GameObject mechObj;
    [SerializeField] private int attackCount;


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        enemy.curState = Enemy_Base.State.Attack;

        // 스폰 이펙트 - 신호탄?
        mechSpawnVFX.SetActive(true);

        // 기체 호출 - 미사일 난사
        MechSpawn();
        enemy.LookAt(enemy.target, 0.15f);

        anim.SetTrigger("Action");
        anim.SetFloat("AnimValue", 0);
        anim.SetBool("isMisslieReady", true);
        anim.SetBool("isMisslieCall", true);
        while (anim.GetBool("isMisslieReady"))
        {
            yield return null;
        }

        // 총기 난사 - 조준
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.25f;
            enemy.LookAt(enemy.target, 0);
            anim.SetFloat("AnimValue", EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetFloat("AnimValue", 1);

        // 딜레이
        yield return new WaitForSeconds(0.1f);

        // 총기 난사
        for (int i = 0; i < attackCount; i++)
        {
            // 총기 난사 - 공격
            anim.SetTrigger("Action");
            anim.SetBool("isMisslieShotgun", true);
            while (anim.GetBool("isMisslieShotgun"))
            {
                enemy.LookAt(enemy.target, 0);
                yield return null;
            }

            yield return new WaitForSeconds(0.05f);
        }

        // 사운드
        elite.sound.Sound(Enemy_Elite_Phase2.SoundKey.Misslie_Off.ToString());

        // 총기 난사 - 종료
        anim.SetTrigger("Action");
        anim.SetBool("isMisslieShotgun", false);
        while (anim.GetBool("isMisslieCall"))
        {
            yield return null;
        }

        enemy.isPatten = false;
    }

    public void MechSpawn()
    {
        // 사운드
        elite.sound.Sound(Enemy_Elite_Phase2.SoundKey.Misslie_Charge.ToString());

        // 메카닉 소환
        GameObject obj = Instantiate(mech, mechSpawnPos.position, Quaternion.identity);
        Enemy_MisslieCall_Mech mech_Misslie = obj.GetComponent<Enemy_MisslieCall_Mech>();
        mechObj = obj;

        // 데미지 셋팅 - 미사일
        (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[0]);
        Skill_Value_SO.Value_Data skillData = value_Normal[0].levelValue.GetData(skillLevel);
        mech_Misslie.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, enemy.target);

        // 데미지 셋팅 - 미사일 폭발
        (isCritical, damage) = enemy.DamageCalculation(value_Normal[1]);
        skillData = value_Normal[1].levelValue.GetData(skillLevel);
        mech_Misslie.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, enemy.target);

        // 동작
        mech_Misslie.Use();
    }

    public override void AttackVFX(int index)
    {
        // 발사 이펙트
        Instantiate(shootVFX, shootPos.position, shootPos.rotation);

        // 사운드
        elite.sound.Sound(Enemy_Elite_Phase2.SoundKey.Misslie_GunShoot.ToString());

        // 총알 소환
        GameObject obj = Instantiate(bullet, shootPos.position, Quaternion.identity);
        Attack_Collider_Shooting shoot = obj.GetComponent<Attack_Collider_Shooting>();

        // 총알 회전
        Vector3 lookDir = (enemy.target.transform.position - obj.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDir);
        obj.transform.rotation = lookRotation;

        // 데미지 셋팅 - 탄 직격
        (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[2]);
        Skill_Value_SO.Value_Data skillData = value_Normal[2].levelValue.GetData(skillLevel);
        shoot.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);

        // 데미지 셋팅 - 탄 폭발
        (isCritical, damage) = enemy.DamageCalculation(value_Normal[3]);
        skillData = value_Normal[3].levelValue.GetData(skillLevel);
        shoot.hitVFX.GetComponent<Attack_Collider_AOE>().Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCritical, skillData.hitCount, damage, 0.05f);

        // 이동 셋팅
        Vector3 moveDir = enemy.target.transform.position - shootPos.position;
        shoot.Movement_Setting(moveDir.normalized, 30f, 10f);
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

        // 이펙트 종료
        mechSpawnVFX.SetActive(false);

        // 메카 종료
        mechObj.GetComponent<Enemy_MisslieCall_Mech>().Groggy();

        // 리스트 리셋
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
