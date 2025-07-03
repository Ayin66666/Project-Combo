using System.Collections.Generic;
using UnityEngine;


public class ItemData_Container : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private List<Item_Base> items;
    private Dictionary<int, Item_Base> itemDatas;


    private void Awake()
    {
        Data_Setting();
    }


    /// <summary>
    /// 게임 시작 시 딕셔너리에 아이템 - 키값 셋팅
    /// </summary>
    private void Data_Setting()
    {
        // 딕셔너리에 데이터 추가
        for (int i = 0; i < items.Count; i++)
        {
            // 중복 아이템 코드 체크
            Item_Base item = items[i];
            if (!itemDatas.ContainsKey(item.itemCode))
            {
                itemDatas.Add(item.itemCode, item);
            }
            else
            {
                Debug.LogWarning($"[ItemData_Container] 중복된 아이템 코드가 존재합니다: {item.itemCode}");
            }
        }
    }

    /// <summary>
    /// 데이터 로드 시 아이템 코드 기반 데이터 전달
    /// </summary>
    /// <param name="itemCode"></param>
    /// <returns></returns>
    public Item_Base FindItem(int itemCode)
    {
        // 빈 슬롯 체크
        if(itemCode == -1)
        {
            return null;
        }

        // 아이템 체크
        foreach(Item_Base item in items)
        {
            if(item.itemCode == itemCode)
            {
                return item;
            }
        }

        // 코드가 존재하지 않는 아이템이라면
        Debug.Log($"존재하지 않는 아이템입니다! 아이템 코드 : {itemCode}");
        return null;
    }
}
