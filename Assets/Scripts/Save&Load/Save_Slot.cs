using TMPro;
using UnityEngine;

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
