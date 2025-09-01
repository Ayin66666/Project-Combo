using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "Item Effect", menuName = "Item Effect/Shockwave", order = int.MaxValue)]
public class Item_Effect_ShockWave : Item_Effect_SO
{
    [Header("---Damage Setting---")]
    [SerializeField] private IDamageSysteam.DamageType damageType;
    [SerializeField] private IDamageSysteam.HitVFX hitVFX;
    [SerializeField] private Vector2Int minMaxDamage;
    [SerializeField] private int hitCount;

    [Header("---Effect---")]
    [SerializeField] private GameObject shockwaveVFX;



    public override void Effect()
    {
        Player_Manager.instance.cooldown.EffectUse(Key, Shockwave(), Cooldown);
    }

    public IEnumerator Shockwave()
    {
        Debug.Log($"Shockwave Call!");
        GameObject obj = Instantiate(shockwaveVFX, Player_Manager.instance.action.bodyObject.transform.position, Quaternion.identity);
        Attack_Collider_AOE aoe = obj.GetComponent<Attack_Collider_AOE>();

        int damage = Random.Range(30, 50);
        aoe.Damage_Setting(damageType, hitVFX, Attack_Collider_AOE.AttackType.SingleHit, false, hitCount, damage, 10);

        yield return null;
    }
}
