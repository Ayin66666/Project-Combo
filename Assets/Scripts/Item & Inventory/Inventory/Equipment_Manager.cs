using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment_Manager : MonoBehaviour
{
    [Header("---Settting---")]
    [SerializeField] private List<Inventory_Slot_Equipment> equipmentSlot;

    /// <summary>
    /// ���� �ε� �� ������ ����
    /// </summary>
    /// <param name="data"></param>
    public void Data_Setting(Data data)
    {

    }


    /// <summary>
    /// ��� ���� ����
    /// </summary>
    /// <param name="item"></param>
    public void Equipment(Item_Equipment item)
    {
        switch(item.equipmentType)
        {
            case Item_Equipment.EquipmentType.Weapon:
                break;

            case Item_Equipment.EquipmentType.Head:

                break;
            case Item_Equipment.EquipmentType.Body:

                break;
            case Item_Equipment.EquipmentType.Pants:

                break;
            case Item_Equipment.EquipmentType.Shoes:

                break;
            case Item_Equipment.EquipmentType.Core:
                break;
        }
    }

    /// <summary>
    /// ��� ���� ����
    /// </summary>
    /// <param name="index">���� �ε���</param>
    public void EnEquipment(int index)
    {

    }
}
