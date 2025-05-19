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

        // ��¡
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

        // ź�� �߻�
        for (int i = 0; i < 10; i++)
        {
            // �߻�
            for (int i1 = 0; i1 < Random.Range(15, 20); i1++)
            {
                // �߻�ü ��ȯ
                GameObject obj = Instantiate(shootVFX[0], shootPos.position, Quaternion.identity);
                Attack_Collider_Shooting shoot = obj.GetComponent<Attack_Collider_Shooting>();

                // ������ ����
                (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[0]);
                Skill_Base.Value_Data skillData = value_Normal[0].levelValue.GetData(skillLevel);
                shoot.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);

                // ���� ������ ����
                (isCritical, damage) = enemy.DamageCalculation(value_Normal[1]);
                skillData = value_Normal[1].levelValue.GetData(skillLevel);
                shoot.hitVFX.GetComponent<Attack_Collider_AOE>().Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCritical, skillData.hitCount, damage, 0.05f);

                // �̵� ����
                Vector3 moveDir = MovePos(targetPosCollider) - shootPos.position;
                shoot.Movement_Setting(moveDir.normalized, Random.Range(15, 30), 15f);

                // �ٶ󺸱�
                Quaternion lookRotation = Quaternion.LookRotation(moveDir.normalized);
                obj.transform.rotation = lookRotation;
            }

            // �߻� ������
            yield return new WaitForSeconds(Random.Range(0.35f, 0.65f));
        }

        // ������ü ��¡
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

        // ���� �ִ�
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
        // ��ü ��ȯ
        energyOrbVFX.transform.position = enemy.transform.position;
        energyOrbVFX.SetActive(true);

        // ��ü �̵�
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

        // ������ ���� - ����
        GameObject big = Instantiate(shootVFX[1], shootPos.position, Quaternion.identity);
        Attack_Collider_Shooting bigShoot = big.GetComponent<Attack_Collider_Shooting>();

        (bool isCri, int dam) = enemy.DamageCalculation(value_Normal[2]);
        Skill_Base.Value_Data skillData = value_Normal[2].levelValue.GetData(skillLevel);
        bigShoot.Damage_Setting(skillData.type, skillData.attackEffect, isCri, skillData.hitCount, dam);

        // ������ ���� - ����
        (isCri, dam) = enemy.DamageCalculation(value_Normal[3]);
        skillData = value_Normal[3].levelValue.GetData(skillLevel);
        bigShoot.hitVFX.GetComponent<Attack_Collider_AOE>().Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCri, skillData.hitCount, dam, 0.05f);

        // �̵� ����
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
        // ���� ����
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        // ������Ʈ ����
        line.enabled = false;

        // ����Ʈ ����
        chargeVFX.SetActive(false);
        attackVFX.SetActive(false);
        energyOrbVFX.SetActive(false);

        // ����Ʈ ����
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
