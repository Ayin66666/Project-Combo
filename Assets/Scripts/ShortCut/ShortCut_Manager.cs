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
    /// 인풋 이벤트 셋팅
    /// </summary>
    private void SlotAction_Setting()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            Input_Manager.instance.ShortCut_Setting(i, slots[i].Use);
        }
    }

    /// <summary>
    /// 쇼트컷 슬롯 데이터 셋팅 - 로드 시 호출
    /// </summary>
    /// <param name="slot_Index"></param>
    /// <param name="slot"></param>
    public void SlotData_Setting(int slot_Index, Inventory_Slot slot)
    {
        // 데이터가 있을 경우에는
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
