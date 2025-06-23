using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item Equipment", menuName = "Item/Item Equipment", order = int.MaxValue)]
public class Item_Equipment : Item_Base
{
    [Header("---Equipment Setting---")]
    public EquipmentType equipmentType;
    public ItemStatus equipment_Status;
    public bool haveEffect;
    public List<Item_Effect_SO> effectList; // ��ũ��Ʈ �߰� �ʿ�
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
    /// ��� ����
    /// </summary>
    public override void Use()
    {
        Debug.Log("��� ����");

        // �������ͽ� ����
        Player_Manager.instance.status.Equipment_Status_Setting(true, equipment_Status);
    }

    /// <summary>
    /// ��� ����
    /// </summary>
    public void Unequip()
    {
        // �������ͽ� ����
        Player_Manager.instance.status.Equipment_Status_Setting(false, equipment_Status);
    }

    /// <summary>
    /// ������ ȿ��
    /// </summary>
    public void Effect()
    {

    }
}
