using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AlterEgo_Animaton : MonoBehaviour
{
    [SerializeField] private Enemy_AlterEgo ego;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Shoot()
    {
        ego.Shoot();
    }

    public void AttackOver()
    {
        anim.SetBool("isAttack", false);
    }
}
