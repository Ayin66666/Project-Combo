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
        // 페이드 종료
        UI_Manager.instance.Fade(false, 1.5f);

        // 플레이어 활성화
        Player_Manager.instance.Player_Action_Setting(true);
        Player_Manager.instance.PlayerPos_Setting(spawnPos);
        Player_Manager.instance.PlayerOnOff_Setting(true);

        // 시작 체크포인트 셋팅
        CheckPoint_Seting(startSpawnPos);

        // 시작 다이얼로그
        if (haveStartDialog)
        {
            Dialog(0);
        }

        // 시작 목표
        if (haveStartQuest)
        {
            Quest(0);
        }

        // 웨이포인트
        if (haveStartwayPoint)
        {
            WayPoint(true, 0);
        }

        // 타이머 동작
        startTime = Time.time;
    }

    /// <summary>
    /// 다음 씬으로 넘어가기
    /// </summary>
    public void Stage_End()
    {
        StartCoroutine(StageEndCall());
    }

    private IEnumerator StageEndCall()
    {
        // 플레이어 동작 비활성화
        Player_Manager.instance.Player_Action_Setting(false);

        // 클리어 경험치
        Player_Manager.instance.status.ExpAdd(stageData.ClearExp);

        // 클리어 데이터 셋팅
        ClearData();

        // 클리어 UI - 이 함수 내부에 Fade 동작도 같이 있음!
        UI_Manager.instance.StageClearUI();
        while (UI_Manager.instance.isClear)
        {
            yield return null;
        }

        // 플레이어 비활성화
        Player_Manager.instance.PlayerOnOff_Setting(false);

        // 씬 이동
        SceneLoad_Manager.LoadScene(nextScene);
    }

    /// <summary>
    /// 게임 클리어 시점의 시간 & 랭크 셋팅
    /// </summary>
    public void ClearData()
    {
        // 데이터 생성
        StageData clearData = new StageData
        {
            isClear = true,
            clearTime = Time.time - startTime,
            clearRank = StageData.Rank.N,
        };

        // 랭크 셋팅
        for (int i = 0; i < stageData.Data.Count; i++)
        {
            if (clearData.clearTime < stageData.Data[i].time)
                clearData.clearRank = stageData.Data[i].rank;
        }

        // 데이터 전달
        ChapterData_Manager.instance.Data_Updata(chapter, stage, clearData);
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
    }

    /// <summary>
    /// 사망 시 체크포인트 호출 - 동작은 사망 후 플레이어 매니저 부분에서
    /// </summary>
    public void CheckPoint_Call()
    {
        // 스테이지 리셋
        foreach (Field_Base field in spawn)
        {
            if (!field.isClear) field.Field_Reset();
        }

        // 스테이터스 변경
        Player_Manager.instance.status.curhp = Player_Manager.instance.status.maxHp;
        Player_Manager.instance.status.curStamina = Player_Manager.instance.status.maxStamina;
        Player_Manager.instance.status.curAwakening = 0;

        // UI 리셋
        UI_Manager.instance.Hp();
        UI_Manager.instance.Awakening();

        // 플레이어 위치 이동
        PlayerAction_Manager.instance.Pos_Setting(spawnPos, spawnRotation);
    }
}
