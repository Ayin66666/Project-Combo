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
     * 여기 헤더들 각 소환 시스템에 따라 스크립트별로 나눠야 함!
     * 퍼즐은 일단 적어두긴 했는데 만들지는 몰?루
     * 
     * 
     * 기능 정의
     * 1. 스폰 - 단발성 소환 
     * 2. 러쉬 - 웨이브 형태로 여러번 소환
     * 3. 가드 - 오브젝트 보호 + 웨이브 형태로 여러번 소환
     * 4. 퍼즐 - 퍼즐 클리어시 문 개방
     * 5. 보스 - 보스 소환
     * 
     * 
     * 필드 동작 간 예상되는 기능
     * 1. 다이얼로그 호출
     * 2. 컷씬 호출
     * 3. 사운드 변경
     * 4. 에리어 효과 (포스트프로세싱)
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
    /// 몬스터 스폰 시작
    /// </summary>
    public abstract void Field_Start();

    /// <summary>
    /// 필드 종료
    /// </summary>
    public abstract void Field_End();

    /// <summary>
    /// 플레이어 사망 후 체크포인트 복귀 시 리셋 기능
    /// </summary>
    public abstract void Field_Reset();


    /// <summary>
    /// 스테이지 시작 시 사운드 변경 or 종료 기능
    /// </summary>
    public void Field_BGM()
    {
        // 사운드
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
    /// 문 On / Off
    /// </summary>
    /// <param name="isOn"></param>
    public void Door_Setting(bool isOn)
    {
        // 문 개방
        foreach (GameObject obj in door)
        {
            obj.SetActive(isOn);
        }
    }
}
