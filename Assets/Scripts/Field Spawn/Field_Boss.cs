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


    public override void Field_Start()
    {
        StartCoroutine(StartCall());
    }

    private IEnumerator StartCall()
    {
        // ���� ��ȯ
        boss.gameObject.SetActive(true);

        // �� UI �ּ�ȭ
        UI_Manager.instance.MiniMap_SizeSetting(false);

        // �������� üũ
        while (true)
        {
            // ���� üũ
            if (boss == null)
                break;

            yield return delay;
        }

        // �������� ����
        Field_End();
    }

    public override void Field_End()
    {
        // Ŭ���� UI
        UI_Manager.instance.FieldClear_Normal();

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
