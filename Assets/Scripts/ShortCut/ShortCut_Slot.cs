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
    /// ���Կ� ������ �Է�
    /// </summary>
    /// <param name="item_Slot"></param>
    /// <param name="item"></param>
    /// <param name="count"></param>
    public void Slot_Setting(Inventory_Slot item_Slot)
    {
        // �̹� ���Կ� �������� �ִٸ� - ���� ������ ����
        if (haveItem)
            Slot_Reset();

        // ������ Ÿ�� �˻�
        if (item_Slot.item.itemType == Item_Base.Item_Type.Consumable)
        {
            // �Һ� �������̶�� - UI ����
            haveItem = true;
            itemSlot = item_Slot;
            icon.sprite = itemSlot.item.Icon;
            countText.text = itemSlot.itemCount.ToString();
        }
        else
        {
            // �Һ� �������� �ƴ϶��
            Debug.Log("�Һ� �������� �ƴմϴ�!");
        }
    }

    /// <summary>
    /// ���� ������ ����
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
    /// ������ ��� ����
    /// </summary>
    public void Use()
    {
        if (itemSlot != null)
        {
            // ������ ���
            itemSlot.Slot_Use();

            // ��Ÿ�� ȣ��
            Player_Manager.instance.shortCut.IngameSlotCooldown(this);

            // ������ �ܷ� Ȯ��
            if (!itemSlot.haveItem)
            {
                Slot_Reset();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // ���콺 ��Ŭ�� �� ���� �� ������ ����
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // ���� �ʱ�ȭ
            Slot_Reset();
        }
    }
}
