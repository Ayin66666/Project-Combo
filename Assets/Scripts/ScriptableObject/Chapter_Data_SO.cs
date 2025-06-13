using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Chapter Data", menuName = "Scriptable Object/Chapter Data", order = int.MaxValue)]
public class Chapter_Data_SO : ScriptableObject
{
    [Header("---Chapter Data---")]
    public string chapterName;
    public string sceneName;
    public List<Stage> stageData;
    public enum StageType { Normal, Boss }


    [System.Serializable]
    public struct Stage
    {
        public StageType stageType;
        public Sprite stageImage;
        public string stageName;
        public string sceneName;
        public int stageLevel;
        public int stageClearExp;
        [TextArea] public string stageSummation;
        [TextArea] public string stageDescription;
    }
}
