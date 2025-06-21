using System.Collections;
using UnityEngine;


public abstract class Item_Effect_SO : ScriptableObject
{
    [Header("---Effect Setting---")]
    public string key;
    public float cooldown;


    /// <summary>
    /// 아이템 사용 시 기능 구현 부분
    /// </summary>
    public void Use()
    {
        // 쿨타임 체크
        if (!Cooldown_Manager.instance.IsCooldownActive(key))
        {
            // 효과 동작
            Effect();
        }
    }

    /// <summary>
    /// 기능 구현부
    /// </summary>
    /// <returns></returns>
    protected abstract void Effect();
}
