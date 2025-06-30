using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "Item Consumable", menuName = "Item/Item Consumable", order = int.MaxValue)]
public class Item_Consumable : Item_Base
{
    [Header("---Consumable Setting---")]
    [SerializeField] private string key;
    public string Key { get { return key; } private set { key = value; } }

    public ConsumableType consumableType;
    [SerializeField] private float timeCooldown;
    [SerializeField] private float effect_duration;
    [SerializeField] private float heal_interval;
    public enum ConsumableType { oneoff, persistence }


    [Header("---Heal Setting---")]
    [SerializeField] private int healing;
    [SerializeField] private int stamina;
    [SerializeField] private int awakening;


    [Header("---Effect---")]
    [SerializeField] private GameObject recoveryVFX;


    public override void Use()
    {
        switch (consumableType)
        {
            case ConsumableType.oneoff:
                Player_Manager.instance.cooldown.EffectUse(key, OneOff(), timeCooldown);
                break;

            case ConsumableType.persistence:
                Player_Manager.instance.cooldown.EffectUse(key, Persistence(), timeCooldown);
                break;
        }
    }


    #region 회복 로직
    /// <summary>
    /// 즉시 회복 로직
    /// </summary>
    private IEnumerator OneOff()
    {
        // 회복 이펙트
        GameObject obj = Instantiate(recoveryVFX, Player_Manager.instance.Player.transform.position, Quaternion.identity);
        obj.transform.parent = Player_Manager.instance.Player.transform;

        // 회복
        if (healing > 0) 
            Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Hp, healing);

        if (stamina > 0)
            Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Stamina, stamina);

        if (awakening > 0)
            Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Awakening, awakening);

        yield return null;
    }

    /// <summary>
    /// 지속 회복 로직
    /// </summary>
    private IEnumerator Persistence()
    {
        float timer = 0f;
        float intervalTimer = 0f;

        while (timer < effect_duration)
        {
            timer += Time.deltaTime;
            intervalTimer += Time.deltaTime;

            if (intervalTimer >= heal_interval)
            {
                intervalTimer = 0f;

                // 회복 이펙트
                GameObject obj = Instantiate(recoveryVFX, Player_Manager.instance.Player.transform.position, Quaternion.identity);
                obj.transform.parent = Player_Manager.instance.Player.transform;

                // 틱마다 회복
                Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Hp, healing);
                Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Stamina, stamina);
                Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Awakening, awakening);
            }

            yield return null;
        }
    }
    #endregion
}
