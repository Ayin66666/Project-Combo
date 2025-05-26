using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_Manager : MonoBehaviour
{
    public static Stage_Manager instance;


    [Header("---State---")]
    [SerializeField] private bool haveStartDialog;
    [SerializeField] private bool haveStartQuest;
    [SerializeField] private bool haveStartwayPoint;
    [SerializeField] private string nextScene;


    [Header("---Dialog---")]
    [SerializeField] private List<Dialog_Data_SO> dialogDatas;


    [Header("---Quest---")]
    [SerializeField] private Quest_Data_SO questDatas;


    [Header("---Door---")]
    [SerializeField] private GameObject[] doors;


    [Header("---Component---")]
    [SerializeField] private AudioSource bgmAudio;
    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private Waypoint_Manager wayPointManager;


    [Header("---Check Point---")]
    [SerializeField] private Transform startSpawnPos;
    public Vector3 spawnPos;
    public Quaternion spawnRotation;


    // Defence
    public int curhp;
    public int maxHp;
    public int physicalDefence;
    public int magicalDefence;

    // Attack Status
    public int physcialDamage;
    public int magicalDamage;
    public int attackSpeed;
    public float criticalhit;
    public float critical_multiplier;

    // Other Status
    public float moveSpeed;
    public float curSteamina;
    public float maxSteamina;
    public float curAwankning;
    public float maxAwankning;


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

    private void Start()
    {
        Stage_Start();
    }

    public void Stage_Start()
    {
        // ���� üũ����Ʈ ����
        CheckPoint_Seting(startSpawnPos);

        // ���� ���̾�α�
        if (haveStartDialog)
        {
            Dialog(0);
        }

        // ���� ��ǥ
        if(haveStartQuest)
        {
            Quest(0);
        }

        // ��������Ʈ
        if(haveStartwayPoint)
        {
            WayPoint(true, 0);
        }
    }

    /// <summary>
    /// ���� ������ �Ѿ��
    /// </summary>
    public void Stage_End()
    {
        SceneLoad_Manager.LoadScene(nextScene);
    }

    /// <summary>
    /// ���̾�α� ȣ�� / �̹� �ڵ忡�� �������� �Ŵ����� ���� ����
    /// </summary>
    /// <param name="index">���̾�α� �ε���</param>
    public void Dialog(int index)
    {
        UI_Manager.instance.Dialog_Fight(dialogDatas[index]);
    }

    /// <summary>
    /// ����Ʈ ȣ�� / �̹� �ڵ忡�� �������� �Ŵ����� ���� ����
    /// </summary>
    /// <param name="index">����Ʈ �ε���</param>
    public void Quest(int index)
    {
        UI_Manager.instance.Quset(questDatas.Datas[index]);
    }

    /// <summary>
    /// BGM ȣ��
    /// </summary>
    /// <param name="index"></param>
    public void BGM(int index)
    {
        bgmAudio.clip = bgmClips[index];
        bgmAudio.Play();
    }

    /// <summary>
    /// �� ����
    /// </summary>
    /// <param name="index"></param>
    public void Door(bool isOn, int index)
    {
        doors[index].SetActive(isOn);
    }

    /// <summary>
    /// ��������Ʈ ȣ��
    /// </summary>
    /// <param name="isOn"></param>
    /// <param name="index"></param>
    public void WayPoint(bool isOn, int index)
    {
        wayPointManager.Waypoint_Setting(isOn, index);
    }

    /// <summary>
    /// üũ����Ʈ ���� - ȣ���� üũ����Ʈ ������Ʈ����
    /// </summary>
    public void CheckPoint_Seting(Transform pos)
    {
        // ��ġ ����
        spawnPos = pos.position;
        spawnRotation = Player_Manager.instance.transform.rotation;

        // ���� ����
        curhp = Player_Manager.instance.curhp;
        maxHp = Player_Manager.instance.maxHp;
        physicalDefence = Player_Manager.instance.physicalDefence;
        magicalDamage = Player_Manager.instance.magicalDefence;
        physcialDamage = Player_Manager.instance.physicalDamage;
        magicalDamage = Player_Manager.instance.magicalDamage;
        attackSpeed = Player_Manager.instance.attackSpeed;
        criticalhit = Player_Manager.instance.criticalhit;
        critical_multiplier = Player_Manager.instance.critical_multiplier;
        moveSpeed = Player_Manager.instance.moveSpeed;
        curSteamina = Player_Manager.instance.curStamina;
        maxSteamina = Player_Manager.instance.maxStamina;
        curAwankning = Player_Manager.instance.curAwakening;
        maxAwankning = Player_Manager.instance.maxAwakening;

        // ������ ����

        // ��ų ����
    }

    /// <summary>
    /// ��� �� üũ����Ʈ ȣ�� - ������ ��� �� �÷��̾� �Ŵ��� �κп���
    /// </summary>
    public void CheckPoint_Call()
    {
        // �������ͽ� ����
        Player_Manager.instance.curhp = curhp;
        Player_Manager.instance.maxHp = maxHp;
        Player_Manager.instance.physicalDefence = physicalDefence;
        Player_Manager.instance.magicalDefence = magicalDamage;
        Player_Manager.instance.physicalDamage = physcialDamage;
        Player_Manager.instance.magicalDamage = magicalDamage;
        Player_Manager.instance.attackSpeed = attackSpeed;
        Player_Manager.instance.criticalhit = criticalhit;
        Player_Manager.instance.critical_multiplier = critical_multiplier;
        Player_Manager.instance.moveSpeed = moveSpeed;
        Player_Manager.instance.curStamina = curSteamina;
        Player_Manager.instance.maxStamina = maxSteamina;
        Player_Manager.instance.curAwakening = curAwankning;
        Player_Manager.instance.maxAwakening = maxAwankning;


        // UI ����
        UI_Manager.instance.Hp();


        // �÷��̾� ��ġ �̵�
        Player_Manager.instance.Pos_Setting(spawnPos, spawnRotation);


        // �������� ����
    }
}
