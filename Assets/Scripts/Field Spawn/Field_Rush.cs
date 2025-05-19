using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field_Rush : Field_Base
{
    [Header("---Setting---")]
    [SerializeField] private List<DialogData> datas;
    [SerializeField] private float nextRoundDelay;
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

        // ���� ��ȯ - ����
        for (int i = 0; i < datas.Count; i++)
        {
            // ���� ��ȯ - ����
            enemyCount = spawnDatas[i].enemys.Count;
            for (int j = 0; j < spawnDatas[i].enemys.Count; j++)
            {
                spawnDatas[i].enemys[j].SetActive(true);
            }

            // ���� �� üũ
            int aliveCount = 0;
            foreach (var enemy in spawnDatas[i].enemys)
            {
                if (enemy != null)
                    aliveCount++;
            }

            // ���� ���� ���
            while (true)
            {
                // ���� üũ
                if (aliveCount == 0)
                    break;

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
        // Ŭ���� UI
        UI_Manager.instance.FieldClear_Normal();

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
}
