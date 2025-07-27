using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field_Rush : Field_Base
{
    [Header("---Setting---")]
    [SerializeField] private List<DialogData> datas;
    [SerializeField] private float nextRoundDelay;
    private readonly WaitForSeconds checkInterval = new WaitForSeconds(1f);
    private List<Enemy_Base> enemyList;
    private Coroutine checkCoroutine;


    public override void Field_Start()
    {
        if(checkCoroutine != null) StopCoroutine(checkCoroutine);
        checkCoroutine = StartCoroutine(StartCall());
    }

    private IEnumerator StartCall()
    {
        // �� ����
        foreach (GameObject obj in door)
        {
            obj.SetActive(true);
        }

        // ���� ���̾�α� üũ
        if (haveStartDialog)
            UI_Manager.instance.Dialog_Fight(startDialog.dialog);

        // �� UI �ּ�ȭ
        UI_Manager.instance.MiniMap_SizeSetting(false);

        // ���� ��ȯ - ����
        enemyList = new List<Enemy_Base>();
        for (int i = 0; i < datas.Count; i++)
        {
            enemyList.Clear();

            // ���� ��ȯ - ����
            enemyCount = spawnDatas[i].enemys.Count;
            for (int j = 0; j < spawnDatas[0].enemys.Count; j++)
            {
                GameObject obj = Stage_Manager.instance.enemy_Container.Spawn_Enemy(spawnDatas[0].enemys[j].enemy);
                enemyList.Add(obj.GetComponent<Enemy_Base>());

                obj.transform.position = spawnDatas[0].enemys[j].spawnPos.position;
                obj.transform.rotation = spawnDatas[0].enemys[j].spawnPos.rotation;
                obj.SetActive(true);

                // ���� ������
                yield return new WaitForSeconds(spawnDatas[i].spawnDelay);
            }

            // ���� ���� ���
            while (enemyCount == 0)
            {
                // ���� �� üũ
                for (int j = 0; i < enemyList.Count; i++)
                {
                    if (enemyList[i].curState == Enemy_Base.State.Die || !enemyList[i].gameObject.activeSelf)
                        enemyList.RemoveAt(i);
                }
                enemyCount = enemyList.Count;

                yield return checkInterval;
            }

            // ���� ���� ��� �ð�
            yield return new WaitForSeconds(nextRoundDelay);
        }

        // �ʵ� ����
        Field_End();
    }

    public override void Field_End()
    {
        isClear = true;

        // Ŭ���� UI
        UI_Manager.instance.FieldClearUI(UI_Manager.ClearType.Normal);

        // ���� ���̾�α�
        if (haveEndDialog)
            UI_Manager.instance.Dialog_Fight(endDialog.dialog);

        // �� ����
        foreach (GameObject obj in door)
        {
            obj.SetActive(false);
        }

        // �� UI �ִ�ȭ
        UI_Manager.instance.MiniMap_SizeSetting(true);
    }

    public override void Field_Reset()
    {
        isClear = false;

        // üũ �ߴ�
        if (checkCoroutine != null) StopCoroutine(checkCoroutine);

        // �� ����
        foreach (GameObject door in door)
        {
            door.SetActive(false);
        }

        // ���� ����
        foreach (Enemy_Base e in enemyList)
        {
            e.Reset_Enemy();
        }
    }
}
