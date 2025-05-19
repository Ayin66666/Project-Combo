using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Collider : MonoBehaviour
{
    [Header("---State---")]
    [SerializeField] private IDamageSysteam.DamageType damageType;
    [SerializeField] private IDamageSysteam.HitVFX hitType;
    [SerializeField] private bool isCritical;
    [SerializeField] private int hitCont;
    [SerializeField] private int damage;


    [Header("--- Setting ---")]
    public float moveSpeed;
    public float moveTimer;


    public void Status_Setting(IDamageSysteam.DamageType type, bool critical, int damage)
    {
        damageType = type;
        isCritical = critical;
        this.damage = damage;
    }

    public void Shooting(Vector3 moveDir, float speed, float moveTime)
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Player_Manager.instance.Take_Damage(gameObject, damageType, hitType, isCritical, hitCont, damage);
        }
    }
}
