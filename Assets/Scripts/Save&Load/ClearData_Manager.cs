using UnityEngine;


public class ClearData_Manager : MonoBehaviour
{
    public static ClearData_Manager instance;


    [Header("---Setting---")]
    public bool haveNewData;
    [SerializeField] private int chapterCount;
    [SerializeField] private int stageCount;
    [SerializeField] private StageData data;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    /// <summary>
    /// 스테이지 클리어 시 데이터 전달
    /// </summary>
    /// <param name="chapter"></param>
    /// <param name="stage"></param>
    /// <param name="data"></param>
    public void Set_StageData(int chapter, int stage, StageData data)
    {
        haveNewData = true;
        chapterCount = chapter;
        stageCount = stage;
        this.data = data;
    }

    /// <summary>
    /// 데이터 최신화
    /// </summary>
    /// <returns></returns>
    public (int chapter, int stageCount, StageData data) Get_StageData()
    {
        return (chapterCount, stageCount, data);
    }
}
