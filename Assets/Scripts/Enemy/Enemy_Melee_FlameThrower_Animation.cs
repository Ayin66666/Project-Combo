using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Melee_FlameThrower_Animation : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private Enemy_Melee_FlameThrower enemy;
    private Animator anim;


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    public void ThrustVFX()
    {
        enemy.attackDatas[0].AttackVFX(0);
    }

    public void FlameVFX(int index)
    {
        enemy.attackDatas[1].AttackVFX(index);
    }


    public void AttackCollider(int index)
    {
        enemy.attackDatas[index].value_Normal[0].attackCollider.AttackColliderOn(0);
    }

    public void ThrustOver()
    {
        anim.SetBool("isNormalAttack", false);
    }

    public void FlameOver()
    {
        anim.SetBool("isFlame", false);
    }
    public void DashOver()
    {
        anim.SetBool("isDash", false);
    }


    public void SpawnOver()
    {
        anim.SetBool("isSpawn", false);
    }

    public void KnockbackOver()
    {
        anim.SetBool("isKnockBack", false);
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
