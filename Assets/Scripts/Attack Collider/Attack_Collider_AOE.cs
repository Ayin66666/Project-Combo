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
    /// �ݶ��̴� ��Ȱ��ȭ ������
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
    /// timer ��ŭ �ݶ��̴��� Ȱ��ȭ
    /// </summary>
    /// <param name="timer"></param>
    /// <returns></returns>
    private IEnumerator Delay_Collider(float timer)
    {
        // �ݶ��̴� Ȱ��ȭ �ð� ���
        yield return new WaitForSeconds(timer);

        // �ݶ��̴� ����
        attackCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(owner == ColliderOwner.Player ? "Enemy" : "Player") && !isDelay)
        {
            // ������
            other.GetComponent<IDamageSysteam>().Take_Damage(gameObject, damageType, hitEffect, isCritical, hitCount, damage);

            // �ǰ� ����Ʈ
            if (hitVFX != null)
                Instantiate(hitVFX, other.transform.position, Quaternion.identity);

            // ������ ��� ����
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
