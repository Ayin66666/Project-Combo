using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Enemy_Elite_Phase2_Animation : MonoBehaviour
{
    [SerializeField] private Enemy_Elite_Phase2 enemy;
    private Coroutine lookCoroutine;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    public void LookAt(float speed)
    {
        if (lookCoroutine != null)
            StopCoroutine(lookCoroutine);

        lookCoroutine = StartCoroutine(LookAtCall(speed));
    }

    private IEnumerator LookAtCall(float speed)
    {
        float timer = 0;
        while (timer < speed)
        {
            enemy.LookAt(enemy.target, 0.1f);
            timer += Time.deltaTime;
            yield return null;
        }
    }


    public void SpawnOver()
    {
        anim.SetBool("isSpawn", false);
    }

    public void DownOver()
    {
        anim.SetBool("isDown", false);
    }

    public void DieOver()
    {
        anim.SetBool("isDie", false);
    }


    // 백스탭 스나이핑
    public void BackstepSnipingChargeVFX(int index)
    {
        ((EAttack_BackstepSniping)enemy.attackDatas[1]).ChargeVFX(index);
    }

    public void BackstepSnipingVFX()
    {
        enemy.attackDatas[1].AttackVFX(0);
    }

    public void BackstepSnipingOver()
    {
        anim.SetBool("isBackstepSniping", false);
    }


    // 트리플 샷 - 이펙트 포함
    public void TripeCollider(int index)
    {
        enemy.attackDatas[2].AttackVFX(index);
    }

    public void TripleMoveOver()
    {
        anim.SetBool("isTripleMove", false);
    }

    public void TripleOver()
    {
        anim.SetBool("isTriple", false);
    }


    // 얼터에고 샷
    public void AlterEgoChargeVFX(int index)
    {
        ((EAttack_AlterEgoShooting)enemy.attackDatas[3]).ChargeVFX(index);
    }

    public void AlterEgoAttack()
    {
        ((EAttack_AlterEgoShooting)enemy.attackDatas[3]).Shoot();
    }

    public void AlterEgoOver()
    {
        anim.SetBool("isAlterAttack", false);
    }


    // 미사일 콜
    public void MisslieCall_ShotGun()
    {
        enemy.attackDatas[4].AttackVFX(0);
    }

    public void MisslieCall_ShotGunOver()
    {
        anim.SetBool("isMisslieShotgun", false);
    }

    public void MisslieReadyOver()
    {
        anim.SetBool("isMisslieReady", false);
    }

    public void MisslieCallOver()
    {
        anim.SetBool("isMisslieCall", false);
    }


    // 건 슬래쉬
    public void GunSlashVFX(int index)
    {
        enemy.attackDatas[0].AttackVFX(index);
    }

    public void GunSlashCollider(int index)
    {
        enemy.attackDatas[0].value_Normal[index].attackCollider.AttackColliderOn(0);
    }

    public void GunSlashMove(int index)
    {
        if(index == 0)
        {
            ((EAttack_GunSlash)enemy.attackDatas[0]).Movement();
        }
        else
        {
            ((EAttack_GunSlash)enemy.attackDatas[0]).Movement_N();
        }
    }

    public void GunslashCharge(int index)
    {
        ((EAttack_GunSlash)enemy.attackDatas[0]).ChargeVFX(index);
    }

    public void GunSlashGrenade()
    {
        ((EAttack_GunSlash)enemy.attackDatas[0]).Grenade();
    }

    public void GunSlash_MeleeOver()
    {
        anim.SetBool("isGunslashF", false);
    }

    public void GunSlash_ShootOver()
    {
        anim.SetBool("isGunSlash", false);
    }
}

