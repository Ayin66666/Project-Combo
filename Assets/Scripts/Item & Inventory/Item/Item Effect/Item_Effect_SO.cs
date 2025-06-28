using System.Collections;
using UnityEngine;


public abstract class Item_Effect_SO : ScriptableObject
{
    [Header("---Effect Setting---")]
    public float cooldown;


    public void EffectUse()
    {
        Debug.Log("Call Effect");
        // ��Ÿ�� üũ
        if (Player_Manager.instance.cooldown.Cooldown_Check(Item_Cooldown_Manager.Type.Equipment))
        {
            Debug.Log("Call Effect Use");
            // ��� ����
            Effect();
        }
    }

    /// <summary>
    /// ��� ������
    /// </summary>
    /// <returns></returns>
    public abstract void Effect();
}
