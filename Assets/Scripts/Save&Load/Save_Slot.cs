using System;
using TMPro;
using UnityEngine;

public class Save_Slot : MonoBehaviour
{
    [Header("---UI---")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI timeText;


    public void Slot_Setting(string level, float time)
    {
        TimeSpan playtime = TimeSpan.FromSeconds(time);
        timeText.text = $"{playtime.Hours} : {playtime.Minutes} : {playtime.Seconds}";
        levelText.text = level;
    }
}
