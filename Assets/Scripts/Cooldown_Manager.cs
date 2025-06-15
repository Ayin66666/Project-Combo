using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooldown_Manager : MonoBehaviour
{
    public static Cooldown_Manager instance;

    [Header("---Setting---")]
    private Dictionary<string, Coroutine> coroutine_Dictionary = new();
    private Dictionary<string, CooldownData> cooldowns = new();
    private class CooldownData
    {
        public Coroutine coroutine;
        public float duration;
        public float startTime;

        public float GetRemainingTime()
        {
            float elapsed = Time.time - startTime;
            return Mathf.Max(0f, duration - elapsed);
        }

        public bool IsActive()
        {
            return GetRemainingTime() > 0f;
        }
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    /// <summary>
    /// 코루틴 & 쿨타임 동시 호출 로직
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cooldown"></param>
    /// <param name="coroutine"></param>
    public void StartConsumableRoutine(string key, float cooldown, IEnumerator coroutine)
    {
        if (IsCooldownActive(key)) return;

        Coroutine_Cooldown(key, cooldown);
        Coroutine_Delegate(key, coroutine);
    }

    /// <summary>
    /// 스크립터블 오브젝트로 구현한 아이템 내 코루틴 기능 대리자
    /// </summary>
    /// <param name="key">코루틴에 접근하는 값</param>
    /// <param name="coroutine">동작할 코루틴</param>
    public void Coroutine_Delegate(string key, IEnumerator coroutine)
    {
        if (coroutine_Dictionary.ContainsKey(key)) return;

        coroutine_Dictionary.Add(key, StartCoroutine(coroutine));
    }

    /// <summary>
    /// 아이템 내에 존재하는 쿨타임 동작
    /// </summary>
    /// <param name="key"></param>
    /// <param name="duration"></param>
    public void Coroutine_Cooldown(string key, float duration)
    {
        if (cooldowns.ContainsKey(key)) return;

        var data = new CooldownData
        {
            duration = duration,
            startTime = Time.time,
            coroutine = StartCoroutine(CooldownCoroutine(key))
        };
        cooldowns[key] = data;
    }

    /// <summary>
    /// 실제 쿨타임 동작 부분
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private IEnumerator CooldownCoroutine(string key)
    {
        yield return new WaitForSeconds(cooldowns[key].duration);
        Remove(key);
    }

    /// <summary>
    /// 해당 아이템의 쿨타임이 동작중인지 체크
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool IsCooldownActive(string key)
    {
        if (!cooldowns.TryGetValue(key, out var data)) return false;
        return data.IsActive();
    }

    /// <summary>
    /// 남은 쿨타임 값 받아오기 - 쇼트컷 슬롯 UI 용
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public float GetCooldown(string key)
    {
        if (!cooldowns.TryGetValue(key, out var data)) return 0f;
        return data.GetRemainingTime();
    }

    /// <summary>
    /// 동작 종료 후 딕셔너리에서 제거
    /// </summary>
    /// <param name="key"></param>
    public void Remove(string key)
    {
        if (coroutine_Dictionary.ContainsKey(key))
        {
            StopCoroutine(coroutine_Dictionary[key]);
            coroutine_Dictionary.Remove(key);
        }
        cooldowns.Remove(key);
    }
}
