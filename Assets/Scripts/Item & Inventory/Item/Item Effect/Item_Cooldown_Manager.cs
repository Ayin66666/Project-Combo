using System.Collections;
using UnityEngine;

public class Item_Cooldown_Manager : MonoBehaviour
{
    [Header("---Setting---")]
    private CooldownData[] cooldownData;
    public enum Type { Equipment, Consumable_Oneoff, Consuumable_Persistence }

    private class CooldownData
    {
        public bool isCooldown;
        public float cooldown;
        public float cur;
        public Coroutine cooldownCoroutine;


        /// <summary>
        /// ��Ÿ�� ����
        /// </summary>
        /// <returns></returns>
        public IEnumerator Cooldown(float time)
        {
            cooldown = time;
            float timer = 0;
            while(timer < 1)
            {
                timer += Time.deltaTime / cooldown;
                cur = Mathf.Lerp(1, 0, timer);
                yield return null;
            }

            cur = 0;
            cooldownCoroutine = null;
        }
    }


    private void Awake()
    {
        int length = System.Enum.GetValues(typeof(Type)).Length;
        cooldownData = new CooldownData[length];
        for (int i = 0; i < length; i++)
        {
            cooldownData[i] = new CooldownData();
        }
    }

    /// <summary>
    /// �ش� ȿ�� �׷��� ��Ÿ�� üũ
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool Cooldown_Check(Type type)
    {
        // ��ٿ� üũ
        if (cooldownData[(int)type].cooldownCoroutine != null)
        {
            Debug.Log($"��Ÿ�� : {type} / {cooldownData[(int)type].cur}");
            return false;
        }

        return true;
    }

    /// <summary>
    ///���� ��Ÿ�� ����
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public float Cooldown(Type type)
    {
        return cooldownData[(int)type].cur;
    }

    /// <summary>
    /// ������, ��� ȿ�� ȣ��
    /// </summary>
    /// <param name="type"></param>
    /// <param name="coroutine"></param>
    /// <param name="cooldown"></param>
    public void EffectUse(Type type, IEnumerator coroutine, float cooldown)
    {
        // ��ٿ� ����
        cooldownData[(int)type].cooldownCoroutine = StartCoroutine(cooldownData[(int)type].Cooldown(cooldown));

        // �ɷ� ����
        StartCoroutine(coroutine);
    }
}
