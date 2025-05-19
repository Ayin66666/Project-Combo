using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing.Tweening;
using DG.Tweening;


public class Attack_Collider_Shooting : MonoBehaviour
{
    [Header("---Status---")]
    [SerializeField] private AttackOwner attackOwner;
    private enum AttackOwner { Player, Enemy }


    [Header("---Damage Status---")]
    [SerializeField] private IDamageSysteam.DamageType damageType;
    [SerializeField] private IDamageSysteam.HitVFX hitType;
    [SerializeField] private int damage;
    [SerializeField] private int attackCount;
    [SerializeField] private bool isCritical;


    [Header("--- Movement Setting ---")]
    [SerializeField] private bool isDestoryByGround;
    [SerializeField] private bool isDestoryByHit;
    [SerializeField] private Vector3 moveDir;
    [SerializeField] private float speed;
    [SerializeField] private float lifeTimer;
    private Coroutine movementCoroutine;

    [Header("---VFX---")]
    public GameObject hitVFX;


    /// <summary>
    /// 데미지 계산 후 해당 함수에 데이터를 넣어 콜라이더에 적용!
    /// </summary>
    /// <param name="damageType"></param>
    /// <param name="hitType"></param>
    /// <param name="isCritical"></param>
    /// <param name="attackCount"></param>
    /// <param name="damage"></param>
    public void Damage_Setting(IDamageSysteam.DamageType damageType, IDamageSysteam.HitVFX hitType, bool isCritical, int attackCount, int damage)
    {
        this.damageType = damageType;
        this.hitType = hitType;
        this.isCritical = isCritical;
        this.attackCount = attackCount;
        this.damage = damage;
    }


    public void Movement_Target(Vector3 endPos, float speed, float delayTime)
    {
        StartCoroutine(TargetMovement(endPos, speed, delayTime));
    }

    private IEnumerator TargetMovement(Vector3 endPos, float speed, float delayTime)
    {
        // 이동방향 바라보기
        Vector3 lookDir = (endPos - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDir);
        transform.DORotateQuaternion(lookRotation, 0);

        // 낙하 대기시간
        yield return new WaitForSeconds(delayTime);

        Vector3 startPos = transform.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(startPos, endPos, timer);
            yield return null;
        }

        Hit();

        Destroy(gameObject);
    }


    public void Movement_Setting(Vector3 moveDir, float moveSpeed, float lifeTime)
    {
        this.moveDir = moveDir;
        speed = moveSpeed;
        lifeTimer = lifeTime;

        movementCoroutine = StartCoroutine(Movement());
    }

    private IEnumerator Movement()
    {
        while (lifeTimer > 0)
        {
            transform.position += moveDir * speed * Time.deltaTime;
            lifeTimer -= Time.deltaTime;
            yield return null;
        }

        Hit();

        Destroy(gameObject);
    }

    private void Hit()
    {
        // 이펙트
        Instantiate(hitVFX, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (attackOwner == AttackOwner.Player && other.CompareTag("Enemy"))
        {
            other.GetComponent<IDamageSysteam>().Take_Damage(gameObject, damageType, hitType, isCritical, attackCount, damage);
            Hit();

            if(isDestoryByHit)
                Destroy(gameObject);
        }

        if (attackOwner == AttackOwner.Enemy && other.CompareTag("Player"))
        {
            other.GetComponent<IDamageSysteam>().Take_Damage(gameObject, damageType, hitType, isCritical, attackCount, damage);
            Hit();

            if (isDestoryByHit)
                Destroy(gameObject);
        }

        if (other.CompareTag("Ground") && isDestoryByGround)
        {
            Hit();
            Destroy(gameObject);
        }
    }
}
