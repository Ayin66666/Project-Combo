using UnityEngine;


[CreateAssetMenu(fileName = "Item Consumable", menuName = "Item/Item Consumable", order = int.MaxValue)]
public class Item_Consumable : Item_Base
{
    [Header("---Consumable Setting---")]
    [SerializeField] protected ConsumableType consumableType;
    [SerializeField] private int healing;
    [SerializeField] private int stamina;
    [SerializeField] private int awakening;
    protected enum ConsumableType { oneoff, persistence }


    public override void Use()
    {
        Player_Manager.instance.status.HpAdd(healing);
        Player_Manager.instance.status.StaminaAdd(stamina);
        Player_Manager.instance.status.AwankingAdd(awakening);
    }
}
