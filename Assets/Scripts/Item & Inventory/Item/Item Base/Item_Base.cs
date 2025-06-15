using UnityEngine;


public abstract class Item_Base : ScriptableObject
{
    [Header("---UI Setting---")]
    [SerializeField] private Sprite icon;
    [SerializeField] private string itemName;
    [SerializeField, TextArea] private string itemDescription;
    public Sprite Icon { get { return icon; } set { icon = value; } }
    public string ItemName { get { return itemName; } set { itemName = value; } }
    public string ItemDescription { get { return itemDescription; } set { itemName = value; } }


    [Header("---Item Setting---")]
    public Item_Type itemType;
    public Item_Rating itemRating;
    public int itemCode;

    public enum Item_Type { Equipment, Consumable, Other }
    public enum Item_Rating { TierI, TierII, TierIII, TierVI, TierV }


    [Header("---Cooldown Setting---")]
    public string coroutine_Key;


    /// <summary>
    /// 슬롯 & 소트컷에서 호출
    /// </summary>
    public virtual void Use()
    {
        // 내부 구현은 상속받은 스크립트에서 override로!
    }
}
