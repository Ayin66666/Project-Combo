using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item Equipment", menuName = "Item/Item Equipment", order = int.MaxValue)]
public class Item_Equipment : Item_Base
{
    [Header("---Equipment Status---")]
    public EquipmentType equipmentType;
    public List<Equipment_Status> equipment_Status;


    [Header("---Equipment Effect---")]
    public bool haveEffect;
    public List<Item_Effect_SO> effectList;


    [Header("---Epuipment UI---")]
    [TextArea] public string statusText;
    [TextArea] public string damageText;

    [HideInInspector] public string[] typeText = new string[] 
    { 
        "����", "�Ӹ�", "����", "����", "�Ź�", "�ھ�" 
    };

    [HideInInspector] public string[] effectText = new string[]
    {
        "���� ���ݷ�", "���� ���ݷ�", "ġ��Ÿ Ȯ��", "ġ��Ÿ ����",
        "�ִ� ü��", "���� ����", "���� ����",
        "�̵� �ӵ�", "���׹̳� ȸ����"
    };

    public enum EquipmentType { Weapon, Head, Body, Pants, Shoes, Core }
    public enum StatusType 
    { 
        PDamage, MDamage, Criticalhit, Critical_multiplier, 
        MaxHp, PhysicalDefence, MagicalDefence, 
        MoveSpeed, StaminaRecovery
    };

    [System.Serializable]
    public struct Equipment_Status
    {
        public StatusType type;
        public float value;
    }


    /// <summary>
    /// ��� ����
    /// </summary>
    public override void Use()
    {
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
