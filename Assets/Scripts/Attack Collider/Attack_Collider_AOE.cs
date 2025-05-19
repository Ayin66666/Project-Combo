using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Collider_AOE : MonoBehaviour
{
    [Header("---Damage Setting---")]
    [SerializeField] private IDamageSysteam.DamageType damageType;
    [SerializeField] private IDamageSysteam.HitVFX hitEffect;
    [SerializeField] private bool isCritical;
    [SerializeField] private int hitCount;
    [SerializeField] private int damage;


    [Header("---Collider Setting---")]
    [SerializeField] private ColliderOwner owner;
    [SerializeField] private AttackType attackType;
    [SerializeField] private Collider attackCollider;
    [SerializeField] private float delayTime;
    [SerializeField] private bool isDelay;
    [SerializeField] private GameObject hitVFX;
    private enum ColliderOwner { Player, Enemy }
    public enum AttackType { SingleHit, multipleHit }

    private void OnEnable()
    {
        attackCollider.enabled = true;

        if (attackType == AttackType.SingleHit)
            StartCoroutine(Delay_Collider(delayTime));
    }

    public void Damage_Setting(IDamageSysteam.DamageType dType, IDamageSysteam.HitVFX hType, AttackType aType, bool isCri, int attackCount, int damage, float delayTime)
    {
        damageType = dType;
        hitEffect = hType;
        isCritical = isCri;
        hitCount = attackCount;
        this.damage = damage;
        this.delayTime = delayTime;
        attackType = aType;
    }

    /// <summary>
    /// 콜라이더 재활성화 딜레이
    /// </summary>
    /// <returns></returns>
    private IEnumerator Delay()
    {
        attackCollider.enabled = false;
        isDelay = true;

        float timer = delayTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        attackCollider.enabled = true;
        isDelay = false;
    }

    /// <summary>
    /// timer 만큼 콜라이더가 활성화
    /// </summary>
    /// <param name="timer"></param>
    /// <returns></returns>
    private IEnumerator Delay_Collider(float timer)
    {
        // 콜라이더 활성화 시간 대기
        yield return new WaitForSeconds(timer);

        // 콜라이더 종료
        attackCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(owner == ColliderOwner.Player ? "Enemy" : "Player") && !isDelay)
        {
            // 데미지
            other.GetComponent<IDamageSysteam>().Take_Damage(gameObject, damageType, hitEffect, isCritical, hitCount, damage);

            // 피격 이펙트
            if (hitVFX != null)
                Instantiate(hitVFX, other.transform.position, Quaternion.identity);

            // 딜레이 기능 동작
            switch (attackType)
            {
                case AttackType.SingleHit:
                    isDelay = true;
                    attackCollider.enabled = false;
                    break;

                case AttackType.multipleHit:
                    StartCoroutine(Delay());
                    break;
            }
        }
    }
}
