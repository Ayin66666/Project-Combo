using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field_Rush : Field_Base
{
    [Header("---Setting---")]
    [SerializeField] private List<DialogData> datas;
    [SerializeField] private float nextRoundDelay;
    private readonly WaitForSeconds checkInterval = new WaitForSeconds(1f);
    [SerializeField] private List<GameObject> enemyList;
    private Coroutine checkCoroutine;


    public override void Field_Start()
    {
        if(checkCoroutine != null) StopCoroutine(checkCoroutine);
        checkCoroutine = StartCoroutine(StartCall());
    }

    private IEnumerator StartCall()
    {
        // 문 동작
        foreach (GameObject obj in door)
        {
            obj.SetActive(true);
        }

        // 시작 다이얼로그 체크
        if (haveStartDialog)
            UI_Manager.instance.Dialog_Fight(startDialog.dialog);

        // 맵 UI 최소화
        UI_Manager.instance.MiniMap_SizeSetting(false);

        // 몬스터 소환 - 라운드
        for (int i = 0; i < spawnDatas.Count; i++)
        {
            enemyList = new List<GameObject>();
            enemyList.Clear();

            // 몬스터 소환 - 몬스터
            for (int j = 0; j < spawnDatas[i].enemys.Count; j++)
            {
                GameObject obj = Stage_Manager.instance.enemy_Container.Spawn_Enemy(spawnDatas[i].enemys[j].enemy);
                enemyList.Add(obj);

                obj.transform.position = spawnDatas[i].enemys[j].spawnPos.position;
                obj.transform.rotation = spawnDatas[i].enemys[j].spawnPos.rotation;
                obj.SetActive(true);

                // 스폰 딜레이
                yield return new WaitForSeconds(spawnDatas[i].spawnDelay);
            }

            // 라운드 종료 대기
            enemyCount = spawnDatas[i].enemys.Count;
            while (enemyCount > 0)
            {
                // 몬스터 수 체크
                enemyList.RemoveAll(enemy => enemy.GetComponent<Enemy_Base>().curState == Enemy_Base.State.Die);
                enemyCount = enemyList.Count;

                yield return checkInterval;
            }

            // 다음 라운드 대기 시간
            yield return new WaitForSeconds(nextRoundDelay);
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

        // 문 개방
        foreach (GameObject obj in door)
        {
            obj.SetActive(false);
        }

        // 맵 UI 최대화
        UI_Manager.instance.MiniMap_SizeSetting(true);
    }

    public override void Field_Reset()
    {
        isClear = false;

        // 체크 중단
        if (checkCoroutine != null) StopCoroutine(checkCoroutine);

        // 문 개방
        foreach (GameObject door in door)
        {
            door.SetActive(false);
        }

        // 몬스터 제거
        foreach (GameObject e in enemyList)
        {
            e.GetComponent<Enemy_Base>().Reset_Enemy();
        }
    }
}
