using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Inventory_Slot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler /*IBeginDragHandler, IDragHandler, IEndDragHandler*/
{
    [Header("---Slot Setting---")]
    public int slotIndex;


    [Header("---Item Data---")]
    public Item_Base item;
    public bool haveItem;
    public int itemCount;


    [Header("---Slot UI---")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;


    [Header("---Menu UI---")]
    [SerializeField] private GameObject menuSet;



    #region ��� ����
    /// <summary>
    /// ������ �ε��� ����
    /// </summary>
    /// <param name="index"></param>
    public void SlotIndex_Setting(int index)
    {
        slotIndex = index;
    }

    /// <summary>
    /// ������ �߰�
    /// </summary>
    public void Slot_Setting(Item_Base data, int addCount)
    {
        Debug.Log($"Call slot item Setting{data} {addCount}");

        // ������ �߰�
        haveItem = true;
        item = data;
        itemCount = addCount;

        // UI ����
        icon.sprite = item.Icon;
        countText.text = itemCount.ToString();
    }

    /// <summary>
    /// ���� ������ ���� ���
    /// </summary>
    public void Slot_Reset()
    {
        haveItem = false;
        item = null;
        itemCount = 0;

        icon.sprite = null;
        countText.text = "";
    }

    /// <summary>
    /// ������ ���
    /// </summary>
    public void Slot_Use()
    {
        if (!haveItem)
        {
            return;
        }

        switch (item.itemType)
        {
            case Item_Base.Item_Type.Equipment:
                Use_Equipment();
                break;

            case Item_Base.Item_Type.Consumable:
                Use_Consumable();
                break;

            case Item_Base.Item_Type.Other:
                Use_Other();
                break;
        }

    }

    private void Use_Equipment()
    {
        // ���� ���� ȣ��
        Player_Manager.instance.equipment.Equipment(this, (Item_Equipment)item);

        // UI Off
        UI_Manager.instance.ItemEquipment_DescriptionUI(false, null);

        // ���� �ʱ�ȭ
        Slot_Reset();
    }

    private void Use_Consumable()
    {
        // ��Ÿ�� üũ
        (bool isCooldown, float remainingTime) = Player_Manager.instance.cooldown.Cooldown_Check(((Item_Consumable)item).Key);
        if (isCooldown == false)
        {
            // ��� ����
            item.Use();
            itemCount--;
            countText.text = itemCount.ToString();

            if (itemCount <= 0)
            {
                // UI Off & ���� �ʱ�ȭ
                UI_Manager.instance.Item_DescriptionUI(false, null);
                Slot_Reset();
            }
        }
        else
        {
            // ��� �Ұ� UI
            UI_Manager.instance.ItemCooldownUI(remainingTime);
        }
    }

    private void Use_Other()
    {
        // UI Off
        UI_Manager.instance.Item_DescriptionUI(false, null);

        // ������ ����?
        item.Use();
    }
    #endregion


    #region Item Meun
    /// <summary>
    /// �޴� UI ǥ�� 
    /// </summary>
    public void Meun()
    {
        if (haveItem)
            menuSet.SetActive(true);
    }

    /// <summary>
    /// ������ ���
    /// </summary>
    public void Click_Use()
    {
        // �޴� UI ����
        menuSet.SetActive(false);

        // ������ ���
        Slot_Use();
    }

    /// <summary>
    /// ��Ʈ�� ���
    /// </summary>
    public void Click_Shortcut()
    {
        // �޴� UI ����
        menuSet.SetActive(false);

        // ��Ʈ�� ��� ��� ȣ�� - �Լ� ���� �ʿ�
        Player_Manager.instance.shortCut.Shortcut_Setting(this);
    }
    #endregion


    #region Ŭ�� ��� - ��Ŭ�� = �̵� | ��Ŭ�� = ��� | �巡�׿� ������Ʈ �߰��� ��!
    public void OnPointerClick(PointerEventData eventData)
    {
        // �������� �ִٸ�
        if (!haveItem)
        {
            return;
        }

        // Ŭ�� ����
        Player_Sound.instance.Sound_System(Player_Sound.SystemSound.Click);

        // ���콺 ������ Ŭ���� üũ
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item.itemType == Item_Base.Item_Type.Consumable)
            {
                // �Һ� �������̶�� �޴� UI ǥ��
                Meun();
            }
            else
            {
                // �̿� �������� ��� ���
                Slot_Use();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (haveItem)
        {
            if (item.itemType == Item_Base.Item_Type.Equipment)
            {
                UI_Manager.instance.ItemEquipment_DescriptionUI(true, (Item_Equipment)item);
            }
            else
            {
                if (!menuSet.activeSelf)
                    UI_Manager.instance.Item_DescriptionUI(true, item);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (haveItem)
        {
            if (item.itemType == Item_Base.Item_Type.Equipment)
            {
                UI_Manager.instance.ItemEquipment_DescriptionUI(false, null);
            }
            else
            {
                UI_Manager.instance.Item_DescriptionUI(false, null);
            }

        }
    }
    #endregion
}
