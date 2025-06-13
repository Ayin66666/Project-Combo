using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Item_Base : ScriptableObject
{
    /*
     * ������ ��ũ��Ʈ ���� ��!
     * �̰� �� ������ ������ ȿ���� ȣ���ϴµ� ����ϴ� ��ӿ� ��ũ��Ʈ�� ����� ()
     * �װ� ��ӹ޴� ���� ������ ȿ�� ��ũ���ͺ� ������Ʈ�� ����
    */
    [Header("---Status---")]
    public string itemName; // ������ �̸�
    public int itemCode; // ������ �ڵ�
    [TextArea] public string itemDescription; // ������ ��� ����
    [TextArea] public string itemEffectDescription; // ������ ȿ�� ����
    public ItemType itemType; // ������ ������ ���, �Ҹ�, ��ȭ 3��
    public ItemRating itemRating; // ������ �������� 1Ƽ� ���� ����
    public int maxStackCount; // ������ �ִ� ���� ����
    public bool haveEffect;
    public enum ItemType { Equipment, Consumable, Other }
    public enum ItemRating { Tier1, Tier2, Tier3, Tier4, Tier5 }


    [Header("---UI---")]
    public Sprite icon;


    public abstract void Use();

    /// <summary>
    /// ��� �������� Ư��ȿ��?
    /// </summary>
    public void ItemEffect()
    {
        if(haveEffect)
        {

        }
    }
}
