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
    /// �� ���� �� ������ ��ü ���
    /// </summary>
    /// <param name="slotA">��ü ���� 1��</param>
    /// <param name="slotB">��ü ���� 2��</param>
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
