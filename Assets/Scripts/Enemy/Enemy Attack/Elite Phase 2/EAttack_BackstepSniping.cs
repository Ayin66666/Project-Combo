using Easing.Tweening;
using System.Collections;
using UnityEngine;


public class EAttack_BackstepSniping : Attack_Base
{
    [Header("---Setting---")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject chargeVFX;
    [SerializeField] private GameObject shootVFX;
    [SerializeField] private Transform backstepPos;
    [SerializeField] private Transform shotPos;
    Vector3 shotDir;

    [Header("---Component---")]
    [SerializeField] private LineRenderer line;


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        enemy.curState = Enemy_Base.State.Attack;

        // �齺��
        anim.SetTrigger("Action");
        anim.SetFloat("AnimValue", 0);
        anim.SetBool("isBackstep", true);
        anim.SetBool("isBackstepSniping", true);

        // �̵�
        Vector3 startPos = enemy.transform.position;
        Vector3 endPos = backstepPos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            enemy.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            anim.SetFloat("AnimValue", EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetFloat("AnimValue", 1);
        anim.SetBool("isBackstep", false);

        // ����
        anim.SetFloat("AnimValue", 0);
        chargeVFX.SetActive(true);
        line.enabled = true;
        float animValue = 0;
        float t = 0;
        timer = 0;
        Vector3 targetVec = enemy.target.transform.position;
        targetVec.y += 1;
        while (timer < Random.Range(1.35f, 1.75f))
        {
            t += Time.deltaTime * 2f;
            timer += Time.deltaTime;

            enemy.LookAt(enemy.target, 0);

            targetVec = enemy.target.transform.position;
            targetVec.y += 1;

            line.SetPosition(0, shotPos.position);
            line.SetPosition(1, animValue > 0.75f ? targetVec : shotPos.position);
            shotDir = targetVec;

            animValue = Mathf.Lerp(0, 1, EasingFunctions.OutExpo(t));
            anim.SetFloat("AnimValue", animValue);
            yield return null;
        }
        line.enabled = false;

        // ���
        anim.SetTrigger("Action");
        while (anim.GetBool("isBackstepSniping"))
        {
            yield return null;
        }

        enemy.isPatten = false;
    }


    public void ChargeVFX(int index)
    {
        chargeVFX.SetActive(index == 0);
    }

    public override void AttackVFX(int index)
    {
        // �߻� ����Ʈ
        Instantiate(shootVFX, shotPos.position, Quaternion.identity);

        // źȯ ��ȯ
        GameObject obj = Instantiate(bullet, shotPos.position, Quaternion.identity);
        Attack_Collider_Shooting shoot = obj.GetComponent<Attack_Collider_Shooting>();

        // ������ ����
        (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[0]);
        Skill_Value_SO.Value_Data skillData = value_Normal[0].levelValue.GetData(skillLevel);
        shoot.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);

        // �̵� ����
        Vector3 moveDir = shotDir - shotPos.position;
        shoot.Movement_Setting(moveDir, 7f, 10f);
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
        shootVFX.SetActive(false);

        // ����Ʈ ����
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
