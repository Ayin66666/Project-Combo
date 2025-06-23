using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item Equipment", menuName = "Item/Item Equipment", order = int.MaxValue)]
public class Item_Equipment : Item_Base
{
    [Header("---Equipment Setting---")]
    public EquipmentType equipmentType;
    public ItemStatus equipment_Status;
    public bool haveEffect;
    public List<Item_Effect_SO> effectList; // 스크립트 추가 필요
    public enum EquipmentType { Weapon, Head, Body, Pants, Shoes, Core }

    [System.Serializable]
    public struct ItemStatus
    {
        [Header("---Attack Status---")]
        public int physicalDamage;
        public int magicalDamage;
        public float attackSpeed;
        public float criticalhit;
        public float critical_multiplier;


        [Header("---Defence Status---")]
        [SerializeField] public int maxHp;
        public int physicalDefence;
        public int magicalDefence;


        [Header("---Other Status---")]
        [SerializeField] public float moveSpeed;
        public float maxStamina;
        public float maxAwakening;
        public float staminaRecovery;
    }


    /// <summary>
    /// 장비 착용
    /// </summary>
    public override void Use()
    {
        Debug.Log("장비 착용");

        // 스테이터스 증가
        Player_Manager.instance.status.Equipment_Status_Setting(true, equipment_Status);
    }

    /// <summary>
    /// 장비 해제
    /// </summary>
    public void Unequip()
    {
        // 스테이터스 감소
        Player_Manager.instance.status.Equipment_Status_Setting(false, equipment_Status);
    }

    /// <summary>
    /// 아이템 효과
    /// </summary>
    public void Effect()
    {

    }
}
