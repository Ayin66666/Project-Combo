using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Collider_Subdue : MonoBehaviour
{
    [Header("---Status---")]
    [SerializeField] private AttackOwner attackOwner;
    private enum AttackOwner { Player, Enemy }
    public System.Action hitAction;
    public bool isUsed;

    [Header("---Damage Status---")]
    [SerializeField] private IDamageSysteam.DamageType damageType;
    [SerializeField] private IDamageSysteam.HitVFX hitType;
    [SerializeField] private int damage;
    [SerializeField] private int attackCount;
    [SerializeField] private bool isCritical;
    private HashSet<GameObject> hitObjects = new HashSet<GameObject>();


    public void Damage_Setting(IDamageSysteam.DamageType damageType, IDamageSysteam.HitVFX hitType, bool isCritical, int attackCount, int damage, float activateTime)
    {
        this.damageType = damageType;
        this.hitType = hitType;
        this.isCritical = isCritical;
        this.attackCount = attackCount;
        this.damage = damage;
    }

    public void Collider_Reset()
    {
        isUsed = false;
        hitAction = null;
        hitObjects.Clear();
    }


    public void TargetCheck(GameObject obj)
    {
        // 타겟 체크
        foreach (GameObject hit in hitObjects)
        {
            // 이미 데미지를 받았다면?
            if (obj == hit)
            {
                return;
            }
        }

        if (obj.GetComponent<IDamageSysteam>() != null)
        {
            // 데미지
            hitObjects.Add(obj);
            Debug.Log($"{obj} : {damageType} / {hitType} / {isCritical} / {attackCount} / {damage}");
            obj.GetComponent<IDamageSysteam>().Take_Damage(gameObject, damageType, hitType, isCritical, attackCount, damage);
        }
        else
        {
            Debug.Log($"공격 불가 / 판정 인터페이스 없음! / 오브젝트 : {obj}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (attackOwner == AttackOwner.Player && other.CompareTag("Enemy"))
        {
            if (hitAction != null && !isUsed)
            {
                hitAction.Invoke();
            }
            TargetCheck(other.gameObject);
        }

        if (attackOwner == AttackOwner.Enemy && other.CompareTag("Player"))
        {
            if (hitAction != null && !isUsed)
            {
                hitAction.Invoke();
            }
            TargetCheck(other.gameObject);
        }
    }
}
