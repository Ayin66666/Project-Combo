using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Status Data", menuName = "Scriptable Object/Status Data", order = int.MaxValue)]
public class Character_Status_SO : ScriptableObject
{
    [SerializeField] private string objectName;
    public string ObjectName { get { return objectName; } private set { objectName = value; } }


    [SerializeField] private int phyScial_Damage;
    public int PhyScial_Damage { get { return phyScial_Damage; } private set { phyScial_Damage = value; } }


    [SerializeField] private int magical_Damage;
    public int Magical_Damage { get { return magical_Damage; } private set { magical_Damage = value; } }


    [SerializeField] private int critical_hit;
    public int Critical_hit { get { return critical_hit; } private set { critical_hit = value; } }


    [SerializeField] private float critical_multiplier;
    public float Critical_multiplier { get { return critical_multiplier; } private set { critical_multiplier = value; } }


    [SerializeField] private float attackSpeed;
    public float AttackSpeed { get {  return attackSpeed; } private set {  attackSpeed = value; } }



    [SerializeField] private int hp;
    public int Hp { get { return hp; } private set { hp = value; } }


    [SerializeField] private float groggy;
    public float Groggy { get {  return groggy; } private set {  groggy = value; } }


    [SerializeField] private int physical_Defence;
    public int Physical_Defence { get { return physical_Defence; } private set { physical_Defence = value; } }


    [SerializeField] private int magical_Defence;
    public int Magical_Defence { get { return magical_Defence; } private set { magical_Defence = value; } }



    [SerializeField] private float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } private set { moveSpeed = value; } }


    [SerializeField] private float groggyTime;
    public float GroggyTime { get {  return groggyTime; } private set {  groggyTime = value; } }
}
