using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Dialog Data", menuName = "Scriptable Object/Dialog Data", order = int.MaxValue)]
public class Dialog_Data_SO : ScriptableObject
{
    [SerializeField] private List<Data> datas;
    public List<Data> Datas { get { return datas; } }

    public enum EventPos { None, Start, End }
    public enum EventType { None, Door, WayPoint, Quest, Tooltip, Tutorial, Spawn, StageEnd }


    [System.Serializable]
    public struct Data
    {
        [SerializeField] private string dialogName;

        [Header("---Event---")]
        public List<EventData> eventDatas;


        [Header("---Dialog---")]
        public float dialog_Daley;
        public string characterName;
        [TextArea] public string dialog;

    }

    [System.Serializable]
    public struct EventData
    {
        public EventType evnetType;
        public EventPos evnetPos;
        public int typeIndex;
        public bool typeOnOff;
    }

}
