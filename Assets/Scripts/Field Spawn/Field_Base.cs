using System.Collections.Generic;
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


public abstract class Field_Base : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] protected FieldType fieldType;
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
        public List<GameObject> enemys;
        public float spawnDelay;
    }


    [Header("---Dialog---")]
    [SerializeField] protected bool haveStartDialog;
    [SerializeField] protected DialogData startDialog;

    [SerializeField] protected bool haveEndDialog;
    [SerializeField] protected DialogData endDialog;


    /// <summary>
    /// 몬스터 스폰 시작
    /// </summary>
    public abstract void Field_Start();

    /// <summary>
    /// 필드 종료
    /// </summary>
    public abstract void Field_End();
}
