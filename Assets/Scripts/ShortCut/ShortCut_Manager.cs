using System.Collections.Generic;
using UnityEngine;


public class ShortCut_Manager : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private List<ShortCut_Slot> slots;


    private void Start()
    {
        SlotAction_Setting();
    }


    /// <summary>
    /// ��ǲ �̺�Ʈ ����
    /// </summary>
    private void SlotAction_Setting()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            Input_Manager.instance.ShortCut_Setting(i, slots[i].Use);
        }
    }

    /// <summary>
    /// ��Ʈ�� ���� ������ ���� - �ε� �� ȣ��
    /// </summary>
    /// <param name="slot_Index"></param>
    /// <param name="slot"></param>
    public void SlotData_Setting(int slot_Index, Inventory_Slot slot)
    {
        // �����Ͱ� ���� ��쿡��
        if(slot != null)
        {
            slots[slot_Index].Slot_Setting(slot);
        }
        else
        {
            slots[slot_Index].Slot_Reset();
        }
    }
}
