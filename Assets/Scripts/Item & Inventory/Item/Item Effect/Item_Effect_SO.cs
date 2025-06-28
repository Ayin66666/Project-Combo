using System.Collections;
using UnityEngine;


public abstract class Item_Effect_SO : ScriptableObject
{
    [Header("---Effect Setting---")]
    public float cooldown;


    public void EffectUse()
    {
        Debug.Log("Call Effect");
        // 쿨타임 체크
        if (Player_Manager.instance.cooldown.Cooldown_Check(Item_Cooldown_Manager.Type.Equipment))
        {
            Debug.Log("Call Effect Use");
            // 기능 동작
            Effect();
        }
    }

    /// <summary>
    /// 기능 구현부
    /// </summary>
    /// <returns></returns>
    public abstract void Effect();
}
