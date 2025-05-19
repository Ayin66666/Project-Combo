using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field_Normal : Field_Base
{
    [Header("---Setting---")]
    [SerializeField] private bool haveDialog;
    [SerializeField] private List<DialogData> countDialogData;
    private readonly WaitForSeconds checkInterval = new WaitForSeconds(1f);


    public override void Field_Start()
    {
        StartCoroutine(StartCall());
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

        // ���ʹ� ��ȯ
        enemyCount = spawnDatas[0].enemys.Count;
        for (int i = 0; i < spawnDatas[0].enemys.Count; i++)
        {
            spawnDatas[0].enemys[i].SetActive(true);
            yield return new WaitForSeconds(spawnDatas[0].spawnDelay);
        }

        // üũ ����
        StartCoroutine(CheckCall());
    }

    private IEnumerator CheckCall()
    {
        // ���� �� üũ
        enemyCount = spawnDatas[0].enemys.Count;

        // ���� ���
        while (enemyCount > 0)
        {
            // ���� üũ
            foreach (var enemy in spawnDatas[0].enemys)
            {
                if (enemy == null)
                    enemyCount--;
            }

            // ���̾�α� üũ
            if (haveDialog)
            {
                for (int i = 0; i < countDialogData.Count; i++)
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
        // Ŭ���� UI
        UI_Manager.instance.FieldClear_Normal();

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
}
