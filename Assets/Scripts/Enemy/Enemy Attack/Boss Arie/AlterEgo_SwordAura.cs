using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing.Tweening;
using System.Linq;
using static IDamageSysteam;


public class AlterEgo_SwordAura : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private GameObject chargeVFX;
    public GameObject swordAuraVFX;
    [SerializeField] private GameObject dieVFX;

    [SerializeField] private Transform shootPos;
    [SerializeField] private Transform movePos;
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject target;


    [Header("---Damage---")]
    [SerializeField] private IDamageSysteam.DamageType damageType;
    [SerializeField] private IDamageSysteam.HitVFX hitEffect;
    [SerializeField] private bool isCritical;
    [SerializeField] private int hitCount;
    [SerializeField] private int damage;


    [Header("---Damage---")]
    [SerializeField] private LineRenderer line;
    [SerializeField] private Animator anim;
    private Vector3 shootDir;


    public void Damage_Setting(IDamageSysteam.DamageType dType, IDamageSysteam.HitVFX hType, bool isCri, int attackCount, int damage)
    {
        damageType = dType;
        hitEffect = hType;
        isCritical = isCri;
        hitCount = attackCount;
        this.damage = damage;
    }

    public void Movement_Setting(Transform movePos, float speed)
    {
        this.movePos = movePos;
        moveSpeed = speed;
    }

    public void Target_Setting(GameObject target)
    {
        this.target = target;
        StartCoroutine(Use());
    }


    private IEnumerator Use()
    {
        // 이동
        anim.SetFloat("AnimValue", 0);
        Vector3 startPos = transform.position;
        Vector3 endPos = movePos.position;

        Vector3 lookDir = (movePos.transform.position - transform.position).normalized;
        lookDir.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(lookDir);
        transform.rotation = lookRotation;

        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * moveSpeed;
            anim.SetFloat("AnimValue", timer);
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetFloat("AnimValue", 1);

        // 차징
        anim.SetTrigger("Action");
        anim.SetBool("isCharge", true);
        chargeVFX.SetActive(true);
        line.enabled = true;
        timer = 0;
        while(timer < 1)
        {
            shootDir = target.transform.position - shootPos.transform.position;
            line.SetPosition(0, shootPos.position);
            line.SetPosition(1, target.transform.position);
            LookAt();
            timer += Time.deltaTime;
            yield return null;
        }
        chargeVFX.SetActive(false);
        line.enabled = false;
        anim.SetBool("isCharge", false);

        // 검기 발사
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        while (anim.GetBool("isAttack"))
        {
            yield return null;
        }

        // 사망
        dieVFX.transform.parent = null;
        dieVFX.SetActive(true);
        Destroy(gameObject);
    }

    private void LookAt()
    {
        // 바라보기
        Vector3 lookDir = (target.transform.position - transform.position).normalized;
        lookDir.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(lookDir);
        transform.rotation = lookRotation;
    }

    public void Shoot()
    {
        // 검기 소환
        GameObject obj = Instantiate(swordAuraVFX, shootPos.position, Quaternion.identity);
        Attack_Collider_Shooting shoot = obj.GetComponent<Attack_Collider_Shooting>();

        // 바라보기
        Vector3 lookDir = shootDir;
        Quaternion lookRotation = Quaternion.LookRotation(lookDir);
        obj.transform.rotation = lookRotation;


        // 데미지 셋팅
        shoot.Damage_Setting(damageType, hitEffect, isCritical, hitCount, damage);

        // 이동 셋팅
        shootDir.y += 1;
        shoot.Movement_Setting(shootDir.normalized, 40, 10);
    }
}
