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

        // ���� ����Ʈ - ��ȣź?
        mechSpawnVFX.SetActive(true);

        // ��ü ȣ�� - �̻��� ����
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

        // �ѱ� ���� - ����
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.25f;
            enemy.LookAt(enemy.target, 0);
            anim.SetFloat("AnimValue", EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetFloat("AnimValue", 1);

        // ������
        yield return new WaitForSeconds(0.1f);

        // �ѱ� ����
        for (int i = 0; i < attackCount; i++)
        {
            // �ѱ� ���� - ����
            anim.SetTrigger("Action");
            anim.SetBool("isMisslieShotgun", true);
            while (anim.GetBool("isMisslieShotgun"))
            {
                enemy.LookAt(enemy.target, 0);
                yield return null;
            }

            yield return new WaitForSeconds(0.05f);
        }

        // ����
        elite.sound.Sound(Enemy_Elite_Phase2.SoundKey.Misslie_Off.ToString());

        // �ѱ� ���� - ����
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
        // ����
        elite.sound.Sound(Enemy_Elite_Phase2.SoundKey.Misslie_Charge.ToString());

        // ��ī�� ��ȯ
        GameObject obj = Instantiate(mech, mechSpawnPos.position, Quaternion.identity);
        Enemy_MisslieCall_Mech mech_Misslie = obj.GetComponent<Enemy_MisslieCall_Mech>();
        mechObj = obj;

        // ������ ���� - �̻���
        (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[0]);
        Skill_Value_SO.Value_Data skillData = value_Normal[0].levelValue.GetData(skillLevel);
        mech_Misslie.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, enemy.target);

        // ������ ���� - �̻��� ����
        (isCritical, damage) = enemy.DamageCalculation(value_Normal[1]);
        skillData = value_Normal[1].levelValue.GetData(skillLevel);
        mech_Misslie.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, enemy.target);

        // ����
        mech_Misslie.Use();
    }

    public override void AttackVFX(int index)
    {
        // �߻� ����Ʈ
        Instantiate(shootVFX, shootPos.position, shootPos.rotation);

        // ����
        elite.sound.Sound(Enemy_Elite_Phase2.SoundKey.Misslie_GunShoot.ToString());

        // �Ѿ� ��ȯ
        GameObject obj = Instantiate(bullet, shootPos.position, Quaternion.identity);
        Attack_Collider_Shooting shoot = obj.GetComponent<Attack_Collider_Shooting>();

        // �Ѿ� ȸ��
        Vector3 lookDir = (enemy.target.transform.position - obj.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDir);
        obj.transform.rotation = lookRotation;

        // ������ ���� - ź ����
        (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[2]);
        Skill_Value_SO.Value_Data skillData = value_Normal[2].levelValue.GetData(skillLevel);
        shoot.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);

        // ������ ���� - ź ����
        (isCritical, damage) = enemy.DamageCalculation(value_Normal[3]);
        skillData = value_Normal[3].levelValue.GetData(skillLevel);
        shoot.hitVFX.GetComponent<Attack_Collider_AOE>().Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCritical, skillData.hitCount, damage, 0.05f);

        // �̵� ����
        Vector3 moveDir = enemy.target.transform.position - shootPos.position;
        shoot.Movement_Setting(moveDir.normalized, 30f, 10f);
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

        // ����Ʈ ����
        mechSpawnVFX.SetActive(false);

        // ��ī ����
        mechObj.GetComponent<Enemy_MisslieCall_Mech>().Groggy();

        // ����Ʈ ����
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
