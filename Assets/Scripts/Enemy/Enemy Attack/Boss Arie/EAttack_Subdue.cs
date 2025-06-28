using System.Collections;
using UnityEngine;
using Easing.Tweening;


public class EAttack_Subdue : Attack_Base
{
    [Header("---Settting---")]
    [SerializeField] private GameObject[] attackVFX;
    [SerializeField] private Transform subduePos;
    [SerializeField] private bool isSubdue;
    [SerializeField] private Attack_Collider_Subdue subdueCollider;

    /*
     * ��¡
     * ���� ��� ����
     * �ݶ��̴� �� �÷��̾ ���� �� - ���� �ƴ� ���
     * ��� ���� ����
     * ���� ��
     * 360�� ��������
    */
    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        enemy.curState = Enemy_Base.State.Attack;
        isSubdue = false;

        // ������ ����
        for (int i = 1; i < value_Normal.Count; i++)
        {
            (bool isCrit, int dam) = enemy.DamageCalculation(value_Normal[i]);
            Skill_Value_SO.Value_Data skillData1 = value_Normal[i].levelValue.GetData(skillLevel);
            value_Normal[i].attackCollider.Damage_Setting(skillData1.type, skillData1.attackEffect, isCrit, skillData1.hitCount, dam, AttackCollider_Controller.Owner.Enemy);
        }

        // ��¡ �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetFloat("AnimValue", 0);
        anim.SetBool("isSubdueCharge", true);
        anim.SetBool("isSubdue", true);

        // ��¡
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 0.75f;
            anim.SetFloat("AnimValue", timer);
            enemy.LookAt(enemy.target, 0);
            yield return null;
        }
        anim.SetFloat("AnimValue", 1);

        // ���� ������
        yield return new WaitForSeconds(0.15f);


        // ��� �ݶ��̴� Ȱ��ȭ
        subdueCollider.gameObject.SetActive(true);
        subdueCollider.hitAction += Hit;
        (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[0]);
        Skill_Value_SO.Value_Data skillData = value_Normal[0].levelValue.GetData(skillLevel);
        subdueCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage, 0.65f);

        // ���� �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isSubdueCharge", false);

        // ���� ���
        anim.SetFloat("AnimValue", 0);
        Vector3 startPos = enemy.transform.position;
        Vector3 endPos = moveDatas[0].movePos.position;
        timer = 0;
        while (timer < 1 && !isSubdue)
        {
            timer += Time.deltaTime * 1.5f;
            anim.SetFloat("AnimValue", timer);
            enemy.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetFloat("AnimValue", 1);
        subdueCollider.Collider_Reset();
        subdueCollider.gameObject.SetActive(false);

        // ���� ��
        if (isSubdue)
        {
            // �ִϸ��̼�
            anim.SetTrigger("Action");
            anim.SetBool("isSubdue", false);
            anim.SetBool("isSubdueAttack", true);

            // �÷��̾� ���� �ִϸ��̼� ȣ��
            PlayerAction_Manager.instance.Subdue(true, subduePos, this.transform);

            //�ִϸ��̼� ���
            while (anim.GetBool("isSubdueAttack"))
            {
                yield return null;
            }
            PlayerAction_Manager.instance.Subdue(false, null, null);
        }
        else
        {
            // ���� ��
            anim.SetTrigger("Action");
            while (anim.GetBool("isSubdue"))
            {
                yield return null;
            }
        }

        // �ݶ��̴� ����Ʈ ����
        for (int i = 1; i < value_Normal.Count; i++)
        {
            value_Normal[i].attackCollider.ListReset();
        }

        enemy.isPatten = false;
    }

    private void Hit()
    {
        isSubdue = true;
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

        // ī�޶� ����
        PlayerAction_Manager.instance.Subdue(false, null, null);

        // ����Ʈ ����
        foreach (GameObject obj in attackVFX)
        {
            obj.SetActive(false);
        }

        // ��� �ݶ��̴� ����
        isSubdue = false;
        subdueCollider.Collider_Reset();
        subdueCollider.gameObject.SetActive(false);

        // ����Ʈ ����
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
