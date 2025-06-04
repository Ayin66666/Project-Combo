using System.Collections;
using UnityEngine;


public class EAttack_Shooting : Attack_Base
{
    [Header("---Attack VFX---")]
    [SerializeField] private GameObject shotVFX;
    [SerializeField] private Transform shotPos;
    [SerializeField] private GameObject bullet;


    public override void Use()
    {
        if(useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        enemy.curState = Enemy_Base.State.Attack;

        enemy.LookAt(PlayerAction_Manager.instance.gameObject, 0.1f);
        yield return new WaitForSeconds(0.1f);

        anim.SetTrigger("Action");
        anim.SetBool("isShoot", true);
        while(anim.GetBool("isShoot"))
        {
            yield return null;
        }

        enemy.Delay();
    }

    public override void AttackVFX(int index)
    {
        // �߻� ����Ʈ
        shotVFX.SetActive(true);

        // źȯ ����
        GameObject obj = Instantiate(bullet, shotPos.position, Quaternion.identity);
        Attack_Collider_Shooting shoot = obj.GetComponent<Attack_Collider_Shooting>();

        // ������ ����
        (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[0]);
        Skill_Value_SO.Value_Data skillData = value_Normal[0].levelValue.GetData(skillLevel);
        shoot.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);

        // �̵����� ����
        Vector3 moveDir = enemy.target.transform.position - shotPos.position;
        moveDir.y += 1;
        shoot.Movement_Setting(moveDir.normalized, 10f, 15f);

        // �ٶ󺸱�
        Quaternion lookRotation = Quaternion.LookRotation(moveDir.normalized);
        obj.transform.rotation = lookRotation;
    }

    public override void DamageCal(int index)
    {
        (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[0]);
        Skill_Value_SO.Value_Data skillData = value_Normal[0].levelValue.GetData(skillLevel);
        value_Normal[0].attackCollider.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);
    }

    public override void Attack_Reset()
    {
        // ���� ����
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        // ����Ʈ ����
        shotVFX.SetActive(false);

        // ����Ʈ ����
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (value_Normal[i].attackCollider != null)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
