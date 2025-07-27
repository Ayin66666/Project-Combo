using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Elite_Phase1_Animation : MonoBehaviour
{
    [Header("---Component---")]
    [SerializeField] private Enemy_Elite_Phase1 enemy;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    public void StrikeCollider(int index)
    {
        enemy.attackDatas[0].value_Normal[index].attackCollider.AttackColliderOn(0);
    }

    public void StrikeVFX(int index)
    {
        enemy.attackDatas[0].AttackVFX(index);
    }

    public void StrikeOver()
    {
        anim.SetBool("isGroundStrike", false);
    }


    public void FlameChargeVFX(int index)
    {
        ((EAttack_Flame)enemy.attackDatas[1]).ChargeVFX(index);
    }

    public void FlameVFX(int index)
    {
        enemy.attackDatas[1].AttackVFX(index);
    }

    public void FlameOver()
    {
        anim.SetBool("isFlame", false);
    }

    public void MachineGunReadyOver()
    {
        anim.SetBool("isMachineGunReady", false);
    }
    public void MachineGunOver()
    {
        anim.SetBool("isMachineGun", false);
    }

    
    public void MisslieReadyOver()
    {
        anim.SetBool("isMisslieReady", false);
    }

    public void MisslieShootingOver()
    {
        anim.SetBool("isMisslieShooting", false);
    }

    public void MisslieOver()
    {
        anim.SetBool("isMessile", false);
    }


    public void DownOver()
    {
        anim.SetBool("isDown", false);
    }

    public void DieOver()
    {
        anim.SetBool("isDie", false);
    }
}
