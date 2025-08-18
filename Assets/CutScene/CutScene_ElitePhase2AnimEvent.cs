using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutScene_ElitePhase2AnimEvent : MonoBehaviour
{
    [SerializeField] private CutScene_ElitePhase2End elite;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    public void Shooting()
    {
        elite.Shooting();
    }

    public void AttackVFXE()
    {
        elite.AttackVFXE();
    }
}
