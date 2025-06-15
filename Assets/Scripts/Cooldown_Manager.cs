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
    /// �ڷ�ƾ & ��Ÿ�� ���� ȣ�� ����
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
    /// ��ũ���ͺ� ������Ʈ�� ������ ������ �� �ڷ�ƾ ��� �븮��
    /// </summary>
    /// <param name="key">�ڷ�ƾ�� �����ϴ� ��</param>
    /// <param name="coroutine">������ �ڷ�ƾ</param>
    public void Coroutine_Delegate(string key, IEnumerator coroutine)
    {
        if (coroutine_Dictionary.ContainsKey(key)) return;

        coroutine_Dictionary.Add(key, StartCoroutine(coroutine));
    }

    /// <summary>
    /// ������ ���� �����ϴ� ��Ÿ�� ����
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
    /// ���� ��Ÿ�� ���� �κ�
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private IEnumerator CooldownCoroutine(string key)
    {
        yield return new WaitForSeconds(cooldowns[key].duration);
        Remove(key);
    }

    /// <summary>
    /// �ش� �������� ��Ÿ���� ���������� üũ
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool IsCooldownActive(string key)
    {
        if (!cooldowns.TryGetValue(key, out var data)) return false;
        return data.IsActive();
    }

    /// <summary>
    /// ���� ��Ÿ�� �� �޾ƿ��� - ��Ʈ�� ���� UI ��
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public float GetCooldown(string key)
    {
        if (!cooldowns.TryGetValue(key, out var data)) return 0f;
        return data.GetRemainingTime();
    }

    /// <summary>
    /// ���� ���� �� ��ųʸ����� ����
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
