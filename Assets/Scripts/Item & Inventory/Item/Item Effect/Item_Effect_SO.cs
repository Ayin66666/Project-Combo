using UnityEngine;


public abstract class Item_Effect_SO : ScriptableObject
{
    [Header("---Effect Setting---")]
    [SerializeField] private string key;
    [SerializeField] private float cooldown;

    public string Key { get { return key; } private set { key = value; } }
    public float Cooldown { get { return cooldown; } private set { cooldown = value; } }


    /// <summary>
    /// 기능 구현부
    /// </summary>
    /// <returns></returns>
    public abstract void Effect();
}
