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


    #region 세이브 & 로드
    /// <summary>
    /// 데이터 로드 시 동작
    /// </summary>
    public void LoadData(Data data)
    {
        if(data != null)
        {
            for (int i = 0; i < data.shortcut.Count; i++)
            {
                // 여기 슬롯 데이터 반환하는 함수나 변수 자체를 public으로 변경 필요
                slots[i].Slot_Setting(Player_Manager.instance.inventory.item_Slot[data.shortcut[i]]);
            }
        }
    }

    /// <summary>
    /// 세이브 시 쇼트컷 데이터 전달
    /// </summary>
    /// <returns></returns>
    public List<int> GetShortcutData()
    {
        // 해당 쇼트컷에 연결된 슬롯 인덱스를 저장
        List<int> data = new List<int>();
        for (int i = 0; i < slots.Count; i++)
        {
            data.Add(slots[i].itemSlot.slotIndex);
        }

        return data;
    }
    #endregion


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
    /// 게임 화면에서 쿨타임 UI 표기
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
    /// 쇼트컷 슬롯 데이터 셋팅 - 로드 시 호출
    /// </summary>
    /// <param name="slot_Index"></param>
    /// <param name="slot"></param>
    public void SlotData_Setting(int slot_Index, Inventory_Slot slot)
    {
        // 데이터가 있을 경우에는
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
    /// 제작 필요 / 쇼트컷 선택 UI 표기 / 키보드 1234 입력 받아서 해당 값 입력
    /// </summary>
    public void Shortcut_Setting(Inventory_Slot slot)
    {
        StartCoroutine(ShortcutSettingCall(slot));
    }

    private IEnumerator ShortcutSettingCall(Inventory_Slot slot)
    {
        // 쇼트컷 선택 UI On
        UI_Manager.instance.Shortcut_SelectUI(true);

        // 입력 대기
        int selectIndex = 0;
        bool inputWaiting = true;

        while (inputWaiting)
        {
            // 키 입력 감지
            if (Input.anyKeyDown)
            {
                bool isMatched = false;

                // 지정된 키 1,2,3,4 를 입력했는지 체크
                for (int i = 0; i < key.Length; i++)
                {
                    // 1,2,3,4 값 입력 시
                    if (Input.GetKeyDown(key[i]))
                    {
                        inputWaiting = false;
                        selectIndex = i;
                        isMatched = true;
                        break;
                    }
                }

                // 지정한 키가 아니라면
                if (!isMatched)
                {
                    UI_Manager.instance.Shortcut_ResultUI();
                }
            }

            yield return null;
        }

        // 아이템 셋팅
        slots[selectIndex].Slot_Setting(slot);
        slots_Ingame[selectIndex].Slot_Setting(slot.item);

        // UI 종료
        UI_Manager.instance.Shortcut_SelectUI(false);
    }
    #endregion
}
