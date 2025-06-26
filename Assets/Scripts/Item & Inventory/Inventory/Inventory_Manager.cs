using System;
using UnityEngine;
using System.Collections.Generic;


public class Inventory_Manager : MonoBehaviour
{
    [Header("--- Setting ---")]
    [SerializeField] private List<Inventory_Slot> item_Slot;
    public Canvas canvas;


    /// <summary>
    /// 아이템 습득 시 호출
    /// </summary>
    /// <param name="addItem"></param>
    /// <param name="itemCount"></param>
    public void Item_Add(Item_Base addItem, int itemCount)
    {
        // 데이터 오류 체크
        if(addItem == null || itemCount <= 0)
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
                    Debug.Log("중첩가능 - 아이템 체크중");
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
        // 아이템 추가
        slot.Slot_Setting(item, 1);
    }


    /// <summary>
    /// 아이템 습득 전 인벤토리 상태 체크
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
}
