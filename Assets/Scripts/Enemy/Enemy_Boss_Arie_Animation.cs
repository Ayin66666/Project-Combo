using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss_Arie_Animation : MonoBehaviour
{
    [SerializeField] private Enemy_Boss_Arie enemy;
    private Animator anim;


    private void Awake()
    {
        anim = GetComponent<Animator>();
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


    public void WeaponVFX(int isOn)
    {
        enemy.Weapon_Setting(isOn == 0);
    }

    public void WingVFX(int inOn)
    {
        enemy.Bosster_Setting(inOn == 0);
    }

    public void LookAt(float timer)
    {
        enemy.LookAt(enemy.target, timer);
    }


    // 콤보 슬래쉬
    public void ComboVFX(int index)
    {
        enemy.attackDatas[0].AttackVFX(index);
    }

    public void ComboMovement(int index)
    {
        ((EAttack_ComboSlash)enemy.attackDatas[0]).Movement(index);
    }

    public void ComboCollider(int index)
    {
        int attackCount = index / 10;
        int colliderCount = index % 10;
        enemy.attackDatas[0].value_Normal[attackCount].attackCollider.AttackColliderOn(colliderCount);
    }

    public void ComboSlashOver()
    {
        anim.SetBool("isComboSlash", false);
    }


    // 콤보 어퍼
    public void UpperMovement(int index)
    {
        enemy.attackDatas[1].Attack_Movement(index);
    }

    public void UpperCollider(int index)
    {
        int attackCount = index / 10;
        int colliderCount = index % 10;
        enemy.attackDatas[1].value_Normal[attackCount].attackCollider.AttackColliderOn(colliderCount);
    }

    public void UpperVFX(int index)
    {
        enemy.attackDatas[1].AttackVFX(index);
    }

    public void UpperExplosion(int index)
    {
       ((EAttack_ComboUpper)enemy.attackDatas[1]).UpperExplosion(index);
    }

    public void ComboUpperOver()
    {
        anim.SetBool("isUpperSlash", false);
    }


    // 대쉬 어퍼
    public void Look(float time)
    {
        ((EAttack_DashUpper)enemy.attackDatas[2]).Look(time);
    }

    public void DashUpperMovement()
    {
        enemy.attackDatas[2].Attack_Movement(0);
    }

    public void DashUpperCollider(int index)
    {
        int attackIndex = index / 10;
        int colliderIndex = index % 10;
        enemy.attackDatas[2].value_Normal[attackIndex].attackCollider.AttackColliderOn(colliderIndex);
    }

    public void DashUpperVFX(int index)
    {
        enemy.attackDatas[2].AttackVFX(index);
    }

    public void Explosion_DashUpper(int index)
    {
        ((EAttack_DashUpper)enemy.attackDatas[2]).Explosion(index);
    }

    public void DashUpperOver()
    {
        anim.SetBool("isDashUpper", false);
    }


    // 얼터에고 - 소드 오러
    public void AlterEgo_SwordAuraVFX(int index)
    {
        enemy.attackDatas[3].AttackVFX(index);
    }

    public void AlterEgo_Movement(int index)
    {
        enemy.attackDatas[3].Attack_Movement(index);
    }

    public void AlterEgo_Collider(int index)
    {
        int attackIndex = index / 10;
        int colliderIndex = index % 10;

        enemy.attackDatas[3].value_Normal[attackIndex].attackCollider.AttackColliderOn(colliderIndex);
    }

    public void AlterAgo_UpperExplosion()
    {
        ((EAttack_AlterEgo_SwordAura)enemy.attackDatas[3]).Explosion();
    }

    public void AlterEgo_SwordAura(int index)
    {
        ((EAttack_AlterEgo_SwordAura)enemy.attackDatas[3]).SwordAura(index);
    }

    public void AlterEgo_DoubleSlashOver()
    {
        anim.SetBool("isAlterEgoAttack", false);
    }

    public void AlterEgoOver()
    {
        anim.SetBool("isAlterEgo", false);
    }


    // 러쉬
    public void RushVFX(int index)
    {
        enemy.attackDatas[4].AttackVFX(index);
    }

    public void RushCollider(int index)
    {
        int attackIndex = index / 10;
        int colliderIndex = index % 10;

        enemy.attackDatas[4].value_Normal[attackIndex].attackCollider.AttackColliderOn(colliderIndex);
    }

    public void RushMovement(int index)
    {
        enemy.attackDatas[4].Attack_Movement(index);
    }

    public void RushExplosion()
    {
        ((EAttack_Rush)enemy.attackDatas[4]).SwordAura();
    }

    public void RushOver()
    {
        anim.SetBool("isRush", false);
    }


    // 차지 슬래쉬
    public void ChargeSlashVFX(int index)
    {
        enemy.attackDatas[5].AttackVFX(index);
    }

    public void ChargeSlashCollider(int index)
    {
        int attackIndex = index / 10;
        int colliderIndex = index % 10;

        enemy.attackDatas[5].value_Normal[attackIndex].attackCollider.AttackColliderOn(colliderIndex);
    }

    public void ChargeSlashMovement(int index)
    {
        enemy.attackDatas[5].Attack_Movement(index);
    }

    public void ChargeSlashExplosion(int index)
    {
        ((EAttack_ChargeSlash)enemy.attackDatas[5]).Explosion(index);
    }

    public void ChargeSlashSwordAura()
    {
        ((EAttack_ChargeSlash)enemy.attackDatas[5]).Shoot();
    }

    public void ChargeSlashOver()
    {
        anim.SetBool("isChargeSlash", false);
    }


    // 에너지 오브
    public void EnergyOrbVFX()
    {
        enemy.attackDatas[6].AttackVFX(0);
    }

    public void EnergyOrbSpawn()
    {
        ((EAttack_EnergyOrb)enemy.attackDatas[6]).Spawn();
    }

    public void EnergyOrbShoot()
    {
        ((EAttack_EnergyOrb)enemy.attackDatas[6]).Shoot();
    }

    public void EnergyOrbOver()
    {
        anim.SetBool("isEnergyOrb", false);
    }


    // 스트라이크
    public void StrikeVFX(int index)
    {
        enemy.attackDatas[7].AttackVFX(index);
    }

    public void StrikeMovement()
    {
        ((EAttack_Strike)enemy.attackDatas[7]).StrikeMovement();
    }

    public void StrikeExplosion(int index)
    {
        ((EAttack_Strike)enemy.attackDatas[7]).Explosion(index);
    }

    public void StrikeAttackOver()
    {
        anim.SetBool("isStrikeAttack", false);
    }

    public void StrikeOver()
    {
        anim.SetBool("isStrike", false);
    }


    // 제압
    public void SubdueVFX(int index)
    {
        enemy.attackDatas[8].AttackVFX(index);
    }

    public void SubdueCollider(int index)
    {
        int attackIndex = index / 10;
        int colliderIndex = index % 10;

        enemy.attackDatas[8].value_Normal[attackIndex].attackCollider.AttackColliderOn(colliderIndex);
    }

    public void SubdueMovement(int index)
    {
        // 보스 이동
        enemy.attackDatas[8].Attack_Movement(index);
    }

    public void SubdueOver()
    {
        anim.SetBool("isSubdue", false);
    }

    public void SubdueAttackOver()
    {
        anim.SetBool("isSubdueAttack", false);
    }


    // 스페셜
    public void SpecialJumpOver()
    {
        anim.SetBool("isSpecialJump", false);
    }

    public void SpecialOver()
    {
        anim.SetBool("isSpecial", false);
    }
}
