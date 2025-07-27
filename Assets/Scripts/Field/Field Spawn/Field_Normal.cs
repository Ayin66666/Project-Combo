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
        // �� ����
        if(door != null)
        {
            foreach (GameObject obj in door)
            {
                obj.SetActive(true);
            }
        }

        // ���� ���̾�α� üũ
        if (haveStartDialog)
            UI_Manager.instance.Dialog_Fight(startDialog.dialog);

        // �� UI �ּ�ȭ
        UI_Manager.instance.MiniMap_SizeSetting(false);

        // ���ʹ� ��ȯ
        enemyList = new List<Enemy_Base>();
        enemyCount = spawnDatas[0].enemys.Count;
        for (int i = 0; i < spawnDatas[0].enemys.Count; i++)
        {
            GameObject obj = Stage_Manager.instance.enemy_Container.Spawn_Enemy(spawnDatas[0].enemys[i].enemy);
            enemyList.Add(obj.GetComponent<Enemy_Base>());

            obj.transform.position = spawnDatas[0].enemys[i].spawnPos.position;
            obj.transform.rotation = spawnDatas[0].enemys[i].spawnPos.rotation;
            obj.SetActive(true);

            // ���� ������
            yield return new WaitForSeconds(spawnDatas[0].spawnDelay);
        }

        // üũ ����
        StartCoroutine(CheckCall());
    }

    private IEnumerator CheckCall()
    {
        // ���� �� üũ
        enemyCount = enemyList.Count;

        // ���� ���
        while (enemyCount > 0)
        {
            // ���� üũ
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList[i].curState == Enemy_Base.State.Die || !enemyList[i].gameObject.activeSelf)
                    enemyList.RemoveAt(i);
            }
            enemyCount = enemyList.Count;

            // ���̾�α� üũ
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

            // üũ ������
            yield return checkInterval;
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

        // �� UI �ִ�ȭ
        UI_Manager.instance.MiniMap_SizeSetting(true);

        // �� ����
        foreach (GameObject obj in door)
        {
            obj.SetActive(false);
        }
    }

    public override void Field_Reset()
    {
        isClear = false;

        // üũ �ߴ�
        if (checkCoroutine != null) StopCoroutine(checkCoroutine);

        // �� ����
        foreach(GameObject door in door)
        {
            door.SetActive(false);
        }

        // ���� ����
        foreach(Enemy_Base e in enemyList)
        {
            e.Reset_Enemy();
        }
    }
}
