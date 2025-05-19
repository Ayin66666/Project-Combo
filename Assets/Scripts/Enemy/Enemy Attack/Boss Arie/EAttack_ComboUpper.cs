using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing.Tweening;


public class EAttack_ComboUpper : Attack_Base
{
    [Header("---Setting---")]
    [SerializeField] private GameObject[] attackVFX;
    [SerializeField] private Transform[] explosionPos;
    [SerializeField] private GameObject explosionVFX;
    private Coroutine explosionCoroutine;


    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        enemy.curState = Enemy_Base.State.Attack;

        // ���� ����Ʈ
        ((Enemy_Boss_Arie)enemy).Weapon_Setting(true);

        // �÷��̾� �Ÿ� üũ
        Vector3 startPos;
        Vector3 endPos;
        float timer;

        // �ָ� ����
        enemy.Check_Target();
        if (enemy.targetRange >= 5)
        {
            enemy.LookAt(enemy.target, 0);
            anim.SetTrigger("Action");
            anim.SetBool("isForwardStep", true);
            anim.SetFloat("AnimValue", 0);

            // ����
            startPos = enemy.transform.position;
            endPos = moveDatas[0].movePos.position;
            timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime * 1.5f;
                anim.SetFloat("AnimValue", timer);
                enemy.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
                yield return null;
            }
            anim.SetFloat("AnimValue", 1);
            anim.SetBool("isForwardStep", false);
        }

        // ������ �ٷ� ���� - ���� 3ȸ
        enemy.LookAt(enemy.target, 0);
        anim.SetTrigger("Action");
        anim.SetBool("isUpperSlash", true);
        anim.SetBool("isUpperBackstep", true);
        while(anim.GetBool("isUpperSlash"))
        {
            yield return null;
        }

        // ������
        timer = 0;
        while(timer < 0.15f)
        {
            enemy.LookAt(enemy.target, 0);
            timer += Time.deltaTime;
            yield return null; 
        }

        // �齺��
        enemy.LookAt(enemy.target, 0);
        anim.SetTrigger("Action");
        anim.SetFloat("AnimValue", 0);
        startPos = enemy.transform.position;
        endPos = moveDatas[2].movePos.position;
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            anim.SetFloat("AnimValue", timer);
            enemy.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetFloat("AnimValue", 1);
        anim.SetBool("isUpperBackstep", false);

        // ��ȭ ���� - ���� 5ȸ
        enemy.LookAt(enemy.target, 0f);
        anim.SetTrigger("Action");
        anim.SetBool("isUpperSlash", true);
        while (anim.GetBool("isUpperSlash"))
        {
            yield return null;
        }

        enemy.isPatten = false;
    }

    public void UpperExplosion(int index)
    {
        explosionCoroutine = StartCoroutine(UpperExplosionCall(index));
    }

    private IEnumerator UpperExplosionCall(int index)
    {
        for (int i = 0; i < (index == 0 ? 3 : 5); i++)
        {
            // ���� ��ȯ
            GameObject obj = Instantiate(explosionVFX, explosionPos[i].position, Quaternion.identity);
            Attack_Collider_AOE aoe = obj.GetComponent<Attack_Collider_AOE>();

            // ������ ����
            int valueIndex = index == 0 ? 2 : 3;
            (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[valueIndex]);
            Skill_Base.Value_Data skillData = value_Normal[valueIndex].levelValue.GetData(skillLevel);
            aoe.Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCritical, skillData.hitCount, damage, 0.05f);

            // ������
            yield return new WaitForSeconds(0.065f);
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

        if (explosionCoroutine != null)
            StopCoroutine(explosionCoroutine);

        // ����Ʈ ����
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
