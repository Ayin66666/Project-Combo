using System.Collections.Generic;
using UnityEngine;

public class Inventory_Manager : MonoBehaviour
{
    [Header("--- Setting ---")]
    [SerializeField] private List<GameObject> item_Slot;
    public Canvas canvas;


    public void Item_Add()
    {

    }

    /// <summary>
    /// 두 슬롯 간 아이템 교체 기능
    /// </summary>
    /// <param name="slotA">교체 슬롯 1번</param>
    /// <param name="slotB">교체 슬롯 2번</param>
    public void Item_Change(Inventory_Slot slotA, Inventory_Slot slotB)
    {
        Item_Base dataA = slotA.item;
        int countA = slotA.count;

        Item_Base dataB = slotB.item;
        int countB = slotB.count;

        slotA.Slot_Setting(dataB, countB);
        slotB.Slot_Setting(dataA, countA);
    }
}
