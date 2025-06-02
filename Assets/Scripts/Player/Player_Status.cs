using UnityEngine;

public class Player_Status : MonoBehaviour
{
    public static Player_Status instacne;


    [Header("---Status---")]
    public int level;

    // Defence Status
    public int curhp;
    public int maxHp;
    public int physicalDefence;
    public int magicalDefence;

    // Attack Status
    public int physicalDamage;
    public int magicalDamage;
    public float attackSpeed;
    public float criticalhit;
    public float critical_multiplier;

    // Other Status
    public float moveSpeed;
    public float curStamina;
    public float maxStamina;
    public float curAwakening;
    public float maxAwakening;


    private void Awake()
    {
        if (instacne == null)
        {
            instacne = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Update()
    {
        Recovery();
    }

    /// <summary>
    /// ���� ���� �� ������ �ε� �� ����
    /// </summary>
    /// <param name="data"></param>
    public void Status_Setting(Data data)
    {
        curhp = data.curhp;
        maxHp = data.maxHp;
        physicalDefence = data.physicalDefence;
        magicalDefence = data.magicalDefence;

        physicalDamage = data.physicalDamage;
        magicalDamage = data.magicalDamage;
        criticalhit = data.criticalhit;
        critical_multiplier = data.critical_multiplier;
        attackSpeed = data.attackSpeed;

        moveSpeed = data.moveSpeed;
        curStamina = data.curStamina;
        maxStamina = data.maxStamina;
        curAwakening = data.curAwakening;
        maxAwakening = data.maxAwakening;
    }

    /// <summary>
    /// ���׹̳� ȸ��
    /// </summary>
    private void Recovery()
    {
        if (curStamina < maxStamina)
            curStamina += Time.deltaTime * 5f;
    }

    /// <summary>
    /// ���� ������ ȸ��
    /// </summary>
    /// <param name="index"></param>
    public void AwankingAdd(int index)
    {
        curAwakening += index;
        if (curAwakening >= maxAwakening)
        {
            // �ִ�ġ�� ���� ���
            curAwakening = maxAwakening;

            // ���� Ȱ��ȭ
            if (!Player_Manager.instance.canAwakning)
                Player_Manager.instance.canAwakning = true;
        }
    }
}
