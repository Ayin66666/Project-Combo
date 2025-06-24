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
    public void Equipment(Item_Equipment item)
    {
        if(item.equipmentType == Item_Equipment.EquipmentType.Core)
        {
            // ��ű� ���� - ������ ����Ʈ
            for (int i = 0; i < coreSlot.Count; i++)
            {
                if (!coreSlot[i].slot.haveItem)
                {
                    // ������ ����ִٸ� �ش� ���Կ� ����
                    return;
                }
            }

            // �� ������ ���ٸ� - ù��° ���Կ� ����

            // ���� ����
            Player_Manager.instance.status.Equipment_Status_Setting(false, coreSlot[0].slot.Item.equipment_Status);

            // ����
            Player_Manager.instance.status.Equipment_Status_Setting(true, item.equipment_Status);
            coreSlot[0].slot.Item_Setting(true, item);
        }
        else
        {
            // ��� ���� - ��ųʸ� Ű
            if (itemSlot[item.equipmentType].haveItem)
            {
                // ���� ��� O - ��� ���� -> ��� ����

                // ���� ��� ����
                Player_Manager.instance.status.Equipment_Status_Setting(false, itemSlot[item.equipmentType].Item.equipment_Status);

                // ����
                Player_Manager.instance.status.Equipment_Status_Setting(true, item.equipment_Status);
                itemSlot[item.equipmentType].Item_Setting(true, item);
            }
            else
            {
                // ���� ��� X

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
        // �������ͽ� �ʱ�ȭ
        Player_Manager.instance.status.Equipment_Status_Setting(false, slot.Item.equipment_Status);

        // ���� �ʱ�ȭ
        slot.Item_Setting(false, null);
    }
    #endregion
}
