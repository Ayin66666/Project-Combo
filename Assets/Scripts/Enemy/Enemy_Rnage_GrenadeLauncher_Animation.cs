using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Rnage_GrenadeLauncher_Animation : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private Enemy_Range_GrenadeLauncher enemy;
    private Animator anim;


    private void Awake()
    {
        anim = GetComponent<Animator>();
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


    public void GrenadeShoot()
    {
        enemy.attackDatas[0].AttackVFX(0);
    }

    public void GrenadeShootOver()
    {
        anim.SetBool("isGrenadeShoot", false);
    }
}
