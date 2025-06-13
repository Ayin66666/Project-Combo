using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Item_Base : ScriptableObject
{
    /*
     * 아이템 스크립트 제작 중!
     * 이거 내 생각에 아이템 효과를 호출하는데 사용하는 상속용 스크립트를 만들고 ()
     * 그걸 상속받는 개별 아이템 효과 스크립터블 오브젝트를 제작
    */
    [Header("---Status---")]
    public string itemName; // 아이템 이름
    public int itemCode; // 아이템 코드
    [TextArea] public string itemDescription; // 아이템 배경 설명
    [TextArea] public string itemEffectDescription; // 아이템 효과 설명
    public ItemType itemType; // 아이템 종류는 장비, 소모, 잡화 3종
    public ItemRating itemRating; // 아이템 레이팅은 1티어가 가장 높음
    public int maxStackCount; // 아이템 최대 스택 갯수
    public bool haveEffect;
    public enum ItemType { Equipment, Consumable, Other }
    public enum ItemRating { Tier1, Tier2, Tier3, Tier4, Tier5 }


    [Header("---UI---")]
    public Sprite icon;


    public abstract void Use();

    /// <summary>
    /// 장비 아이템의 특수효과?
    /// </summary>
    public void ItemEffect()
    {
        if(haveEffect)
        {

        }
    }
}
