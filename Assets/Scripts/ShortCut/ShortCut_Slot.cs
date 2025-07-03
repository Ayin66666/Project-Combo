using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;


public class ShortCut_Slot : MonoBehaviour, IPointerClickHandler
{
    [Header("---Setting---")]
    public Inventory_Slot itemSlot;
    public Item_Base item;
    public bool haveItem;


    [Header("---UI---")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;


    /// <summary>
    /// 슬롯에 아이템 입력
    /// </summary>
    /// <param name="item_Slot"></param>
    /// <param name="item"></param>
    /// <param name="count"></param>
    public void Slot_Setting(Inventory_Slot item_Slot)
    {
        // 이미 슬롯에 아이템이 있다면 - 기존 아이템 비우기
        if (haveItem)
            Slot_Reset();

        // 아이템 타입 검사
        if (item_Slot.item.itemType == Item_Base.Item_Type.Consumable)
        {
            // 소비 아이템이라면 - UI 셋팅
            haveItem = true;
            itemSlot = item_Slot;
            icon.sprite = itemSlot.item.Icon;
            countText.text = itemSlot.itemCount.ToString();
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
        itemSlot = null;

        // UI Reset
        icon.sprite = null;
        countText.text = "";
    }

    /// <summary>
    /// 아이템 사용 로직
    /// </summary>
    public void Use()
    {
        if (itemSlot != null)
        {
            // 아이템 사용
            itemSlot.Slot_Use();

            // 쿨타임 호출
            Player_Manager.instance.shortCut.IngameSlotCooldown(this);

            // 아이템 잔량 확인
            if (!itemSlot.haveItem)
            {
                Slot_Reset();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 마우스 우클릭 시 슬롯 내 아이템 제거
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 슬롯 초기화
            Slot_Reset();
        }
    }
}
