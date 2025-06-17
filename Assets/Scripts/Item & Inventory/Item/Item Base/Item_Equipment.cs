using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item Equipment", menuName = "Item/Item Equipment", order = int.MaxValue)]
public class Item_Equipment : Item_Base
{
    [Header("---Equipment Setting---")]
    public EquipmentType equipmentType;
    public Equipment_Status_SO equipment_Status;
    public enum EquipmentType { Weapon, Head, Body, Pants, Shoes, Core }


    public override void Use()
    {
        // 스테이터스 증가
        Player_Manager.instance.status.Equipment_Status_Setting(true, equipment_Status);
    }

    public void Unequip()
    {
        // 스테이터스 감소
        Player_Manager.instance.status.Equipment_Status_Setting(false, equipment_Status);
    }
}
