using System.Collections;
using UnityEngine;
using Easing.Tweening;


public class EAttack_EnergyOrb : Attack_Base
{
    [Header("---Setting---")]
    [SerializeField] private GameObject chargeVFX;
    [SerializeField] private GameObject attackVFX;
    [SerializeField] private GameObject energyOrbVFX;
    [SerializeField] private GameObject[] shootVFX;
    [SerializeField] private Transform shootPos;
    [SerializeField] private Collider targetPosCollider;

    [SerializeField] private LineRenderer line;

    [Header("---Movement Setting---")]
    private Vector3 mDir;
    private bool isSpawn;


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        enemy.curState = Enemy_Base.State.Attack;

        // 차징
        isSpawn = true;
        anim.SetTrigger("Action");
        anim.SetBool("isEnergyOrbCharge", true);
        anim.SetBool("isEnergyOrb", true);
        chargeVFX.SetActive(true);
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        while (isSpawn)
        {
            yield return null;
        }

        // 탄막 발사
        for (int i = 0; i < 10; i++)
        {
            // 발사
            for (int i1 = 0; i1 < Random.Range(15, 20); i1++)
            {
                // 발사체 소환
                GameObject obj = Instantiate(shootVFX[0], shootPos.position, Quaternion.identity);
                Attack_Collider_Shooting shoot = obj.GetComponent<Attack_Collider_Shooting>();

                // 데미지 셋팅
                (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[0]);
                Skill_Base.Value_Data skillData = value_Normal[0].levelValue.GetData(skillLevel);
                shoot.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);

                // 직격 데미지 셋팅
                (isCritical, damage) = enemy.DamageCalculation(value_Normal[1]);
                skillData = value_Normal[1].levelValue.GetData(skillLevel);
                shoot.hitVFX.GetComponent<Attack_Collider_AOE>().Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCritical, skillData.hitCount, damage, 0.05f);

                // 이동 셋팅
                Vector3 moveDir = MovePos(targetPosCollider) - shootPos.position;
                shoot.Movement_Setting(moveDir.normalized, Random.Range(15, 30), 15f);

                // 바라보기
                Quaternion lookRotation = Quaternion.LookRotation(moveDir.normalized);
                obj.transform.rotation = lookRotation;
            }

            // 발사 딜레이
            yield return new WaitForSeconds(Random.Range(0.35f, 0.65f));
        }

        // 대형구체 차징
        line.enabled = true;
        timer = 0;
        while (timer < 1)
        {
            enemy.LookAt(enemy.target, 0);
            line.SetPosition(0, shootPos.position);
            line.SetPosition(1, enemy.target.transform.position);
            mDir = enemy.target.transform.position - shootPos.transform.position;
            timer += Time.deltaTime * 1.5f;
            yield return null;
        }
        line.enabled = false;

        // 종료 애니
        chargeVFX.SetActive(false);
        anim.SetBool("isEnergyOrbCharge", false);
        while (anim.GetBool("isEnergyOrb"))
        {
            yield return null;
        }

        enemy.isPatten = false;
    }

    public void Spawn()
    {
        StartCoroutine(SpawnCall());
    }

    private IEnumerator SpawnCall()
    {
        // 구체 소환
        energyOrbVFX.transform.position = enemy.transform.position;
        energyOrbVFX.SetActive(true);

        // 구체 이동
        Vector3 startPos = enemy.transform.position;
        Vector3 endPos = shootPos.transform.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 0.75f;
            energyOrbVFX.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.InOutCubic(timer));
            yield return null;
        }
        energyOrbVFX.transform.position = endPos;
        isSpawn = false;
    }

    public void Shoot()
    {
        energyOrbVFX.SetActive(false);

        // 데미지 셋팅 - 직격
        GameObject big = Instantiate(shootVFX[1], shootPos.position, Quaternion.identity);
        Attack_Collider_Shooting bigShoot = big.GetComponent<Attack_Collider_Shooting>();

        (bool isCri, int dam) = enemy.DamageCalculation(value_Normal[2]);
        Skill_Base.Value_Data skillData = value_Normal[2].levelValue.GetData(skillLevel);
        bigShoot.Damage_Setting(skillData.type, skillData.attackEffect, isCri, skillData.hitCount, dam);

        // 데미지 셋팅 - 폭발
        (isCri, dam) = enemy.DamageCalculation(value_Normal[3]);
        skillData = value_Normal[3].levelValue.GetData(skillLevel);
        bigShoot.hitVFX.GetComponent<Attack_Collider_AOE>().Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCri, skillData.hitCount, dam, 0.05f);

        // 이동 셋팅
        bigShoot.Movement_Setting(mDir.normalized, 10, 30);
    }

    private Vector3 MovePos(Collider coll)
    {
        Vector3 originPosition = coll.transform.position;

        float range_X = coll.bounds.size.x;
        float range_Z = coll.bounds.size.z;

        float random_X = Random.Range(-range_X / 2, range_X / 2);
        float random_Z = Random.Range(-range_Z / 2, range_Z / 2);

        Vector3 randomOffset = new Vector3(random_X, 0f, random_Z);
        Vector3 respawnPosition = originPosition + randomOffset;

        return respawnPosition;
    }

    public override void AttackVFX(int index)
    {
        attackVFX.SetActive(true);
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

        // 컴포넌트 리셋
        line.enabled = false;

        // 이펙트 리셋
        chargeVFX.SetActive(false);
        attackVFX.SetActive(false);
        energyOrbVFX.SetActive(false);

        // 리스트 리셋
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
