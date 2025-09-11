using System;
using System.Collections.Generic;
using UnityEngine;


public class Inventory_Manager : MonoBehaviour
{
    [Header("--- Setting ---")]
    public int slotCount;
    public List<Inventory_Slot> item_Slot;


    private void Awake()
    {
        slotCount = item_Slot.Count;
    }

    #region 세이브 & 로드
    /// <summary>
    /// 세이브 시 데이터 전달
    /// </summary>
    /// <returns></returns>
    public List<Vector2Int> GetItemData()
    {
        List<Vector2Int> item = new List<Vector2Int>();
        for (int i = 0; i < item_Slot.Count; i++)
        {
            if (item_Slot[i].haveItem)
            {
                Vector2Int data = new Vector2Int(item_Slot[i].item.itemCode, item_Slot[i].itemCount);
                item.Add(data);
            }
            else
            {
                Vector2Int data = new Vector2Int(-1, 0);
                item.Add(data);
            }
        }

        return item;
    }

    /// <summary>
    /// 게임 시작 시 데이터 로드
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public void Inventory_Setting(Data data)
    {
        int dataSlotCount = data.itemData.Count;
        for (int i = 0; i < item_Slot.Count; i++)
        {
            if(i < dataSlotCount)
            {
                if (data.itemData[i].itemCode != -1)
                {
                    Item_Base item = ItemData_Container.instance.FindItem(data.itemData[i].itemCode);

                    if (item != null)
                        item_Slot[i].Slot_Setting(item, data.itemData[i].itemCount);
                }
            }
            else
            {
                item_Slot[i].Slot_Reset();
            }
        }
    }
    #endregion


    #region 기능 동작
    /// <summary>
    /// 아이템 습득 시 호출
    /// </summary>
    /// <param name="addItem"></param>
    /// <param name="itemCount"></param>
    public void Item_Add(Item_Base addItem, int itemCount)
    {
        // 데이터 오류 체크
        if (addItem == null || itemCount <= 0)
        {
            return;
        }

        // 1. 아이템 중첩 가능 여부 체크
        if (addItem.stackable)
        {
            //.Log("중첩가능 - 아이템 체크1");
            // 2. 중첩 가능 시
            // 2-1 이미 인벤토리에 해당 아이템이 있는지 & 중첩 최대치가 아닌지
            Inventory_Slot slot = Slot_Find(slot =>
            slot.item != null && slot.item.itemCode == addItem.itemCode);
            if (slot != null)
            {
                //Debug.Log("중첩가능 - 아이템 체크2");

                // 1. 중첩 가능한 슬롯에 먼저 넣기
                while (itemCount > 0)
                {
                    Inventory_Slot stackSlot = Slot_Find(slot =>
                        slot.item != null &&
                        slot.item.itemCode == addItem.itemCode &&
                        slot.itemCount < slot.item.maxStack);

                    if (stackSlot == null)
                        break;

                    int space = stackSlot.item.maxStack - stackSlot.itemCount;
                    int toAdd = Mathf.Min(space, itemCount);
                    stackSlot.Slot_Setting(addItem, stackSlot.itemCount + toAdd);
                    itemCount -= toAdd;
                }
            }

            //Debug.Log("중첩가능 - 아이템 체크 후 빈슬롯 넣기");
            // 3. 빈 슬롯 체크 후 넣기
            itemCount = AddToEmptySlots(addItem, itemCount);
        }
        else
        {
            //Debug.Log("중첩불가능 - 빈슬롯 넣기");
            // 3. 중첩 불가능 시 - 빈 슬롯 검출
            itemCount = AddToEmptySlots(addItem, itemCount);
        }

        // 그래도 남는 아이템이 있다면 - 아이템 드롭
        if (itemCount > 0)
        {
            //Debug.Log("최종 - 아이템 드롭");
            Item_Drop(addItem, itemCount);
        }
    }

    /// <summary>
    /// 빈 슬롯이 있다면 인풋, 아니라면 드롭
    /// </summary>
    /// <param name="addItem"></param>
    /// <param name="itemCount"></param>
    private int AddToEmptySlots(Item_Base addItem, int itemCount)
    {
        while (itemCount > 0)
        {
            Inventory_Slot emptySlot = Slot_Find(slot => !slot.haveItem);
            if (emptySlot == null)
            {
                Item_Drop(addItem, itemCount);
                break;
            }

            int toAdd = Mathf.Min(addItem.maxStack, itemCount);
            emptySlot.Slot_Setting(addItem, toAdd);
            itemCount -= toAdd;
        }

        return itemCount; // 남은 아이템 수 반환 (0이면 다 넣은 것)
    }

    /// <summary>
    /// 아이템을 한계 이상으로 획득했을때 드롭 기능
    /// </summary>
    /// <param name="item"></param>
    /// <param name="itemCount"></param>
    private void Item_Drop(Item_Base item, int itemCount)
    {
        GameObject obj = Instantiate(gameObject, transform.position, Quaternion.identity);
        obj.AddComponent<Item_Drop>();
    }

