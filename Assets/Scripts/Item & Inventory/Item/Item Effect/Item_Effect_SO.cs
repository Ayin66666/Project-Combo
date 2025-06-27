using System.Collections;
using UnityEngine;


public abstract class Item_Effect_SO : ScriptableObject
{
    [Header("---Effect Setting---")]
    public string key;
    public float cooldown;


    /// <summary>
    /// ������ ��� �� ��� ���� �κ�
    /// </summary>
    public void Use()
    {
        // ��Ÿ�� üũ
        if (Player_Manager.instance.cooldown.Cooldown_Check(Item_Cooldown_Manager.Type.Weapon))
        {
            // ��� ����
            Effect();
        }
    }

    /// <summary>
    /// ��� ������
    /// </summary>
    /// <returns></returns>
    protected abstract void Effect();
}
