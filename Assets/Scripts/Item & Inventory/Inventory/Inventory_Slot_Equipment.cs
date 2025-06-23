using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;


public class Inventory_Slot_Equipment : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("---Setting---")]
    [SerializeField] private Item_Equipment item;
    public bool haveItem;


    [Header("---UI---")]
    [SerializeField] private Image icon;


    #region 프로퍼티
    public Item_Equipment Item { get { return item; } private set { item = value; } }
    #endregion


    public void Item_Setting(bool equipment, Item_Equipment item)
    {
        if(equipment)
        {
            // 장비 착용
            icon.sprite = item.Icon;
            this.item = item;
        }
        else
        {
            // 장비 해제
            icon.sprite = null;
            this.item = null;
        }
    }


    #region 클릭 이벤트
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 마우스 우클릭 시 - 아이템 해제
            Player_Manager.instance.equipment.EnEquipment(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (haveItem)
        {
            // 아이템 설명 UI On - 장비 아이템용 UI로 변경 필요
            UI_Manager.instance.Item_DescriptionUI(true, item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (haveItem)
        {
            // 아이템 설명 UI Off - 장비 아이템용 UI로 변경 필요
            UI_Manager.instance.Item_DescriptionUI(false, null);
        }
    }
    #endregion
}