    /// <summary>
    /// 입력값에 따른 조건 검사
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    private Inventory_Slot Slot_Find(Func<Inventory_Slot, bool> predicate)
    {
        foreach (Inventory_Slot slot in item_Slot)
        {
            if (predicate(slot))
            {
                return slot;
            }
        }

        return null;
    }

    /// <summary>
    /// 장비 장착 시 기존 아이템 이동
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="itme"></param>
    public void Item_Change(Inventory_Slot slot, Item_Base item)
    {
        Debug.Log($"장비 교체! {slot} / {item}");

        // 아이템 추가
        slot.Slot_Setting(item, 1);
    }

    /// <summary>
    /// 아이템 습득 전 인벤토리 상태 체크 - 소모품 & 기타
    /// </summary>
    /// <param name="addItem"></param>
    /// <returns></returns>
    public bool IsFull(Item_Base addItem)
    {
        foreach (var slot in item_Slot)
        {
            // 슬롯이 비어있으면 인벤토리는 꽉 차지 않은 상태
            if (slot.item == null)
                return false;

            // 중첩 가능하고, 해당 아이템과 동일하며, 아직 최대 스택에 도달하지 않았다면
            if (slot.item.itemCode == addItem.itemCode && slot.item.stackable && slot.itemCount < slot.item.maxStack)
                return false;
        }

        // 빈 슬롯도 없고, 최대 스택 초과 가능한 슬롯도 없으니 꽉 찬 상태
        return true;
    }

    /// <summary>
    /// 아이템 슬롯 체크 - 장비
    /// </summary>
    /// <returns></returns>
    public bool IsFull()
    {
        foreach (var slot in item_Slot)
        {
            if (slot.item == null)
                return false;
        }

        return true;
    }

    /// <summary>
    /// 인벤토리 내 아이템 정리
    /// 정리 조건 : (장비 - 소모품 - 기타) / (고티어 - 저티어) / (무기, 코어, 머리, 몸통, 하의, 신발)
    /// </summary>
    public void Inventory_Organize()
    {
        // 설명 UI Off
        UI_Manager.instance.Item_DescriptionUI(false, null);

        // 아이템 데이터 저장
        List<(Item_Base item, int count)> itemData = new List<(Item_Base item, int count)>();
        for (int i = 0; i < item_Slot.Count; i++)
        {
            if (item_Slot[i].haveItem)
            {
                itemData.Add((item_Slot[i].item, item_Slot[i].itemCount));
            }
        }

        // 데이터 정렬
        itemData.Sort((a, b) =>
        {
            // 타입 체크
            int typeCompare = ((int)a.item.itemType) - ((int)b.item.itemType);
            if (typeCompare != 0) return typeCompare;

            // 등급 체크
            int ratingCompare = ((int)a.item.itemRating) - ((int)b.item.itemRating);
            if (ratingCompare != 0) return ratingCompare;

            // 장비 타입의 경우 순서 체크 (무기 - 코어 - 머리 - 몸통 - 하의 - 신발 순서)
            if(a.item.itemType == Item_Base.Item_Type.Equipment && b.item.itemType == Item_Base.Item_Type.Equipment)
            {
                Item_Equipment eqA = a.item as Item_Equipment;
                Item_Equipment eqB = b.item as Item_Equipment;

                if(eqA != null && eqB != null)
                    return ((int)eqB.equipmentType - (int)eqA.equipmentType);
            }

            return 0;
        });

        // 슬롯 초기화
        foreach (Inventory_Slot slot in item_Slot)
        {
            slot.Slot_Reset();
        }

        // 데이터 입력
        for (int i = 0; i < itemData.Count; i++)
        {
            item_Slot[i].Slot_Setting(itemData[i].item, itemData[i].count);
        }
    }
    #endregion


    #region 쇼트컷 아이템 소모 & 체크
    /// <summary>
    /// 해당 코드의 아이템이 몇개 있는지 체크
    /// </summary>
    /// <param name="itemCode"></param>
    public int GetItemCount(int itemCode)
    {
        int itemCount = 0;
        foreach (Inventory_Slot slot in item_Slot)
        {
            if (slot.haveItem && slot.item.itemCode == itemCode)
            {
                itemCount += slot.itemCount;
            }
        }

        return itemCount;
    }

    /// <summary>
    /// 해당 코드의 소비 아이템 소모
    /// </summary>
    /// <param name="itemCode"></param>
    public bool RemoveItemCount(int itemCode)
    {
        foreach (Inventory_Slot slot in item_Slot)
        {
            // 아이템 코드가 동일하다면 소모
            if (slot.haveItem && slot.item.itemCode == itemCode)
            {
                // 아이템 소모
                slot.itemCount--;

                // 만약 해당 슬롯에서 아이템을 모두 소비했다면 비우기
                if(slot.itemCount <= 0)
                    slot.Slot_Reset();

                // 남은 아이템 갯수에 따른 리턴값 변화
                return GetItemCount(itemCode) > 0;
            }
        }

        // 아이템이 없다면 - 없음 반환
        return false;
    }
    #endregion
}
