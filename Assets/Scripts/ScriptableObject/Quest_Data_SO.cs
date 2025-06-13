using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest Data", menuName = "Scriptable Object/Quest Data", order = int.MaxValue)]
public class Quest_Data_SO : ScriptableObject
{
    [SerializeField] private List<Data> datas;
    public List<Data> Datas {  get { return datas; } }

    [System.Serializable]
    public struct Data
    {
        [SerializeField] public string questTitle;
        [SerializeField][TextArea] public string questDescription;
    }
}
