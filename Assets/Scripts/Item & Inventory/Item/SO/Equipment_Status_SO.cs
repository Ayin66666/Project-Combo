using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item Equipment Status", menuName = "Item/Item Equipment Status", order = int.MaxValue)]
public class Equipment_Status_SO : ScriptableObject
{
    [Header("---Attack Status---")]
    [SerializeField] private int physicalDamage;
    [SerializeField] private int magicalDamage;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float criticalhit;
    [SerializeField] private float critical_multiplier;


    [Header("---Defence Status---")]
    [SerializeField] private int maxHp;
    [SerializeField] private int physicalDefence;
    [SerializeField] private int magicalDefence;


    [Header("---Other Status---")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxStamina;
    [SerializeField] private float maxAwakening;
    [SerializeField] private float staminaRecovery;


    public int PhysicalDamage { get { return physicalDamage; } set { physicalDamage = value; } }
    public int MagicalDamage { get { return magicalDamage; } set { magicalDamage = value; } }
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    public float CriticalHit { get { return criticalhit; } set { criticalhit = value; } }
    public float CriticalMultiplier { get { return critical_multiplier; } set { critical_multiplier = value; } }

    public int MaxHp { get { return maxHp; } set { maxHp = value; } }
    public int PhysicalDefence { get { return physicalDefence; } set { physicalDefence = value; } }
    public int MagicalDefence { get { return magicalDefence; } set { magicalDefence = value; } }

    public float MoveSpeed { get { return moveSpeed; } set {  moveSpeed = value; } }
    public float MaxStamina { get {  return maxStamina; } set { maxStamina = value; } }
    public float MaxAwakening {  get { return maxAwakening; } set { maxAwakening = value; } }
    public float StaminaRecovery {  get { return staminaRecovery; } set { staminaRecovery = value; } }
}
