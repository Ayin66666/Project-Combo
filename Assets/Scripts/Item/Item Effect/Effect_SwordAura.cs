using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect SwordAura", menuName = "Item Data / Item Effect / Effect SwordAura", order = int.MaxValue)]
public class Effect_SwordAura : Item_EffectBase
{
    [Header("---SwordAura Status---")]
    [SerializeField] private IDamageSysteam.DamageType type;
    [SerializeField] private IDamageSysteam.HitVFX hitVFX;
    [SerializeField] private int damage;
    [SerializeField] private int hitCount;


    [Header("---Prefabs---")]
    [SerializeField] private GameObject aura_Prefab;


    public override void Use(int indx)
    {
        canUse = false;
        Player_Manager.instance.cooldown.Cooldown(key, TimerCall());

        // 검기 소환
        //GameObject obj = Instantiate(aura_Prefab, player_Manager.instnace.shotPos.potion, Quaternion.identity);
        //Attack_Collider_Shooting collider = obj.GetComponent<Attack_Collider_Shooting>();

        // 데미지 셋팅
        //collider.Damage_Setting(type, hitVFX, false, hitCount, damage);

        // 이동 셋팅
        //Vector3 moveDir = player_Manager.instance.gameobject.forward;
        //collider.Movement_Setting(moveDir, 10f, 10f);
    }
}
