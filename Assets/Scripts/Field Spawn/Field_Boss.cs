using System.Collections;
using UnityEngine;


public class Field_Boss : Field_Base
{
    [Header("---Setting---")]
    [SerializeField] private Enemy_Base boss;
    private  WaitForSeconds delay = new WaitForSeconds(1f);


    // ����Ʈ ���� �� ��ȯ Ʈ�� �߰��� ��?
    // �Ϻη� �ڵ嵵 ���̽� ��� �������� �����µ� ��ĥ �ʿ䵵 ����
    // ������ ����Ʈ �����ڵ� �����ϰ� ��ȯ�ڵ� ���⿡ �ϴ� �͵�?
    // üũ ����� ��Ʈ�ѷ��� ��� �����ϸ� ���� ������ / ��� - �׳� �ϳ��� ���
    

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

        // ���̾�α�
        if(haveStartDialog)
        {

        }

        // �������� üũ
        while (boss == null)
        {
            yield return delay;
        }

        // �������� ����
        Field_End();
    }

    public override void Field_End()
    {
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
}
