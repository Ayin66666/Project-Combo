using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory_Slot_Equipment : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("---Setting---")]
    [SerializeField] private Item_Equipment item;
    public bool haveItem;


    public void Equipment(Item_Equipment item)
    {
        // �÷��̾� �Ŵ����� ���� ���� - �̰� ���� �Լ� �̾ƵѰ���?
        Player_Manager.instance.status.Equipment_Status_Setting(true, item.equipment_Status);
    }

    public void UnEquipment()
    {
        // �÷��̾� �Ŵ����� ���� ���� - �̰� ���� �Լ� �̾ƵѰ���?
    }


    #region Ŭ�� �̺�Ʈ
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // ���콺 ��Ŭ�� �� - ������ ����
            UnEquipment();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (haveItem)
        {
            UI_Manager.instance.Item_DescriptionUI(true, item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (haveItem)
        {
            UI_Manager.instance.Item_DescriptionUI(false, null);
        }
    }
    #endregion
}
