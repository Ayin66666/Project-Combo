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
        // ����
        stageCoroutine = StartCoroutine(Field_Check());
    }

    private IEnumerator Field_Check()
    {
        // �� UI �ּ�ȭ & �� ���
        UI_Manager.instance.MiniMap_SizeSetting(false);
        Door_Setting(true);

        // ���� ���̾�α�
        if (haveStartDialog)
            UI_Manager.instance.Dialog_Fight(startDialog.dialog);

        // ��ȯ ���� - ������ �ִ� ���� ����ŭ ��ȯ
        for (int i = 0; i < enemyData.Length; i++)
        {
            // ���� ��ȯ
            GameObject enemy = Instantiate(enemyData[i].enemy, enemyData[i].spawnPos.position, Quaternion.identity);
            curEnemy = enemy.GetComponent<Enemy_Base>();
            curEnemy.Spawn();

            // ���� �ƽ� ���
            while (curEnemy.isCutScene)
            {
                yield return null;
            }

            // �������� üũ
            while (enemy != null)
            {
                // ���̾�α� üũ
                Dialog_Check(i);

                // üũ ������
                yield return delay;
            }
        }

        // ���� ó�� �Ϸ� �� ����
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

        // Ŭ���� UI
        UI_Manager.instance.FieldClearUI(UI_Manager.ClearType.Boss);

        // �� UI �ִ�ȭ
        UI_Manager.instance.MiniMap_SizeSetting(true);

        // ���� ���̾�α�
        if (haveEndDialog)
            UI_Manager.instance.Dialog_Fight(endDialog.dialog);

        // �� ����
        Door_Setting(false);
    }

    public override void Field_Reset()
    {
        isClear = false;

        // �������� üũ ����
        if (stageCoroutine != null) 
            StopCoroutine(stageCoroutine);

        // ���� �ı�
        Destroy(curEnemy.gameObject);

        // �� ����
        Door_Setting(false);
    }
}
