using UnityEngine;


public class Enemy_Melee_Axe_Animation : MonoBehaviour
{
    [Header("---Component---")]
    [SerializeField] private Enemy_Melee_Axe enemy;
    private Animator anim;


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SpawnOver()
    {
        anim.SetBool("isSpawn", false);
    }

    #region Movement
    public void Normal_Movement(int index)
    {
        enemy.attackDatas[0].Attack_Movement(index);
    }

    public void Movement_Rush(int index)
    {
        enemy.attackDatas[1].Attack_Movement(index);
    }
    #endregion


    #region Hit
    public void KnockBackOver()
    {
        anim.SetBool("isKnockBack", false);
    }

    public void DownOver()
    {
        anim.SetBool("isDown", false);
    }

    public void DeadOver()
    {
        anim.SetBool("isDie", false);
    }
    #endregion


    #region Attack
    public void NormalCollider()
    {
        enemy.attackDatas[0].value_Normal[0].attackCollider.AttackColliderOn(0);
    }

    public void NormalOver()
    {
        anim.SetBool("isNormalAttack", false);
    }

    public void DashOver()
    {
        anim.SetBool("isDash", false);
    }
    #endregion


    #region VFX
    public void NormalVFX(int index)
    {
        // 사운드
        enemy.sound.Sound(Enemy_Melee_Axe.SoundKey.Swing.ToString());

        // 이펙트
        enemy.attackDatas[0].AttackVFX(index);
    }
    #endregion
}
