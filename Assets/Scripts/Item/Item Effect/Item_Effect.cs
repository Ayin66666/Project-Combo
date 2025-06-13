using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item_EffectBase : ScriptableObject
{
    [Header("---Effect Setting---")]
    public string effectName;
    public string key; // ��ٿ� �Ŵ����� ��ųʸ� Ű
    public Effect_Timing timing;
    public float effectCooldown;
    public bool canUse;
    public enum Effect_Timing { None, NormalAll, Nomral1, Normal2, Normal3, Normal4, SmashAll, Smash1, Smash2, Smash3, Smash4, Counter, RushSlash, Special }


    /// <summary>
    /// ȿ�� ���� �ڵ� - �˱� �߻糪 ���� ���� ����Ʈ�� ��� ������
    /// </summary>
    public abstract void Use(int indx);

    /// <summary>
    /// ȿ���� � ��Ȳ���� �������� �����ϴ� �κ�
    /// </summary>
    public void Setting(bool isOn)
    {
        // �� �κ� �÷��̾� �Ŵ����� �����ؼ�
        // �̳�(�ε���)�� �˸´� �׼����� �����ϰ� ����°� ������?
    }

    protected IEnumerator TimerCall()
    {
        float startTime = Time.time;
        float addTime = effectCooldown;
        yield return new WaitUntil(() => Time.time >= startTime + addTime);
        canUse = true;

        Player_Manager.instance.cooldown.Remove(key);
    }
}
