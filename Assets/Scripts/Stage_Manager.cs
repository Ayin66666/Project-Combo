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
        // 시작 체크포인트 셋팅
        CheckPoint_Seting(startSpawnPos);

        // 시작 다이얼로그
        if (haveStartDialog)
        {
            Dialog(0);
        }

        // 시작 목표
        if(haveStartQuest)
        {
            Quest(0);
        }

        // 웨이포인트
        if(haveStartwayPoint)
        {
            WayPoint(true, 0);
        }
    }

    /// <summary>
    /// 다음 씬으로 넘어가기
    /// </summary>
    public void Stage_End()
    {
        SceneLoad_Manager.LoadScene(nextScene);
    }

    /// <summary>
    /// 다이얼로그 호출 / 이번 코드에선 스테이지 매니저가 전부 제어
    /// </summary>
    /// <param name="index">다이얼로그 인덱스</param>
    public void Dialog(int index)
    {
        UI_Manager.instance.Dialog_Fight(dialogDatas[index]);
    }

    /// <summary>
    /// 퀘스트 호출 / 이번 코드에선 스테이지 매니저가 전부 제어
    /// </summary>
    /// <param name="index">퀘스트 인덱스</param>
    public void Quest(int index)
    {
        UI_Manager.instance.Quset(questDatas.Datas[index]);
    }

    /// <summary>
    /// BGM 호출
    /// </summary>
    /// <param name="index"></param>
    public void BGM(int index)
    {
        bgmAudio.clip = bgmClips[index];
        bgmAudio.Play();
    }

    /// <summary>
    /// 문 개방
    /// </summary>
    /// <param name="index"></param>
    public void Door(bool isOn, int index)
    {
        doors[index].SetActive(isOn);
    }

    /// <summary>
    /// 웨이포인트 호출
    /// </summary>
    /// <param name="isOn"></param>
    /// <param name="index"></param>
    public void WayPoint(bool isOn, int index)
    {
        wayPointManager.Waypoint_Setting(isOn, index);
    }

    /// <summary>
    /// 체크포인트 저장 - 호출은 체크포인트 오브젝트에서
    /// </summary>
    public void CheckPoint_Seting(Transform pos)
    {
        // 위치 저장
        spawnPos = pos.position;
        spawnRotation = Player_Manager.instance.transform.rotation;

        // 스텟 저장
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

        // 아이템 저장

        // 스킬 저장
    }

    /// <summary>
    /// 사망 시 체크포인트 호출 - 동작은 사망 후 플레이어 매니저 부분에서
    /// </summary>
    public void CheckPoint_Call()
    {
        // 스테이터스 변경
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


        // UI 리셋
        UI_Manager.instance.Hp();


        // 플레이어 위치 이동
        Player_Manager.instance.Pos_Setting(spawnPos, spawnRotation);


        // 스테이지 리셋
    }
}
