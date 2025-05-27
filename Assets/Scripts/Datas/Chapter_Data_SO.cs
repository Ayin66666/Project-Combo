using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Chapter Data", menuName = "Scriptable Object/Chapter Data", order = int.MaxValue)]
public class Chapter_Data_SO : ScriptableObject
{
    [Header("---C---")]
    public string chapterName;
    public StageType stageType;
    public List<Stage> stageData;
    public enum StageType { Normal, Boss }


    [System.Serializable]
    public struct Stage
    {
        public string stageName;
        public Sprite stageImage;
        [TextArea] public string stageDescription;
    }
}
