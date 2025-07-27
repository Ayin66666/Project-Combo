using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Range_Gunner_Animation : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private Enemy_Range_Gunner enemy;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Shoot()
    {
        enemy.attackDatas[0].AttackVFX(0);
    }

    public void Barrage(int index)
    {
        enemy.attackDatas[index].AttackVFX(0);
    }


    public void ShootOver()
    {
        anim.SetBool("isShoot", false);
    }

    public void BarrageOver()
    {
        anim.SetBool("isBarrage", false);
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
