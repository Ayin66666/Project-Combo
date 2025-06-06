using UnityEngine;
using TMPro;


public class Save_Slot : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private int slotIndex;


    [Header("---Slot UI---")]
    [SerializeField] private TextMeshProUGUI chapterText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI timeText;


    [Header("---Button UI---")]
    [SerializeField] private GameObject buttonSet;
    [SerializeField] private GameObject saveButton;


    /// <summary>
    /// 슬롯 데이터 UI 셋팅
    /// </summary>
    /// <param name="chapter"></param>
    /// <param name="level"></param>
    /// <param name="time"></param>
    public void Slot_Setting(string chapter, string level, float time)
    {
        chapterText.text = $"- {chapter} -";
        System.TimeSpan playtime = System.TimeSpan.FromSeconds(time);
        timeText.text = $"{playtime.Hours} : {playtime.Minutes} : {playtime.Seconds}";
        levelText.text = $"Lv.{level}";
    }


    #region 버튼 이벤트
    public void Click_Slot()
    {
        saveButton.SetActive(!SaveLoad_Manager.instance.isStartScene);

        // 데이터 체크
        if (SaveLoad_Manager.instance.CheckData(slotIndex))
        {
            // 데이터가 있다면 선택 UI 표기
            buttonSet.SetActive(!buttonSet.activeSelf);
        }
        else
        {
            // 데이터가 없다면 신규 데이터 추가 + 씬 이동
            SaveLoad_Manager.instance.Click_Create(slotIndex);
        }
    }


    public void Click_Save()
    {
        Debug.Log("isClick - Save");
        SaveLoad_Manager.instance.Click_Save(slotIndex);
    }

    public void Click_Load()
    {
        Debug.Log("isClick - Load");
        SaveLoad_Manager.instance.Click_Load(slotIndex);
    }

    public void Click_Remove()
    {
        Debug.Log("isClick - Remove");
        SaveLoad_Manager.instance.Click_Remvoe(slotIndex);
    }
    #endregion
}
