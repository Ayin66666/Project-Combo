using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field_Boss : Field_Base
{
    [Header("---Boss Setting---")]
    [SerializeField] private EnemyData[] enemyData;
    [SerializeField] private Enemy_Base curEnemy;
    private Coroutine stageCoroutine;


    [Header("---Delay Setting---")]
    [SerializeField] private float delayTime;
    private WaitForSeconds delay;


    [System.Serializable]
    public struct EnemyData
    {
        [SerializeField] private string DataName;
        public GameObject enemy;
        public Transform spawnPos;
        public List<Dialog> dialogs;
    }

    [System.Serializable]
    public class Dialog
    {
        [SerializeField] private string dialogName;
        public bool isUsed;
        public int hp;
        public Dialog_Data_SO dialog;
    }


    private void Awake()
    {
        delay = new WaitForSeconds(delayTime);
    }

    public override void Field_Start()
    {
        // 시작
        stageCoroutine = StartCoroutine(Field_Check());
    }

    private IEnumerator Field_Check()
    {
        // 맵 UI 최소화 & 문 폐쇄
        UI_Manager.instance.MiniMap_SizeSetting(false);
        Door_Setting(true);

        // 시작 다이얼로그
        if (haveStartDialog)
            UI_Manager.instance.Dialog_Fight(startDialog.dialog);

        // 소환 로직 - 가지고 있는 몬스터 수만큼 소환
        for (int i = 0; i < enemyData.Length; i++)
        {
            // 몬스터 소환
            GameObject enemy = Instantiate(enemyData[i].enemy, enemyData[i].spawnPos.position, Quaternion.identity);
            curEnemy = enemy.GetComponent<Enemy_Base>();
            curEnemy.Spawn();

            // 스폰 컷신 대기
            while (curEnemy.isCutScene)
            {
                yield return null;
            }

            // 스테이지 체크
            while (enemy != null)
            {
                // 다이얼로그 체크
                Dialog_Check(i);

                // 체크 딜레이
                yield return delay;
            }
        }

        // 몬스터 처지 완료 시 로직
        Field_End();
    }

    private void Dialog_Check(int index)
    {
        for (int j = 0; j < enemyData[index].dialogs.Count; j++)
        {
            if (curEnemy.curHp <= enemyData[index].dialogs[j].hp && !enemyData[index].dialogs[j].isUsed)
            {
                enemyData[index].dialogs[j].isUsed = true;
                UI_Manager.instance.Dialog_Fight(enemyData[index].dialogs[j].dialog);
            }
        }
    }

    public override void Field_End()
    {
        isClear = true;

        // 클리어 UI
        UI_Manager.instance.FieldClearUI(UI_Manager.ClearType.Boss);

        // 맵 UI 최대화
        UI_Manager.instance.MiniMap_SizeSetting(true);

        // 종료 다이얼로그
        if (haveEndDialog)
            UI_Manager.instance.Dialog_Fight(endDialog.dialog);

        // 문 개방
        Door_Setting(false);
    }

    public override void Field_Reset()
    {
        isClear = false;

        // 스테이지 체크 종료
        if (stageCoroutine != null) 
            StopCoroutine(stageCoroutine);

        // 몬스터 파괴
        Destroy(curEnemy.gameObject);

        // 문 개방
        Door_Setting(false);
    }
}
