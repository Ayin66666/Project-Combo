using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field_Normal : Field_Base
{
    [Header("---Setting---")]
    [SerializeField] private bool haveDialog;
    [SerializeField] private List<DialogData> countDialogData;


    [Header("---Enemy Check---")]
    [SerializeField] private List<Enemy_Base> enemyList;
    private Coroutine checkCoroutine;
    private readonly WaitForSeconds checkInterval = new WaitForSeconds(1f);


    public override void Field_Start()
    {
        if (checkCoroutine != null) StopCoroutine(checkCoroutine);
        checkCoroutine = StartCoroutine(StartCall());
    }

    private IEnumerator StartCall()
    {
        // 문 동작
        if(door != null)
        {
            foreach (GameObject obj in door)
            {
                obj.SetActive(true);
            }
        }

        // 시작 다이얼로그 체크
        if (haveStartDialog)
            UI_Manager.instance.Dialog_Fight(startDialog.dialog);

        // 맵 UI 최소화
        UI_Manager.instance.MiniMap_SizeSetting(false);

        // 에너미 소환
        enemyList = new List<Enemy_Base>();
        enemyCount = spawnDatas[0].enemys.Count;
        for (int i = 0; i < spawnDatas[0].enemys.Count; i++)
        {
            GameObject obj = Stage_Manager.instance.enemy_Container.Spawn_Enemy(spawnDatas[0].enemys[i].enemy);
            enemyList.Add(obj.GetComponent<Enemy_Base>());

            obj.transform.position = spawnDatas[0].enemys[i].spawnPos.position;
            obj.transform.rotation = spawnDatas[0].enemys[i].spawnPos.rotation;
            obj.SetActive(true);

            // 스폰 딜레이
            yield return new WaitForSeconds(spawnDatas[0].spawnDelay);
        }

        // 체크 동작
        StartCoroutine(CheckCall());
    }

    private IEnumerator CheckCall()
    {
        // 몬스터 수 체크
        enemyCount = enemyList.Count;

        // 종료 대기
        while (enemyCount > 0)
        {
            // 몬스터 체크
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList[i].curState == Enemy_Base.State.Die || !enemyList[i].gameObject.activeSelf)
                    enemyList.RemoveAt(i);
            }
            enemyCount = enemyList.Count;

            // 다이얼로그 체크
            if (haveDialog)
            {
                for (int i = countDialogData.Count; i >= 0; i--)
                {
                    if (countDialogData[i].useCount <= enemyCount && !countDialogData[i].isUsed)
                    {
                        countDialogData[i].isUsed = true;
                        UI_Manager.instance.Dialog_Fight(countDialogData[i].dialog);
                    }
                }
            }

            // 체크 딜레이
            yield return checkInterval;
        }

        // 필드 종료
        Field_End();
    }

    public override void Field_End()
    {
        isClear = true;

        // 클리어 UI
        UI_Manager.instance.FieldClearUI(UI_Manager.ClearType.Normal);

        // 종료 다이얼로그
        if (haveEndDialog)
            UI_Manager.instance.Dialog_Fight(endDialog.dialog);

        // 맵 UI 최대화
        UI_Manager.instance.MiniMap_SizeSetting(true);

        // 문 개방
        foreach (GameObject obj in door)
        {
            obj.SetActive(false);
        }
    }

    public override void Field_Reset()
    {
        isClear = false;

        // 체크 중단
        if (checkCoroutine != null) StopCoroutine(checkCoroutine);

        // 문 개방
        foreach(GameObject door in door)
        {
            door.SetActive(false);
        }

        // 몬스터 제거
        foreach(Enemy_Base e in enemyList)
        {
            e.Reset_Enemy();
        }
    }
}
