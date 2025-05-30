using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Save_Slot : MonoBehaviour
{
    [Header("---UI---")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI timeText;


    public void Slot_Setting(string level, string time)
    {
        levelText.text = level;
        timeText.text = time;
    }
}
