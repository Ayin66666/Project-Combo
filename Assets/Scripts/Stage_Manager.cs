using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_Manager : MonoBehaviour
{
    public static Stage_Manager instance;

    [Header("---Stage Data---")]
    [SerializeField] private int chapter;
    [SerializeField] private int stage;
    [SerializeField] private StageData_SO stageData;
    private float startTime;


    [Header("---State---")]
    [SerializeField] private bool haveStartDialog;
    [SerializeField] private bool haveStartQuest;
    [SerializeField] private bool haveStartwayPoint;
    [SerializeField] private string nextScene;


    [Header("---Spawn---")]
    [SerializeField] private List<Field_Base> spawn;


    [Header("---Dialog---")]
    [SerializeField] private List<Dialog_Data_SO> dialogDatas;


    [Header("---Quest---")]
    [SerializeField] private Quest_Data_SO questDatas;


    [Header("---Door---")]
    [SerializeField] private GameObject[] doors;


    [Header("---Component---")]
    public Enemy_Container enemy_Container;
    [SerializeField] private AudioSource bgmAudio;
    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private Waypoint_Manager wayPointManager;


    [Header("---Check Point---")]
    [SerializeField] private Transform startSpawnPos;
    public Vector3 spawnPos;
    public Quaternion spawnRotation;


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
        // ���̵� ����
        UI_Manager.instance.Fade(false, 1.5f);

        // �÷��̾� Ȱ��ȭ
        Player_Manager.instance.Player_Action_Setting(true);
        Player_Manager.instance.PlayerPos_Setting(spawnPos);
        Player_Manager.instance.PlayerOnOff_Setting(true);

        // ���� üũ����Ʈ ����
        CheckPoint_Seting(startSpawnPos);

        // ���� ���̾�α�
        if (haveStartDialog)
        {
            Dialog(0);
        }

        // ���� ��ǥ
        if (haveStartQuest)
        {
            Quest(0);
        }

        // ��������Ʈ
        if (haveStartwayPoint)
        {
            WayPoint(true, 0);
        }

        // Ÿ�̸� ����
        startTime = Time.time;
    }

    /// <summary>
    /// ���� ������ �Ѿ��
    /// </summary>
    public void Stage_End()
    {
        StartCoroutine(StageEndCall());
    }

    private IEnumerator StageEndCall()
    {
        // �÷��̾� ���� ��Ȱ��ȭ
        Player_Manager.instance.Player_Action_Setting(false);

        // Ŭ���� ����ġ
        Player_Manager.instance.status.ExpAdd(stageData.ClearExp);

        // Ŭ���� ������ ����
        ClearData();

        // Ŭ���� UI - �� �Լ� ���ο� Fade ���۵� ���� ����!
        UI_Manager.instance.StageClearUI();
        while (UI_Manager.instance.isClear)
        {
            yield return null;
        }

        // �÷��̾� ��Ȱ��ȭ
        Player_Manager.instance.PlayerOnOff_Setting(false);

        // �� �̵�
        SceneLoad_Manager.LoadScene(nextScene);
    }

    /// <summary>
    /// ���� Ŭ���� ������ �ð� & ��ũ ����
    /// </summary>
    public void ClearData()
    {
        // ������ ����
        StageData clearData = new StageData
        {
            isClear = true,
            clearTime = Time.time - startTime,
            clearRank = StageData.Rank.N,
        };

        // ��ũ ����
        for (int i = 0; i < stageData.Data.Count; i++)
        {
            if (clearData.clearTime < stageData.Data[i].time)
                clearData.clearRank = stageData.Data[i].rank;
        }

        // ������ ����
        ChapterData_Manager.instance.Data_Updata(chapter, stage, clearData);
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
    }

    /// <summary>
    /// ��� �� üũ����Ʈ ȣ�� - ������ ��� �� �÷��̾� �Ŵ��� �κп���
    /// </summary>
    public void CheckPoint_Call()
    {
        // �������� ����
        foreach (Field_Base field in spawn)
        {
            if (!field.isClear) field.Field_Reset();
        }

        // �������ͽ� ����
        Player_Manager.instance.status.curhp = Player_Manager.instance.status.maxHp;
        Player_Manager.instance.status.curStamina = Player_Manager.instance.status.maxStamina;
        Player_Manager.instance.status.curAwakening = 0;

        // UI ����
        UI_Manager.instance.Hp();
        UI_Manager.instance.Awakening();

        // �÷��̾� ��ġ �̵�
        PlayerAction_Manager.instance.Pos_Setting(spawnPos, spawnRotation);
    }
}
