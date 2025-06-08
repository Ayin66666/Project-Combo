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
    /// ���� ������ UI ����
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


    #region ��ư �̺�Ʈ
    public void Click_Slot()
    {
        saveButton.SetActive(!SaveLoad_Manager.instance.isStartScene);

        // ����ȭ�� üũ
        if(SaveLoad_Manager.instance.isStartScene)
        {
            // ����ȭ��
            if(SaveLoad_Manager.instance.CheckData(slotIndex))
            {
                // ����ȭ�� / ������ O - ���� UI ǥ��
                buttonSet.SetActive(!buttonSet.activeSelf);
            }
            else
            {
                // ����ȭ�� / ������ X - �ű� ������ �߰� + �� �̵�
                SaveLoad_Manager.instance.Click_Create(slotIndex);
            }
        }
        else
        {
            // ����Ʈ
            if (SaveLoad_Manager.instance.CheckData(slotIndex))
            {
                // �����Ͱ� �ִٸ� - ����� UI
                buttonSet.SetActive(!buttonSet.activeSelf);
                //SaveLoad_Manager.instance.Click_Save(slotIndex);
            }
            else
            {
                // �����Ͱ� ���ٸ� - �ش� ���Կ� ����
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
