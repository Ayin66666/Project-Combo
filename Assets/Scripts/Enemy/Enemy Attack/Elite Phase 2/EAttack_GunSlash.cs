using System.Collections;
using UnityEngine;
using Easing.Tweening;


public class EAttack_GunSlash : Attack_Base
{
    [Header("---Setting---")]
    [SerializeField] private GameObject[] meleeVFX;
    [SerializeField] private GameObject chargeVFX;

    [SerializeField] private Transform attackMovePos;
    [SerializeField] private Transform rushPos;
    [SerializeField] private Transform shootPos;

    [SerializeField] private GameObject grenade;
    [SerializeField] private Transform[] grendePos;
    private Coroutine movementCoroutine;
    private Coroutine grenadeCoroutine;


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        enemy.curState = Enemy_Base.State.Attack;

        // 데미지 셋팅
        for (int i = 0; i < 3; i++)
        {
            (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[i]);
            Skill_Base.Value_Data skillData = value_Normal[i].levelValue.GetData(skillLevel);
            value_Normal[i].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);
        }

        anim.SetTrigger("Action");
        anim.SetFloat("AnimValue", 0);
        anim.SetBool("isGunslashF", true);
        anim.SetBool("isGunSlash", true);

        // 근접 휘둘기 2타
        enemy.LookAt(enemy.target, 0.1f);
        while (anim.GetBool("isGunslashF"))
        {
            yield return null;
        }

        // 돌진
        anim.SetTrigger("Action");
        while (anim.GetBool("isGunSlash"))
        {
            yield return null;
        }

        // 리스트 리셋
        for (int i = 0; i < 3; i++)
        {
            value_Normal[i].attackCollider.ListReset();
        }

        enemy.isPatten = false;
    }

    public void Movement_N()
    {
        enemy.Attack_Movement(attackMovePos, 2.5f);
    }

    public void Movement()
    {
        movementCoroutine = StartCoroutine(MovementCall());
    }

    private IEnumerator MovementCall()
    {
        // 돌진
        Vector3 startPos = enemy.transform.position;
        Vector3 endPos = rushPos.transform.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.65f;
            anim.SetFloat("AnimValue", EasingFunctions.OutExpo(timer));
            enemy.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetFloat("AnimValue", 1);
        anim.SetBool("isGunSlashbackstep", false);
    }

    public void ChargeVFX(int index)
    {
        chargeVFX.SetActive(index == 0);
    }

    public override void AttackVFX(int index)
    {
        meleeVFX[index].SetActive(true);

        if(index == 2)
        {
            Grenade();
        }
    }

    public void Grenade()
    {
        grenadeCoroutine = StartCoroutine(GrenadeCall());
    }

    private IEnumerator GrenadeCall()
    {
        for (int i = 0; i < grendePos.Length; i++)
        {
            // 수류탄 소환
            GameObject obj = Instantiate(grenade, shootPos.position, Quaternion.identity);
            Attack_Collider_Shooting shoot = obj.GetComponent<Attack_Collider_Shooting>();

            // 직격 데미지 셋팅
            (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[0]);
            Skill_Base.Value_Data skillData = value_Normal[0].levelValue.GetData(skillLevel);
            shoot.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);

            // 폭발 데미지 셋팅
            (isCritical, damage) = enemy.DamageCalculation(value_Normal[4]);
            skillData = value_Normal[4].levelValue.GetData(skillLevel);
            Attack_Collider_AOE explosion = shoot.hitVFX.GetComponent<Attack_Collider_AOE>();
            explosion.Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCritical, skillData.hitCount, damage, 0.05f);

            // 총알 회전
            Vector3 lookDir = (enemy.target.transform.position - obj.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(lookDir);
            obj.transform.rotation = lookRotation;

            // 이동 셋팅
            Vector3 p = shootPos.position;
            Vector3 moveDir = grendePos[i].position - p;
            shoot.Movement_Setting(moveDir.normalized, 15f, 10f);

            yield return new WaitForSeconds(0.025f);
        }
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

        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        if (grenadeCoroutine != null)
            StopCoroutine(grenadeCoroutine);

        // 이펙트 종료
        chargeVFX.SetActive(false);
        foreach(GameObject vfx in meleeVFX)
        {
            vfx.SetActive(false);
        }

        // 리스트 리셋
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
