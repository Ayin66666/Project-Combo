using System.Collections;
using UnityEngine;
using Easing.Tweening;


public class EAttack_Rush : Attack_Base
{
    [Header("---Setting---")]
    [SerializeField] private GameObject chargeVFX;
    [SerializeField] private GameObject[] attackVFX;
    [SerializeField] private GameObject explosionBulletVFX;
    [SerializeField] private Transform shootPos;
    [SerializeField] private Transform[] explosionPos;


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        enemy.curState = Enemy_Base.State.Attack;

        for (int i = 0; i < 3; i++)
        {
            (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[i]);
            Skill_Value_SO.Value_Data skillData = value_Normal[i].levelValue.GetData(skillLevel);
            value_Normal[i].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);
        }

        // ��¡
        anim.SetTrigger("Action");
        anim.SetBool("isRushCharge", true);
        anim.SetBool("isRushMove", true);
        anim.SetBool("isRush", true);

        chargeVFX.SetActive(true);
        float timer = 0;
        while (timer < 1)
        {
            enemy.LookAt(enemy.target, 0);
            timer += Time.deltaTime;
            yield return null;
        }
        chargeVFX.SetActive(false);

        // ����
        anim.SetTrigger("Action");
        anim.SetBool("isRushCharge", false);
        anim.SetFloat("AnimValue", 0);
        Vector3 startPos = enemy.transform.position;
        Vector3 endPos = moveDatas[0].movePos.position;
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * moveDatas[0].moveSpeed;
            anim.SetFloat("AnimValue", timer);
            enemy.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetBool("isRushMove", false);

        // �������� 2��Ÿ
        enemy.LookAt(enemy.target, 0);
        while (anim.GetBool("isRush"))
        {
            yield return null;
        }

        for (int i = 0; i < 3; i++)
        {
            value_Normal[i].attackCollider.ListReset();
        }

        enemy.isPatten = false;
    }


    public void SwordAura()
    {
        for (int i = 0; i < explosionPos.Length; i++)
        {
            // ���� ��ȯ
            GameObject obj = Instantiate(explosionBulletVFX, shootPos.transform.position, Quaternion.identity);
            Attack_Collider_Shooting shoot = obj.GetComponent<Attack_Collider_Shooting>();

            // ���� ������
            (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[3]);
            Skill_Value_SO.Value_Data skillData = value_Normal[3].levelValue.GetData(skillLevel);
            shoot.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);

            // ���� ������
            (isCritical, damage) = enemy.DamageCalculation(value_Normal[4]);
            skillData = value_Normal[4].levelValue.GetData(skillLevel);
            shoot.hitVFX.GetComponent<Attack_Collider_AOE>().Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCritical, skillData.hitCount, damage, 0.05f);

            // �̵� ����
            Vector3 moveDir = explosionPos[i].position - obj.transform.position;
            shoot.Movement_Setting(moveDir.normalized, 15, 10);

            // �ٶ󺸱�
            Quaternion lookRotation = Quaternion.LookRotation(moveDir.normalized);
            obj.transform.rotation = lookRotation;
        }
    }

    public override void AttackVFX(int index)
    {
        attackVFX[index].SetActive(true);
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
        chargeVFX.SetActive(false);
        foreach (GameObject obj in attackVFX)
        {
            obj.SetActive(false);
        }

        // ����Ʈ ����
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
