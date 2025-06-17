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
        // �������ͽ� ����
        Player_Manager.instance.status.Equipment_Status_Setting(true, equipment_Status);
    }

    public void Unequip()
    {
        // �������ͽ� ����
        Player_Manager.instance.status.Equipment_Status_Setting(false, equipment_Status);
    }
}
