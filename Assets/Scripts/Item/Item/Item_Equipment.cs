using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "Item Data / Item Equipment", order = int.MaxValue)]
public class Item_Equipment : Item_Base
{
    [Header("---Equipment Setting---")]
    [SerializeField] private PartType partType;
    [SerializeField] private Item_EffectBase effects;
    public enum PartType { Head, body, Pants, Shoes, Weapon, Accessories }


    [Header("---Status Data---")]
    [SerializeField] private GameObject status;


    public override void Use()
    {
        throw new System.NotImplementedException();
    }
}
