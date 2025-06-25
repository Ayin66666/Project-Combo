using System.Collections.Generic;
using UnityEngine;


public class Equipment_Manager : MonoBehaviour
{
    [Header("---Equipment Slot---")]
    [SerializeField] private List<SlotData> slots; // 외부 셋팅용
    private Dictionary<Item_Equipment.EquipmentType, Inventory_Slot_Equipment> itemSlot; // 실제 데이터 1
    [SerializeField] private List<SlotData> coreSlot; // 실제 데이터 2

    /// <summary>
    /// 딕셔너리 용 장비 슬롯 데이터
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
        // 1. 장비 슬롯 데이터 할당
        Dictonary_Setting();

        // 2. 착용 장비 데이터 로드
        // Data_Setting();
    }


    #region 스크립트 초기 실행 로직
    /// <summary>
    /// 딕셔너리 장비 슬롯 데이터 할당
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
                Debug.Log("중복 장비 셋팅");
            }
        }
    }

    /// <summary>
    /// 게임 로드 시 데이터 셋팅
    /// </summary>
    /// <param name="data"></param>
    public void Data_Setting()
    {
        // 데이터 로드
        Data data = SaveLoad_Manager.instance.LoadData(SaveLoad_Manager.instance.curSlot);

        // 데이터 적용

    }
    #endregion


    #region 장비 착용 & 해제 로직
    /// <summary>
    /// 장비 착용 로직
    /// </summary>
    /// <param name="item"></param>
    public void Equipment(Inventory_Slot slot, Item_Equipment item)
    {
        Debug.Log("Call Equipment");

        if(item.equipmentType == Item_Equipment.EquipmentType.Core)
        {
            // 장신구 착용 - 별개의 리스트
            for (int i = 0; i < coreSlot.Count; i++)
            {
                if (!coreSlot[i].slot.haveItem)
                {
                    Debug.Log("Equipment - Core / New");
                    // 슬롯이 비어있다면 해당 슬롯에 착용
                    coreSlot[i].slot.Item_Setting(true, item);
                    return;
                }
            }

            // 빈 슬롯이 없다면 - 첫번째 슬롯에 장착

            // 스테이터스 감소
            Player_Manager.instance.status.Equipment_Status_Setting(false, coreSlot[0].slot.Item.equipment_Status);

            // 기존 장착 장비 인벤토리로 복귀
            Player_Manager.instance.inventory.Item_Change(slot, coreSlot[0].slot.Item);

            // 신규 장비 장착
            Player_Manager.instance.status.Equipment_Status_Setting(true, item.equipment_Status);
            coreSlot[0].slot.Item_Setting(true, item);

            Debug.Log("Equipment - Core/Change");
        }
        else
        {
            // 장비 착용 - 딕셔너리 키
            if (itemSlot[item.equipmentType].haveItem)
            {
                // 착용 장비 O - 장비 해제 -> 장비 착용
                Debug.Log($"Equipment - {item}");

                // 스테이터스 감소
                Player_Manager.instance.status.Equipment_Status_Setting(false, itemSlot[item.equipmentType].Item.equipment_Status);
               
                // 기존 장착 장비 인벤토리로 복귀
                Player_Manager.instance.inventory.Item_Change(slot, itemSlot[item.equipmentType].Item);

                // 장착
                Player_Manager.instance.status.Equipment_Status_Setting(true, item.equipment_Status);
                itemSlot[item.equipmentType].Item_Setting(true, item);
            }
            else
            {
                // 착용 장비 X - 즉시 장착
                Debug.Log($"Equipment / New - {item}");

                // 장착
                Player_Manager.instance.status.Equipment_Status_Setting(true, item.equipment_Status);
                itemSlot[item.equipmentType].Item_Setting(true, item);
            }
        }
    }

    /// <summary>
    /// 장비 해제 로직
    /// </summary>
    /// <param name="index">슬롯 인덱스</param>
    public void EnEquipment(Inventory_Slot_Equipment slot)
    {
        // 인벤토리 내 빈 슬롯 체크

        if (true)
        {
            // 빈 자리가 있다면 - 장비 해제 후 슬롯 이동

            // 스테이터스 초기화
            Player_Manager.instance.status.Equipment_Status_Setting(false, slot.Item.equipment_Status);

            // 슬롯 초기화
            slot.Item_Setting(false, null);
        }
        else
        {
            // 빈 자리가 없다면 - 해제불가
        }
    }
    #endregion
}
