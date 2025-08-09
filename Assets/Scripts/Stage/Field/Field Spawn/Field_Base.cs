using System.Collections.Generic;
using System.Collections;
using UnityEngine;


[System.Serializable]
public class DialogData
{
    [SerializeField] private string dialogName;
    public Field_Base.UseType useType;
    public int useCount;
    public bool isUsed;
    public Dialog_Data_SO dialog;
}

[System.Serializable]
public class EnemySpawnData
{
    public Enemy_Container.Enemy enemy;
    [SerializeField] public Transform spawnPos;
}


public abstract class Field_Base : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] protected FieldType fieldType;
    public bool isClear;
    [SerializeField] protected GameObject[] door;
    protected enum FieldType { Normal, Rush, Guard, Puzzle, Boss }
    public enum UseType { Stage_Start, Stage_End, EnemyCount, Hp }


    /*
     * ���� ����� �� ��ȯ �ý��ۿ� ���� ��ũ��Ʈ���� ������ ��!
     * ������ �ϴ� ����α� �ߴµ� �������� ��?��
     * 
     * 
     * ��� ����
     * 1. ���� - �ܹ߼� ��ȯ 
     * 2. ���� - ���̺� ���·� ������ ��ȯ
     * 3. ���� - ������Ʈ ��ȣ + ���̺� ���·� ������ ��ȯ
     * 4. ���� - ���� Ŭ����� �� ����
     * 5. ���� - ���� ��ȯ
     * 
     * 
     * �ʵ� ���� �� ����Ǵ� ���
     * 1. ���̾�α� ȣ��
     * 2. �ƾ� ȣ��
     * 3. ���� ����
     * 4. ������ ȿ�� (����Ʈ���μ���)
    */

    [Header("---Spawn---")]
    [SerializeField] protected List<SpawnData> spawnDatas;
    [SerializeField] protected int enemyCount;
    [SerializeField] protected int roundCount;

    [System.Serializable]
    public struct SpawnData
    {
        [SerializeField] private string dataName;
        public List<EnemySpawnData> enemys;
        public float spawnDelay;
    }


    [Header("---Dialog---")]
    [SerializeField] protected bool haveStartDialog;
    [SerializeField] protected DialogData startDialog;

    [SerializeField] protected bool haveEndDialog;
    [SerializeField] protected DialogData endDialog;


    [Header("---Sound---")]
    [SerializeField] protected bool haveSoundEffect;
    [SerializeField] protected SoundEffect soundEffect;
    [SerializeField] protected int soundIndex;
    protected enum SoundEffect { Chanage, Off }


    /// <summary>
    /// ���� ���� ����
    /// </summary>
    public abstract void Field_Start();

    /// <summary>
    /// �ʵ� ����
    /// </summary>
    public abstract void Field_End();

    /// <summary>
    /// �÷��̾� ��� �� üũ����Ʈ ���� �� ���� ���
    /// </summary>
    public abstract void Field_Reset();


    /// <summary>
    /// �������� ���� �� ���� ���� or ���� ���
    /// </summary>
    public void Field_BGM()
    {
        // ����
        if (haveSoundEffect)
        {
            switch (soundEffect)
            {
                case SoundEffect.Chanage:
                    Stage_Manager.instance.BGM(true, soundIndex);
                    break;

                case SoundEffect.Off:
                    Stage_Manager.instance.BGM(false, -1);
                    break;
            }
        }
    }

    /// <summary>
    /// �� On / Off
    /// </summary>
    /// <param name="isOn"></param>
    public void Door_Setting(bool isOn)
    {
        // �� ����
        foreach (GameObject obj in door)
        {
            obj.SetActive(isOn);
        }
    }
}
