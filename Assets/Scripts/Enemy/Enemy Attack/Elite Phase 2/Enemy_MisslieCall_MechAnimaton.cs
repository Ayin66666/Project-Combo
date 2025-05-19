using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_MisslieCall_MechAnimaton : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    public void Ready()
    {
        anim.SetBool("isMisslieReady", false);
    }

    public void ShootOver()
    {
        anim.SetBool("isMisslieShooting", false);
    }

    public void End()
    {
        anim.SetBool("isMessile", false);
    }
}
