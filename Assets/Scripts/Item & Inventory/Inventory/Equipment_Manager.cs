using System.Collections.Generic;
using UnityEngine;


public class Equipment_Manager : MonoBehaviour
{
    [Header("---Equipment Slot---")]
    [SerializeField] private List<SlotData> slots; // �ܺ� ���ÿ�
    private Dictionary<Item_Equipment.EquipmentType, Inventory_Slot_Equipment> itemSlot; // ���� ������ 1
    [SerializeField] private List<SlotData> coreSlot; // ���� ������ 2

    /// <summary>
    /// ��ųʸ� �� ��� ���� ������
    /// </summary>
    [System.Serializable]
    public class SlotData
    {
        [SerializeField] private Item_Equipment.EquipmentType equipmentType;
        public Inventory_Slot_Equipment slot;
        public Item_Equipment.EquipmentType EquipmentType { get { return equipmentType; } private set { equipmentType = value; } }
    }


    private void Start()
    {
        // 1. ��� ���� ������ �Ҵ�
        Dictonary_Setting();

        // 2. ���� ��� ������ �ε�
        // Data_Setting();
    }


    #region ��ũ��Ʈ �ʱ� ���� ����
    /// <summary>
    /// ��ųʸ� ��� ���� ������ �Ҵ�
    /// </summary>
    private void Dictonary_Setting()
    {
        itemSlot = new();
        foreach (SlotData data in slots)
        {
            if (!itemSlot.ContainsKey(data.EquipmentType))
            {
                itemSlot.Add(data.EquipmentType, data.slot);
            }
            else
            {
                Debug.Log("�ߺ� ��� ����");
            }
        }
    }

    /// <summary>
    /// ���� �ε� �� ������ ����
    /// </summary>
    /// <param name="data"></param>
    public void Data_Setting()
    {
        // ������ �ε�
        Data data = SaveLoad_Manager.instance.LoadData(SaveLoad_Manager.instance.curSlot);

        // ������ ����

    }
    #endregion


    #region ��� ���� & ���� ����
    /// <summary>
    /// ��� ���� ����
    /// </summary>
    /// <param name="item"></param>
    public void Equipment(Inventory_Slot slot, Item_Equipment item)
    {
        Debug.Log("Call Equipment");

        if(item.equipmentType == Item_Equipment.EquipmentType.Core)
        {
            // ��ű� ���� - ������ ����Ʈ
            for (int i = 0; i < coreSlot.Count; i++)
            {
                if (!coreSlot[i].slot.haveItem)
                {
                    Debug.Log("Equipment - Core / New");
                    // ������ ����ִٸ� �ش� ���Կ� ����
                    coreSlot[i].slot.Item_Setting(true, item);
                    return;
                }
            }

            // �� ������ ���ٸ� - ù��° ���Կ� ����

            // �������ͽ� ����
            Player_Manager.instance.status.Equipment_Status_Setting(false, coreSlot[0].slot.Item.equipment_Status);

            // ���� ���� ��� �κ��丮�� ����
            Player_Manager.instance.inventory.Item_Change(slot, coreSlot[0].slot.Item);

            // �ű� ��� ����
            Player_Manager.instance.status.Equipment_Status_Setting(true, item.equipment_Status);
            coreSlot[0].slot.Item_Setting(true, item);

            Debug.Log("Equipment - Core/Change");
        }
        else
        {
            // ��� ���� - ��ųʸ� Ű
            if (itemSlot[item.equipmentType].haveItem)
            {
                // ���� ��� O - ��� ���� -> ��� ����
                Debug.Log($"Equipment - {item}");

                // �������ͽ� ����
                Player_Manager.instance.status.Equipment_Status_Setting(false, itemSlot[item.equipmentType].Item.equipment_Status);
               
                // ���� ���� ��� �κ��丮�� ����
                Player_Manager.instance.inventory.Item_Change(slot, itemSlot[item.equipmentType].Item);

                // ����
                Player_Manager.instance.status.Equipment_Status_Setting(true, item.equipment_Status);
                itemSlot[item.equipmentType].Item_Setting(true, item);
            }
            else
            {
                // ���� ��� X - ��� ����
                Debug.Log($"Equipment / New - {item}");

                // ����
                Player_Manager.instance.status.Equipment_Status_Setting(true, item.equipment_Status);
                itemSlot[item.equipmentType].Item_Setting(true, item);
            }
        }
    }

    /// <summary>
    /// ��� ���� ����
    /// </summary>
    /// <param name="index">���� �ε���</param>
    public void EnEquipment(Inventory_Slot_Equipment slot)
    {
        // �κ��丮 �� �� ���� üũ

        if (true)
        {
            // �� �ڸ��� �ִٸ� - ��� ���� �� ���� �̵�

            // �������ͽ� �ʱ�ȭ
            Player_Manager.instance.status.Equipment_Status_Setting(false, slot.Item.equipment_Status);

            // ���� �ʱ�ȭ
            slot.Item_Setting(false, null);
        }
        else
        {
            // �� �ڸ��� ���ٸ� - �����Ұ�
        }
    }
    #endregion
}
