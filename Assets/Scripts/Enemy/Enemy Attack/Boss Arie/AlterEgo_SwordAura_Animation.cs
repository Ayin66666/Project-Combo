using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlterEgo_SwordAura_Animation : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private AlterEgo_SwordAura enemy;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void AttackVFX()
    {
        enemy.swordAuraVFX.SetActive(true);
    }

    public void SwordAura()
    {
        enemy.Shoot();
    }

    public void AttackOver()
    {
        anim.SetBool("isAttack", false);
    }
}
