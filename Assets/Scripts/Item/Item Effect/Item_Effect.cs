using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item_EffectBase : ScriptableObject
{
    [Header("---Effect Setting---")]
    public string effectName;
    public string key; // 쿨다운 매니저의 딕셔너리 키
    public Effect_Timing timing;
    public float effectCooldown;
    public bool canUse;
    public enum Effect_Timing { None, NormalAll, Nomral1, Normal2, Normal3, Normal4, SmashAll, Smash1, Smash2, Smash3, Smash4, Counter, RushSlash, Special }


    /// <summary>
    /// 효과 실행 코드 - 검기 발사나 장판 같은 이펙트의 기능 구현부
    /// </summary>
    public abstract void Use(int indx);

    /// <summary>
    /// 효과가 어떤 상황에서 동작할지 셋팅하는 부분
    /// </summary>
    public void Setting(bool isOn)
    {
        // 이 부분 플레이어 매니저에 접근해서
        // 이넘(인덱스)에 알맞는 액션으로 연결하게 만드는게 맞을듯?
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
