using System;
using UnityEngine;
using System.Collections.Generic;


public class Inventory_Manager : MonoBehaviour
{
    [Header("--- Setting ---")]
    [SerializeField] private List<Inventory_Slot> item_Slot;
    public Canvas canvas;


    /// <summary>
    /// ������ ���� �� ȣ��
    /// </summary>
    /// <param name="addItem"></param>
    /// <param name="itemCount"></param>
    public void Item_Add(Item_Base addItem, int itemCount)
    {
        // ������ ���� üũ
        if(addItem == null || itemCount <= 0)
        {
            return;
        }

        // 1. ������ ��ø ���� ���� üũ
        if (addItem.stackable)
        {
            //.Log("��ø���� - ������ üũ1");
            // 2. ��ø ���� ��
            // 2-1 �̹� �κ��丮�� �ش� �������� �ִ��� & ��ø �ִ�ġ�� �ƴ���
            Inventory_Slot slot = Slot_Find(slot => 
            slot.item != null && slot.item.itemCode == addItem.itemCode);
            if (slot != null)
            {
                //Debug.Log("��ø���� - ������ üũ2");

                // 1. ��ø ������ ���Կ� ���� �ֱ�
                while (itemCount > 0)
                {
                    Debug.Log("��ø���� - ������ üũ��");
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

            //Debug.Log("��ø���� - ������ üũ �� �󽽷� �ֱ�");
            // 3. �� ���� üũ �� �ֱ�
            itemCount = AddToEmptySlots(addItem, itemCount);
        }
        else
        {
            //Debug.Log("��ø�Ұ��� - �󽽷� �ֱ�");
            // 3. ��ø �Ұ��� �� - �� ���� ����
            itemCount = AddToEmptySlots(addItem, itemCount);
        }

        // �׷��� ���� �������� �ִٸ� - ������ ���
        if (itemCount > 0)
        {
            //Debug.Log("���� - ������ ���");
            Item_Drop(addItem, itemCount);
        }
    }

    /// <summary>
    /// �� ������ �ִٸ� ��ǲ, �ƴ϶�� ���
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

        return itemCount; // ���� ������ �� ��ȯ (0�̸� �� ���� ��)
    }

    /// <summary>
    /// �������� �Ѱ� �̻����� ȹ�������� ��� ���
    /// </summary>
    /// <param name="item"></param>
    /// <param name="itemCount"></param>
    private void Item_Drop(Item_Base item, int itemCount)
    {
        GameObject obj = Instantiate(gameObject, transform.position, Quaternion.identity);
        obj.AddComponent<Item_Drop>();
    }


    /// <summary>
    /// �Է°��� ���� ���� �˻�
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
    /// ��� ���� �� ���� ������ �̵�
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="itme"></param>
    public void Item_Change(Inventory_Slot slot, Item_Base item)
    {
        // ������ �߰�
        slot.Slot_Setting(item, 1);
    }


    /// <summary>
    /// ������ ���� �� �κ��丮 ���� üũ
    /// </summary>
    /// <param name="addItem"></param>
    /// <returns></returns>
    public bool IsFull(Item_Base addItem)
    {
        foreach (var slot in item_Slot)
        {
            // ������ ��������� �κ��丮�� �� ���� ���� ����
            if (slot.item == null)
                return false;

            // ��ø �����ϰ�, �ش� �����۰� �����ϸ�, ���� �ִ� ���ÿ� �������� �ʾҴٸ�
            if (slot.item.itemCode == addItem.itemCode && slot.item.stackable && slot.itemCount < slot.item.maxStack)
                return false;
        }

        // �� ���Ե� ����, �ִ� ���� �ʰ� ������ ���Ե� ������ �� �� ����
        return true;
    }
}
