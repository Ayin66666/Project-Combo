using System.Collections.Generic;
using UnityEngine;


public class ItemData_Container : MonoBehaviour
{
    public static ItemData_Container instance;

    [Header("---Setting---")]
    [SerializeField] private List<Item_Base> items;
    private Dictionary<int, Item_Base> itemDatas = new Dictionary<int, Item_Base>();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Data_Setting();
    }


    /// <summary>
    /// ���� ���� �� ��ųʸ��� ������ - Ű�� ����
    /// </summary>
    private void Data_Setting()
    {
        // ��ųʸ��� ������ �߰�
        for (int i = 0; i < items.Count; i++)
        {
            // �ߺ� ������ �ڵ� üũ
            Item_Base item = items[i];
            if (!itemDatas.ContainsKey(item.itemCode))
            {
                itemDatas.Add(item.itemCode, item);
            }
            else
            {
                Debug.LogWarning($"[ItemData_Container] �ߺ��� ������ �ڵ尡 �����մϴ�: {item.itemCode}");
            }
        }
    }

    /// <summary>
    /// ������ �ε� �� ������ �ڵ� ��� ������ ����
    /// </summary>
    /// <param name="itemCode"></param>
    /// <returns></returns>
    public Item_Base FindItem(int itemCode)
    {
        Debug.Log("Call ItemFind");
        // �� ���� üũ
        if(itemCode == -1)
        {
            Debug.Log("ItemFind = Empty");
            return null;
        }

        // ������ üũ
        foreach(Item_Base item in items)
        {
            if(item.itemCode == itemCode)
            {
                Debug.Log($"ItemFind = {item}");
                return item;
            }
        }

        // �ڵ尡 �������� �ʴ� �������̶��
        Debug.Log($"�������� �ʴ� �������Դϴ�! ������ �ڵ� : {itemCode}");
        return null;
    }
}
