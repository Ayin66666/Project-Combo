using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class Field_Boss : Field_Base
{
    [Header("---Setting---")]
    [SerializeField] private Enemy_Base boss;
    private  WaitForSeconds delay = new WaitForSeconds(1f);


    [Header("---Dialog---")]
    [SerializeField] private List<Dialog> dialogs;

    [System.Serializable]
    public class Dialog
    {
        [SerializeField] private string dialogName;
        public bool isUsed;
        public int hp;
        public Dialog_Data_SO dialog;
    }


    public override void Field_Start()
    {
        StartCoroutine(StartCall());
    }

    private IEnumerator StartCall()
    {
        // �� UI �ּ�ȭ
        UI_Manager.instance.MiniMap_SizeSetting(false);

        // ���� ��ȯ
        boss.gameObject.SetActive(true);

        // �ƽ� ��� -> ������� ���� �ʿ� (�ƽ� ��� / ü�¿� ���� ���̾�α� ǥ�� / ������ 2 ��ȯ �� ��� ���)
        while(boss.isCutScene)
        {
            yield return null;
        }

        // ����
        Field_BGM();

        // ���̾�α�
        if (haveStartDialog)
            UI_Manager.instance.Dialog_Fight(startDialog.dialog);


        // �������� üũ
        while (boss == null)
        {
            Dialog_Check();
            yield return delay;
        }

        // �������� ����
        Field_End();
    }

    private void Dialog_Check()
    {
        for (int i = 0; i < dialogs.Count; i++)
        {
            if (boss.curHp <= dialogs[i].hp && !dialogs[i].isUsed)
            {
                UI_Manager.instance.Dialog_Fight(dialogs[i].dialog);
                dialogs[i].isUsed = true;
            }
        }
    }

    public override void Field_End()
    {
        isClear = true;

        // Ŭ���� UI
        UI_Manager.instance.FieldClearUI(UI_Manager.ClearType.Boss);

        // �� UI �ִ�ȭ
        UI_Manager.instance.MiniMap_SizeSetting(true);

        // ���� ���̾�α�
        if (haveEndDialog)
            UI_Manager.instance.Dialog_Fight(endDialog.dialog);

        // �� ����
        foreach (GameObject obj in door)
        {
            obj.SetActive(false);
        }
    }

    public override void Field_Reset()
    {

    }
}
