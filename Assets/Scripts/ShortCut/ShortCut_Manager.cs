using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class ShortCut_Manager : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private List<ShortCut_Slot> slots;
    [SerializeField] private List<ShortCut_InGame_Slot> slots_Ingame;
    private KeyCode[] key;


    private void Start()
    {
        SlotAction_Setting();

        key = new KeyCode[]
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
        };
    }


    #region ���̺� & �ε�
    /// <summary>
    /// ������ �ε� �� ����
    /// </summary>
    public void LoadData(Data data)
    {
        if(data != null)
        {
            for (int i = 0; i < data.shortcut.Count; i++)
            {
                // ���� ���� ������ ��ȯ�ϴ� �Լ��� ���� ��ü�� public���� ���� �ʿ�
                slots[i].Slot_Setting(Player_Manager.instance.inventory.item_Slot[data.shortcut[i]]);
            }
        }
    }

    /// <summary>
    /// ���̺� �� ��Ʈ�� ������ ����
    /// </summary>
    /// <returns></returns>
    public List<int> GetShortcutData()
    {
        // �ش� ��Ʈ�ƿ� ����� ���� �ε����� ����
        List<int> data = new List<int>();
        for (int i = 0; i < slots.Count; i++)
        {
            data.Add(slots[i].itemSlot.slotIndex);
        }

        return data;
    }
    #endregion


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
    /// ���� ȭ�鿡�� ��Ÿ�� UI ǥ��
    /// </summary>
    /// <param name="slot"></param>
    public void IngameSlotCooldown(ShortCut_Slot slot)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == slot)
            {
                slots_Ingame[i].Cooldown();
            }
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
        if (slot != null)
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
    /// ���� �ʿ� / ��Ʈ�� ���� UI ǥ�� / Ű���� 1234 �Է� �޾Ƽ� �ش� �� �Է�
    /// </summary>
    public void Shortcut_Setting(Inventory_Slot slot)
    {
        StartCoroutine(ShortcutSettingCall(slot));
    }

    private IEnumerator ShortcutSettingCall(Inventory_Slot slot)
    {
        // ��Ʈ�� ���� UI On
        UI_Manager.instance.Shortcut_SelectUI(true);

        // �Է� ���
        int selectIndex = 0;
        bool inputWaiting = true;

        while (inputWaiting)
        {
            // Ű �Է� ����
            if (Input.anyKeyDown)
            {
                bool isMatched = false;

                // ������ Ű 1,2,3,4 �� �Է��ߴ��� üũ
                for (int i = 0; i < key.Length; i++)
                {
                    // 1,2,3,4 �� �Է� ��
                    if (Input.GetKeyDown(key[i]))
                    {
                        inputWaiting = false;
                        selectIndex = i;
                        isMatched = true;
                        break;
                    }
                }

                // ������ Ű�� �ƴ϶��
                if (!isMatched)
                {
                    UI_Manager.instance.Shortcut_ResultUI();
                }
            }

            yield return null;
        }

        // ������ ����
        slots[selectIndex].Slot_Setting(slot);
        slots_Ingame[selectIndex].Slot_Setting(slot.item);

        // UI ����
        UI_Manager.instance.Shortcut_SelectUI(false);
    }
    #endregion
}
