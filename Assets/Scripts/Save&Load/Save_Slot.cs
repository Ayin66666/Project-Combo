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

    public void ButtonOnOff(bool isOn)
    {
        buttonSet.SetActive(isOn);
    }


    #region 버튼 이벤트
    public void Click_Slot()
    {
        saveButton.SetActive(!SaveLoad_Manager.instance.isStartScene);

        // 시작화면 체크
        if(SaveLoad_Manager.instance.isStartScene)
        {
            // 시작화면
            if(SaveLoad_Manager.instance.CheckData(slotIndex))
            {
                // 시작화면 / 데이터 O - 선택 UI 표기
                buttonSet.SetActive(!buttonSet.activeSelf);
            }
            else
            {
                // 시작화면 / 데이터 X - 신규 데이터 추가 + 씬 이동
                SaveLoad_Manager.instance.Click_Create(slotIndex);
            }
        }
        else
        {
            // 아지트
            if (SaveLoad_Manager.instance.CheckData(slotIndex))
            {
                // 데이터가 있다면 - 덮어쓰기 UI
                buttonSet.SetActive(!buttonSet.activeSelf);
                //SaveLoad_Manager.instance.Click_Save(slotIndex);
            }
            else
            {
                // 데이터가 없다면 - 해당 슬롯에 저장
                SaveLoad_Manager.instance.SaveResultUI(SaveLoad_Manager.instance.Save(slotIndex));
                SaveLoad_Manager.instance.SlotUI_Setting();
            }
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
