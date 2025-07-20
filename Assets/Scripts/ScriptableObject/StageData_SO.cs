using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Stage Data", menuName = "Scriptable Object/Stage Data", order = int.MaxValue)]
public class StageData_SO : ScriptableObject
{
    [Header("---Rank---")]
    [SerializeField] private string ChapterName;
    [SerializeField] private List<RankData> data;
    public List <RankData> Data { get { return data; }  private set { data = value; } }

    [System.Serializable]
    public struct RankData
    {
        public StageData.Rank rank;
        public float time;
    }


    [Header("---Exp---")]
    [SerializeField] private int clearExp;
    public int ClearExp {  get { return clearExp; } private set {  clearExp = value; } }
}
