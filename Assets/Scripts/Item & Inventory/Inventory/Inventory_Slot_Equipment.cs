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


    #region ������Ƽ
    public Item_Equipment Item { get { return item; } private set { item = value; } }
    #endregion


    public void Item_Setting(bool equipment, Item_Equipment item)
    {
        if(equipment)
        {
            // ��� ����
            icon.sprite = item.Icon;
            this.item = item;
        }
        else
        {
            // ��� ����
            icon.sprite = null;
            this.item = null;
        }
    }


    #region Ŭ�� �̺�Ʈ
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // ���콺 ��Ŭ�� �� - ������ ����
            Player_Manager.instance.equipment.EnEquipment(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (haveItem)
        {
            // ������ ���� UI On - ��� �����ۿ� UI�� ���� �ʿ�
            UI_Manager.instance.Item_DescriptionUI(true, item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (haveItem)
        {
            // ������ ���� UI Off - ��� �����ۿ� UI�� ���� �ʿ�
            UI_Manager.instance.Item_DescriptionUI(false, null);
        }
    }
    #endregion
}
