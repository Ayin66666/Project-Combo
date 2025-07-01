using System.Collections.Generic;
using UnityEngine;


public class ShortCut_Manager : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private List<ShortCut_Slot> slots;
    [SerializeField] private List<ShortCut_InGame_Slot> slots_Ingame;


    private void Start()
    {
        SlotAction_Setting();
    }


    #region 옵션 UI
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
            slots_Ingame[slot_Index].Slot_Setting(slot.item);
        }
        else
        {
            slots[slot_Index].Slot_Reset();
            slots_Ingame[slot_Index].Slot_Reset();
        }
    }

    /// <summary>
    /// 제작 필요 / 쇼트컷 선택 UI 표기 / 키보드 1234 입력 받아서 해당 값 입력?
    /// </summary>
    public void Shortcut_Setting()
    {

    }
    #endregion
}
