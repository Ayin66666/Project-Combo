using System;
using System.Collections.Generic;
using UnityEngine;


public class Equipment_Manager : MonoBehaviour
{
    [Header("---Equipment Slot---")]
    [SerializeField] private List<SlotData> slots; // �ܺ� ���ÿ�
    [SerializeField] private List<SlotData> coreSlot; // ���� ������ 2
    private Dictionary<Item_Equipment.EquipmentType, Inventory_Slot_Equipment> itemSlot; // ���� ������ 1

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

    public Action itemEffect;


    private void Awake()
    {
        // ��� ���� ������ �Ҵ�
        Dictonary_Setting();
    }


    #region ��� ȿ��
    /// <summary>
    /// ���Ǻο� ���� ����Ʈ ȣ�� - ������ Ÿ�ݽø� ����
    /// </summary>
    public void Use_ItemEffect()
    {
        // ���Ǻο� ���� ����Ʈ ȣ��
        itemEffect?.Invoke();
    }

    /// <summary>
    /// ��� ȿ�� �߰�
    /// </summary>
    /// <param name="effect"></param>
    public void Add_ItemEffect(Action effect)
    {
        Debug.Log("����Ʈ �߰�");
        itemEffect += effect;
    }

    /// <summary>
    /// ��� ȿ�� ����
    /// </summary>
    /// <param name="effect"></param>
    public void Remove_ItemEffect(Action effect)
    {
        Debug.Log("����Ʈ ����");
        itemEffect -= effect;
    }
    #endregion


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
    #endregion


    #region ���̺� & �ε� ����
    /// <summary>
    /// ���̺� �� ��� ������ ����
    /// </summary>
    /// <returns></returns>
    public List<int> GetEquipmentData()
    {
        List<int> data = new List<int>();

        // ���
        for (int i = 0; i < slots.Count; i++)
        {
            data.Add(slots[i].slot.haveItem ? slots[i].slot.Item.itemCode : -1);
            Debug.Log(slots[i].slot.haveItem ? slots[i].slot.Item.itemCode : -1);
        }

        // �ھ�
        for (int i = 0; i < coreSlot.Count; i++)
        {
            data.Add(coreSlot[i].slot.haveItem ? coreSlot[i].slot.Item.itemCode : -1);
            Debug.Log(coreSlot[i].slot.haveItem ? coreSlot[i].slot.Item.itemCode : -1);
        }


        for (int i = 0; i < data.Count; i++)
        {
            Debug.Log($"��� ������ üũ {i}��° ������ : {data[i]}");
        }
        return data;
    }

    /// <summary>
    /// ���� �ε� �� ������ ����
    /// </summary>
    /// <param name="data"></param>
    public void Data_Setting()
    {
        // ������ �ε�
        Data data = SaveLoad_Manager.instance.LoadData(SaveLoad_Manager.instance.curSlot);

        // ��� ����
        for (int i = 0; i < 5; i++)
        {
            if(data.equipment[i] != -1)
            {
                Item_Base item = ItemData_Container.instance.FindItem(data.equipment[i]);
                slots[i].slot.Item_Setting(true, (Item_Equipment)item);
            }
        }

        // �ھ� ����
        for (int i = 5; i < 8; i++)
        {
            if (data.equipment[i] != -1)
            {
                Item_Base item = ItemData_Container.instance.FindItem(data.equipment[i]);
                int coreIndex = i - 5;
                coreSlot[coreIndex].slot.Item_Setting(true, (Item_Equipment)item);
            }
        }
    }
    #endregion


    #region ��� ���� & ���� ����
    /// <summary>
    /// ��� ���� ����
    /// </summary>
    /// <param name="item"></param>
    public void Equipment(Inventory_Slot slot, Item_Equipment item)
    {
        if (item.equipmentType == Item_Equipment.EquipmentType.Core)
        {
            // ��ű� ���� - ������ ����Ʈ
            for (int i = 0; i < coreSlot.Count; i++)
            {
                if (!coreSlot[i].slot.haveItem)
                {
                    // ������ ����ִٸ� �ش� ���Կ� ����
                    Debug.Log("Equipment - Core / New");
                    coreSlot[i].slot.Item_Setting(true, item);

                    // �������ͽ� UI �ֽ�ȭ
                    UI_Manager.instance.Status_Setting();
                    return;
                }
            }

            // �� ������ ���ٸ� - ù��° ���Կ� ����
            Debug.Log("Equipment - Core/Change");

            // �������ͽ� ����
            Player_Manager.instance.status.Equipment_Status_Setting(false, coreSlot[0].slot.Item.equipment_Status);

            // ���� ���� ��� �κ��丮�� ����
            Player_Manager.instance.inventory.Item_Change(slot, coreSlot[0].slot.Item);

            // �ű� ��� ����
            Player_Manager.instance.status.Equipment_Status_Setting(true, item.equipment_Status);
            coreSlot[0].slot.Item_Setting(true, item);

            // �������ͽ� UI �ֽ�ȭ
            UI_Manager.instance.Status_Setting();

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

                // �������ͽ� UI �ֽ�ȭ
                UI_Manager.instance.Status_Setting();
            }
            else
            {
                // ���� ��� X - ��� ����
                Debug.Log($"Equipment / New - {item}");

                // ����
                Player_Manager.instance.status.Equipment_Status_Setting(true, item.equipment_Status);
                itemSlot[item.equipmentType].Item_Setting(true, item);

                // �������ͽ� UI �ֽ�ȭ
                UI_Manager.instance.Status_Setting();
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
        if (!Player_Manager.instance.inventory.IsFull(slot.Item))
        {
            // �� �ڸ��� �ִٸ� - ��� ���� �� ���� �̵�

            // �������ͽ� �ʱ�ȭ
            Player_Manager.instance.status.Equipment_Status_Setting(false, slot.Item.equipment_Status);

            // �������ͽ� UI �ֽ�ȭ
            UI_Manager.instance.Status_Setting();

            // �ش� ��� �߰�ȿ���� �ִٸ� ȿ�� ����
            if (slot.Item.haveEffect)
                Player_Manager.instance.equipment.Remove_ItemEffect(slot.Item.Effect);

            // ������ �߰�
            Player_Manager.instance.inventory.Item_Add(slot.Item, 1);

            // ���� �ʱ�ȭ
            slot.Item_Setting(false, null);
        }
        else
        {
            // �� �ڸ��� ���ٸ� - �����Ұ�
            UI_Manager.instance.Item_EquipmentOnOff();
        }
    }
    #endregion
}
