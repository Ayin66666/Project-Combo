using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ShortCut_Slot : MonoBehaviour, IPointerClickHandler
{
    [Header("---Setting---")]
    public Item_Base item;
    public bool haveItem;
    public int slotCount;


    [Header("---UI---")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;


    /// <summary>
    /// 슬롯에 아이템 입력
    /// </summary>
    /// <param name="item"></param>
    /// <param name="item"></param>
    /// <param name="count"></param>
    public void Slot_Setting(Item_Base item)
    {
        // 이미 슬롯에 아이템이 있다면 - 기존 아이템 비우기
        if (haveItem)
            Slot_Reset();

        // 아이템 타입 검사
        if (item.itemType == Item_Base.Item_Type.Consumable)
        {
            // 소비 아이템이라면 - UI 셋팅
            haveItem = true;
            this.item = item;
            icon.sprite = item.Icon;
            countText.text = Player_Manager.instance.inventory.GetItemCount(item.itemCode).ToString();
        }
        else
        {
            // 소비 아이템이 아니라면
            Debug.Log("소비 아이템이 아닙니다!");
        }
    }

    /// <summary>
    /// 슬롯 아이템 제거
    /// </summary>
    public void Slot_Reset()
    {
        // Status Reset
        haveItem = false;
        item = null;

        // UI Reset
        icon.sprite = null;
        countText.text = "";
    }

    /// <summary>
    /// 아이템 사용 로직
    /// </summary>
    public void Use()
    {
        // 쇼트컷 사용 불가 상태라면 리턴
        if (!Player_Manager.instance.shortCut.canUseShortcut)
            return;

        // 쇼트컷 내에 아이템이 업다면 리턴
        if (!haveItem)
            return;

        // 아이템 체크
        Debug.Log($"아이템 체크 : {item} / {item.itemCode}");
        int count = Player_Manager.instance.inventory.GetItemCount(item.itemCode);
        if (count > 0)
        {
            // 아이템 사용
            (bool isCooldown, float remainingTime) = Player_Manager.instance.cooldown.Cooldown_Check(((Item_Consumable)item).Key);
            if (isCooldown == false)
            {
                // 기능 동작
                item.Use();

                // 쿨타임 호출
                Player_Manager.instance.shortCut.IngameSlotCooldown(this);

                // 아이템 갯수 감소 & 0개라면 초기화
                bool isEmpty = Player_Manager.instance.inventory.RemoveItemCount(item.itemCode);
                if (isEmpty)
                {
                    Player_Manager.instance.shortCut.Shortcut_Remove(slotCount);
                    Slot_Reset();
                }
            }
            else
            {
                // 사용 불가 UI
                UI_Manager.instance.ItemCooldownUI(remainingTime);
            }
        }
        else
        {
            // 모종의 이유로 아이템이 없다면 - 슬롯 초기화
            Player_Manager.instance.shortCut.Shortcut_Remove(slotCount);
            Slot_Reset();
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        // 마우스 우클릭 시 슬롯 내 아이템 제거
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 슬롯 초기화
            if(haveItem)
            {
                // 클릭 사운드
                Player_Sound.instance.Sound_System(Player_Sound.SystemSound.Click);

                Slot_Reset();
            }
        }
    }
}
