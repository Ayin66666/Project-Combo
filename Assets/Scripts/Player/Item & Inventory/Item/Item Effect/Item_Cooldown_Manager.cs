using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Runtime.ConstrainedExecution;


public class Item_Cooldown_Manager : MonoBehaviour
{
    [Header("---Item Cooldown Setting---")]
    [SerializeField] private List<CooldownData> cooldown;
    private Dictionary<string, CooldownData> cooldownData;

    [System.Serializable]
    private class CooldownData
    {
        public string key;
        public float cooldown;
        public float cur;

        public Coroutine effectCoroutine;
        public Coroutine cooldownCoroutine;
        public Item_Cooldown_Manager manager;

        /// <summary>
        /// ��Ÿ�� ����
        /// </summary>
        /// <returns></returns>
        public IEnumerator Cooldown()
        {
            float timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime / cooldown;
                cur = Mathf.Lerp(1, 0, timer);
                yield return null;
            }

            cur = 0;
            manager.Cooldown_Remove(key);
        }

        public float GetElapsedTime()
        {
            return cooldown * cur;
        }
    }


    /// <summary>
    /// ������ ���� �ش� Ű�� �ִ��� üũ
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public (bool, float) Cooldown_Check(string key)
    {
        for (int i = 0; i < cooldown.Count; i++)
        {
            if (cooldown[i].key == key)
            {
                return (true, cooldown[i].GetElapsedTime());
            }
        }

        return (false, 0);
    }

    /// <summary>
    /// 0~1���� �� ��ȯ
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public float SlotUI_Cooldown(string key)
    {
        for (int i = 0; i < cooldown.Count; i++)
        {
            if (cooldown[i].key == key)
            {
                return cooldown[i].cur;
            }
        }

        return 0;
    }

    /// <summary>
    /// ��Ÿ�� ���� �� �ش� Ű ���� ����
    /// </summary>
    /// <param name="key"></param>
    public void Cooldown_Remove(string key)
    {
        for (int i = 0; i < cooldown.Count; i++)
        {
            if (cooldown[i].key == key)
            {
                cooldown.RemoveAt(i);
                return;
            }
        }
    }

    /// <summary>
    /// ��� ���� & ��Ÿ�� �ý���
    /// </summary>
    /// <param name="key"></param>
    /// <param name="coroutine"></param>
    /// <param name="cool"></param>
    public void EffectUse(string key, IEnumerator coroutine, float cool)
    {
        // �ű� ��Ÿ�� ������ �߰�
        CooldownData equipmentCooldown = new CooldownData()
        {
            key = key,
            cooldown = cool,
            cur = 0,
            manager = this
        };

        // ����Ʈ �߰�
        cooldown.Add(equipmentCooldown);

        // ȿ�� & ��Ÿ�� ����
        equipmentCooldown.effectCoroutine = StartCoroutine(coroutine);
        StartCoroutine(equipmentCooldown.Cooldown());
    }
}
