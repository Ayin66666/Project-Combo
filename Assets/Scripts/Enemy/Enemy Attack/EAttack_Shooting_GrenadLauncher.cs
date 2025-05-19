using System.Collections;
using UnityEngine;
using Easing.Tweening;


public class EAttack_Shooting_GrenadLauncher : Attack_Base
{
    [Header("---Setting---")]
    [SerializeField] private GameObject shootVFX;
    [SerializeField] private GameObject lockOnVFX;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform shootPos;
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

        anim.SetTrigger("Action");
        anim.SetBool("isGrenadeShootReady", true);
        anim.SetBool("isGrenadeShoot", true);

        // ����
        line.enabled = true;
        lockOnVFX.SetActive(true);
        float timer = 0;
        while (timer < Random.Range(0.95f, 1.15f))
        {
            timer += Time.deltaTime;
            enemy.LookAt(enemy.target, 0);

            // ���ؼ� - �
            LockOnLine();
            lockOnVFX.transform.position = enemy.target.transform.position;
            yield return null;
        }

        // ���� ������
        yield return new WaitForSeconds(0.15f);

        line.enabled = false;
        lockOnVFX.SetActive(false);
        anim.SetBool("isGrenadeShootReady", false);

        // �߻�
        while (anim.GetBool("isGrenadeShoot"))
        {
            yield return null;
        }

        enemy.Delay();
    }

    private void LockOnLine()
    {
        if (line.enabled)
        {
            line.positionCount = 20;
            Vector3 startPoint = shootPos.transform.position;
            Vector3 endPoint = lockOnVFX.transform.position;
            Vector3 controlPoint = (startPoint + endPoint) / 2 + Vector3.up * 2; // �����̴��� ���� ��� ����

            for (int i = 0; i < 20; i++)
            {
                float t = i / (float)(20 - 1);
                Vector3 curvePoint = CalculateQuadraticBezierPoint(t, startPoint, controlPoint, endPoint);
                line.SetPosition(i, curvePoint);
            }
        }
    }

    Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }


    public override void AttackVFX(int index)
    {
        // �߻� ����Ʈ
        shootVFX.SetActive(true);

        // ź ��ȯ
        GameObject obj = Instantiate(bullet, shootPos.position, Quaternion.identity);
        Attack_Collider_Shooting shoot = obj.GetComponent<Attack_Collider_Shooting>();
        Attack_Collider_AOE aoe = shoot.hitVFX.GetComponent<Attack_Collider_AOE>();

        // ������ ���� - ź��
        (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[0]);
        Skill_Base.Value_Data skillData = value_Normal[0].levelValue.GetData(skillLevel);
        shoot.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);

        // ������ ���� - ����
        (isCritical, damage) = enemy.DamageCalculation(value_Normal[1]);
        skillData = value_Normal[0].levelValue.GetData(skillLevel);
        aoe.Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCritical, skillData.hitCount, damage, 0.1f);

        // �̵� ����
        shoot.Movement_Setting(Vector3.zero, 0, 1.5f);
        StartCoroutine(shootMoveCoroutine(obj));
    }

    // �߻�ü ������ �̵�
    private IEnumerator shootMoveCoroutine(GameObject bullet)
    {
        Vector3 startPoint = bullet.transform.position;
        Vector3 endPoint = lockOnVFX.transform.position;
        endPoint.y -= 0.1f;
        Vector3 controlPoint = (startPoint + endPoint) / 2 + Vector3.up * 2;

        float elapsed = 0f;

        while (elapsed < 1 && bullet != null)
        {
            float t = elapsed / 1;
            Vector3 pos = CalculateQuadraticBezierPoint(t, startPoint, controlPoint, endPoint);
            bullet.transform.position = pos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // ������ ��ġ ����
        if (bullet != null)
            bullet.transform.position = endPoint;
    }

    public override void DamageCal(int index)
    {
        throw new System.NotImplementedException();
    }

    public override void Attack_Reset()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        line.enabled = false;
        shootVFX.SetActive(false);
        lockOnVFX.SetActive(false);
    }
}
