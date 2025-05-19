using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Animation : MonoBehaviour
{
    [Header("--- Attack Data ---")]
    [SerializeField] private Attack_Base[] comboAttacks;
    [SerializeField] private Attack_Base[] smashAttacks;
    [SerializeField] private Attack_Base[] otherAttacks;
    [SerializeField] private Attack_Other_SpecialSlash speicalAttack;
    [SerializeField] private List<Attack_Base> attacks; // -> 이거 나중에 스매쉬도 일반공격처럼 바꿀것! - 즉 이거 필요없음


    [Header("---Component---")]
    private Animator anim;
    [SerializeField] private GameObject weaponObject;
    [SerializeField] private Transform[] weaponPos;


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    public void WeaponPosChange(int index)
    {
        weaponObject.transform.parent = weaponPos[index];
        weaponObject.transform.localPosition = Vector3.zero;
        weaponObject.transform.localRotation = Quaternion.identity;
    }

    public void AttackOver()
    {
        anim.SetBool("isAttack", false);
    }

    public void ComboOver()
    {
        anim.SetBool("isCombo", false);
        Player_Manager.instance.AttackOver();
    }

    public void SmashOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isSmash", false);
        Player_Manager.instance.AttackOver();
    }

    public void RushSlashOver()
    {
        anim.SetBool("isAdditonalRush", false);
        anim.SetBool("isAttack", false);
        Player_Manager.instance.AttackOver();
    }

    public void CounterOver()
    {
        anim.SetBool("isCounterReady", false);
        anim.SetBool("isCounter", false);
        anim.SetBool("isAttack", false);
        Player_Manager.instance.AttackOver();
    }

    public void SpecialOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isSpecialSlash", false);
    }

    public void DashReset()
    {
        anim.ResetTrigger("Action");
        anim.ResetTrigger("Smash");
        anim.SetBool("isAttack", false);
        anim.SetBool("isSmash", false);
        anim.SetBool("isAdditionalSmash", false);
        Player_Manager.instance.AttackOver();
    }

    public void AdditionalSmashOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isAdditionalSmash", false);
        Player_Manager.instance.AttackOver();
    }

    public void Invincibility(int index)
    {
        Player_Manager.instance.isInvincibility = index == 0;
    }

    public void AwakningOver()
    {
        anim.SetBool("isAwakning", false);
    }

    public void DashOver()
    {
        anim.SetBool("isDodge", false);
    }

    public void DownOver()
    {
        anim.SetBool("isDown", false);
    }

    public void KnockBackOver()
    {
        anim.SetBool("isKnockBack", false);
    }

    public void DieOver()
    {
        anim.SetBool("isDead", false);
    }

    public void RespawnOver()
    {
        anim.SetBool("isRespawn", false);
    }


    public void NormalCollider(int index)
    {
        int attackIndex = index / 10;
        int colliderIndex = index % 10;

        if (Player_Manager.instance.isAwakning && comboAttacks[attackIndex].haveAwakningValue)
        {
            comboAttacks[attackIndex].value_Awakening[0].attackCollider.AttackColliderOn(colliderIndex);
        }
        else
        {
            comboAttacks[attackIndex].value_Normal[0].attackCollider.AttackColliderOn(colliderIndex);
        }
    }

    public void NormalMovement(int index)
    {
        comboAttacks[index].Attack_Movement(0);
    }
    public void NormalVFX(int index)
    {
        comboAttacks[index].AttackVFX(0);
    }


    // 스매쉬
    public void SmashMovement(int index)
    {
        int typeIndex = index / 10;
        int moveIndex = index % 10;
        attacks[typeIndex].Attack_Movement(moveIndex);
    }

    public void SmashCollider(int index)
    {
        int typeIndex = index / 100;
        int countIndex = (index % 10) / 10;
        int colliderIndex = index % 10;

        if (Player_Manager.instance.isAwakning && smashAttacks[typeIndex].haveAwakningValue)
        {
            smashAttacks[typeIndex].value_Awakening[countIndex].attackCollider.AttackColliderOn(colliderIndex);
        }
        else
        {
            smashAttacks[typeIndex].value_Normal[countIndex].attackCollider.AttackColliderOn(colliderIndex);
        }
    }

    // 스매쉬 - 4타 전용
    public void Smash4Collider()
    {
        int count = ((Attack_Smash_ChargeSlash)smashAttacks[3]).chargeCount < 0.5f ? 0 : ((Attack_Smash_ChargeSlash)smashAttacks[3]).chargeCount < 1 ? 1 : 2;
        ((Attack_Smash_ChargeSlash)smashAttacks[3]).DamageCal(count);

        if (Player_Manager.instance.isAwakning)
        {
            smashAttacks[3].value_Awakening[count].attackCollider.AttackColliderOn(0);
        }
        else
        {
            smashAttacks[3].value_Normal[count].attackCollider.AttackColliderOn(0);
        }
    }


    public void UpperVFX(int index)
    {
        smashAttacks[0].AttackVFX(index);
    }

    public void DoubleVFX(int index)
    {
        smashAttacks[1].AttackVFX(index);
    }

    public void DoubleGroundVFX()
    {
        ((Attack_Samsh_DoubleSlash)smashAttacks[1]).Samsh2_GourndVFX();
    }

    public void DoubleAuraVFX()
    {
        if (Player_Manager.instance.isAwakning)
        {
            ((Attack_Samsh_DoubleSlash)smashAttacks[1]).Smash2_Aura();
        }
    }

    public void VerticalVFX(int index)
    {
        smashAttacks[2].AttackVFX(index);
        ((Attack_Smash_VerticalSlash)smashAttacks[2]).Veritcal_Aura(index);
    }

    public void VerticalMovement(int index)
    {
        ((Attack_Smash_VerticalSlash)smashAttacks[2]).Movement(index);
    }


    public void ChargeVFX(int index)
    {
        smashAttacks[3].AttackVFX(index);
    }


    #region Attack - Other
    public void OtherMovement(int index)
    {
        int typeIndex = index / 10;
        int moveIndex = index % 10;
        otherAttacks[typeIndex].Attack_Movement(moveIndex);
    }

    public void OtherCollider(int index)
    {
        int typeIndex = index / 100;
        int countIndex = (index % 100) / 10;
        int colliderIndex = index % 10;

        if (Player_Manager.instance.isAwakning && otherAttacks[typeIndex].haveAwakningValue)
        {
            otherAttacks[typeIndex].value_Awakening[countIndex].attackCollider.AttackColliderOn(colliderIndex);
        }
        else
        {
            otherAttacks[typeIndex].value_Normal[countIndex].attackCollider.AttackColliderOn(colliderIndex);
        }
    }

    public void RushSlashVFX(int index)
    {
        otherAttacks[0].AttackVFX(index);
    }


    // 카운터
    public void CounterVFX(int intdex)
    {
        otherAttacks[1].AttackVFX(intdex);
    }

    public void CounterImpactVFX()
    {
        ((Attack_Other_Counter)otherAttacks[1]).ImpactVFX();
    }


    // 필살기
    public void SpecialSlashMovement(int index)
    {
        speicalAttack.Body_Movement(index);
    }

    public void SpecialCamChange(int index)
    {
        speicalAttack.CameraChange(index);
    }

    public void SpecialVFX(int index)
    {
        Debug.Log($"필살기 공격 이펙트 인덱스 {index}");
        speicalAttack.AttackVFX(index);
    }

    public void SwordAuraVFX(int index)
    {
        speicalAttack.SwordAuraVFX(index);
    }

    public void SpecialCollider(int index)
    {
        speicalAttack.value_Normal[index].attackCollider.AttackColliderOn(0);
    }
    #endregion
}
