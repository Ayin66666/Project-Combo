using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item Equipment", menuName = "Item/Item Equipment", order = int.MaxValue)]
public class Item_Equipment : Item_Base
{
    [Header("---Equipment Status---")]
    public EquipmentType equipmentType;
    public List<Equipment_Status> equipment_Status;


    [Header("---Equipment Effect---")]
    public bool haveEffect;
    public List<Item_Effect_SO> effectList;


    [Header("---Epuipment UI---")]
    [TextArea] public string statusText;
    [TextArea] public string damageText;

    [HideInInspector] public string[] typeText = new string[] 
    { 
        "무기", "머리", "상의", "하의", "신발", "코어" 
    };

    [HideInInspector] public string[] effectText = new string[]
    {
        "물리 공격력", "마법 공격력", "치명타 확률", "치명타 피해",
        "최대 체력", "물리 방어력", "마법 방어력",
        "이동 속도", "스테미너 회복력"
    };

    public enum EquipmentType { Weapon, Head, Body, Pants, Shoes, Core }
    public enum StatusType 
    { 
        PDamage, MDamage, Criticalhit, Critical_multiplier, 
        MaxHp, PhysicalDefence, MagicalDefence, 
        MoveSpeed, StaminaRecovery
    };

    [System.Serializable]
    public struct Equipment_Status
    {
        public StatusType type;
        public float value;
    }


    /// <summary>
    /// 장비 착용 - 실제 기능은 Equipment_Manager 에서!
    /// </summary>
    public override void Use()
    {
        // 실제 기능은 Equipment_Manager 에서!
    }


    /// <summary>
    /// 아이템 효과
    /// </summary>
    public void Effect()
    {

    }
}
