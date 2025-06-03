using UnityEngine;
using System;
using TMPro;


public class Save_Slot : MonoBehaviour
{
    [Header("---UI---")]
    [SerializeField] private TextMeshProUGUI chapterText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI timeText;


    public void Slot_Setting(string chapter, string level, float time)
    {
        chapterText.text = $"- {chapter} -";
        TimeSpan playtime = TimeSpan.FromSeconds(time);
        timeText.text = $"{playtime.Hours} : {playtime.Minutes} : {playtime.Seconds}";
        levelText.text = $"Lv.{level}";
    }
}
