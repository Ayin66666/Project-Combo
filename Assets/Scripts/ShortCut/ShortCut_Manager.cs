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


    #region �ɼ� UI
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
            slots_Ingame[slot_Index].Slot_Setting(slot.item);
        }
        else
        {
            slots[slot_Index].Slot_Reset();
            slots_Ingame[slot_Index].Slot_Reset();
        }
    }

    /// <summary>
    /// ���� �ʿ� / ��Ʈ�� ���� UI ǥ�� / Ű���� 1234 �Է� �޾Ƽ� �ش� �� �Է�?
    /// </summary>
    public void Shortcut_Setting()
    {

    }
    #endregion
}
