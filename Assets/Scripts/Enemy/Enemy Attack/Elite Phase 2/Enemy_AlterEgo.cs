using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy_AlterEgo : MonoBehaviour
{
    [Header("---Damage Setting---")]
    [SerializeField] private IDamageSysteam.DamageType damageType;
    [SerializeField] private IDamageSysteam.HitVFX hitVFX;
    [SerializeField] private bool isCritical;
    [SerializeField] private int hitCount;
    [SerializeField] private int damage;


    [Header("---Prefabs---")]
    [SerializeField] private GameObject alterSpawnVFX;
    [SerializeField] private GameObject shootVFX;
    [SerializeField] private GameObject bullet;

    [Header("---Component---")]
    [SerializeField] private Animator anim;
    [SerializeField] private LineRenderer line;
    [SerializeField] private Transform shotPos;
    [SerializeField] private GameObject target;


    /// <summary>
    /// ������ & Ÿ�� ����
    /// </summary>
    /// <param name="dType"></param>
    /// <param name="hVFX"></param>
    /// <param name="isCri"></param>
    /// <param name="count"></param>
    /// <param name="dam"></param>
    /// <param name="target"></param>
    public void Damage_Setting(IDamageSysteam.DamageType dType, IDamageSysteam.HitVFX hVFX, bool isCri, int count, int dam, GameObject target)
    {
        damageType = dType;
        hitVFX = hVFX;
        isCritical = isCri;
        hitCount = count;
        damage = dam;
        this.target = target;
    }

    public void Attack()
    {
        StartCoroutine(AttackCall());
    }

    private IEnumerator AttackCall()
    {
        // �н� ��ȯ ����Ʈ
        alterSpawnVFX.SetActive(true);

        // �ٶ󺸱�
        Vector3 lookDir = (target.transform.position - transform.position).normalized;
        lookDir.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(lookDir);
        transform.rotation = lookRotation;

        // ����
        line.enabled = true;
        line.SetPosition(0, shotPos.position);
        float timer = 0;
        while( timer < 1)
        {
            // �ٶ󺸱�
            lookDir = (target.transform.position - transform.position).normalized;
            lookDir.y = 0;
            lookRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = lookRotation;

            line.SetPosition(1, target.transform.position);
            timer += Time.deltaTime;
            yield return null;
        }
        line.enabled = false;

        // �߻�
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        while (anim.GetBool("isAttack"))
        {
            yield return null;
        }

        // �н� �ı�
        alterSpawnVFX.transform.parent = null;
        ParticleSystem  ps = alterSpawnVFX.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.stopAction = ParticleSystemStopAction.Destroy;

        alterSpawnVFX.SetActive(true);
        Destroy(gameObject);
    }

    public void Shoot()
    {
        //�߻� ����Ʈ
        Instantiate(shootVFX, shotPos.position, Quaternion.identity);

        // ź �߻�
        GameObject obj = Instantiate(this.bullet, shotPos.position, Quaternion.identity);
        Vector3 lookDir = (target.transform.position - obj.transform.position).normalized;
        lookDir.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(lookDir);
        obj.transform.rotation = lookRotation;

        // ������ ����
        Attack_Collider_Shooting bullet = obj.GetComponent<Attack_Collider_Shooting>();
        bullet.Damage_Setting(damageType, hitVFX, isCritical, hitCount, damage);
        bullet.hitVFX.GetComponent<Attack_Collider_AOE>().Damage_Setting(damageType, hitVFX, Attack_Collider_AOE.AttackType.SingleHit, isCritical, hitCount, damage, 0.05f);
        
        // �̵� ����
        Vector3 moveDir = target.transform.position - shotPos.position;
        moveDir.y += 0.5f;
        bullet.Movement_Setting(moveDir.normalized, 30f, 10f);
    }
}
