using System.Collections.Generic;
using UnityEngine;
using System.IO; // ��ǲ �ƿ�ǲ
using System.Collections; 


public class SaveLoad_Manager : MonoBehaviour
{
    public static SaveLoad_Manager instance;

    [Header("---Save Setting---")]
    public string savePath;
    public List<Path> fileName;
    public int curSlot;

    [System.Serializable]
    public struct Path
    {
        public string filename_PlayerData;
        public string filename_stageData;
    }


    [Header("---UI---")]
    public bool isCover;
    public GameObject saveUIset;
    public GameObject coverUISet;
    [SerializeField] private GameObject saveSuccessSet;
    [SerializeField] private CanvasGroup saveSuccessCanvas;
    private Coroutine saveSuccessCoroutine;


    [Header("---Component---")]
    public PlayerData_Manager playerData_Manager;
    public Chapter_Manager chapter_Manager;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        savePath = Application.persistentDataPath + "/";
    }


    #region UI
    public void SaveUI(bool isOn)
    {
        saveUIset.SetActive(isOn);
    }

    public void CoverUI(bool isOn)
    {
        coverUISet.SetActive(isOn);
    }

    public void SaveSuccessUI()
    {
        if(saveSuccessCoroutine != null)
            StopCoroutine(saveSuccessCoroutine);

        saveSuccessCoroutine = StartCoroutine(SaveSuccessCall());
    }

    private IEnumerator SaveSuccessCall()
    {
        saveSuccessCanvas.alpha = 1f;
        saveSuccessSet.SetActive(true);

        // ������
        yield return new WaitForSeconds(0.25f);

        // ���̵� �ƿ�
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            saveSuccessCanvas.alpha = Mathf.Lerp(1, 0, timer);
            yield return null;
        }

        saveSuccessCanvas.alpha = 0;
        saveSuccessSet.SetActive(false);
    }
    #endregion


    #region Save & Load
    /// <summary>
    /// �ش� ���Կ� �����Ͱ� �ִ��� üũ - �÷��̾� & �������� �� �� �־����!
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool CheckData(int index)
    {
        if (playerData_Manager.CheckData(index) && chapter_Manager.CheckData(index))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// ������ ���̺� ���
    /// </summary>
    /// <param name="index"></param>
    public void SaveData(int index)
    {
        // �̹� ����� �����Ͱ� ���� ��� ���
        if (CheckData(index))
        {
            // ����� �ȳ� UI

        }
        else
        {
            // �÷��̾� ������ ����
            playerData_Manager.SaveData(index);

            // �� ������ ����
            chapter_Manager.SaveData(index);

            // ���̺� ���� UI
            SaveSuccessUI();
        }
    }

    /// <summary>
    /// ������ �ε� ���
    /// </summary>
    /// <param name="index"></param>
    public void LoadData(int index)
    {
        if (!CheckData(index))
        {
            // �ε� ���� ����!
            Debug.LogWarning("�ε��� ������ �����ϴ�: ");
            return;
        }

        try
        {
            // �������ͽ� �ε�
            playerData_Manager.LoadData(index);

            // �� ������ �ε�
            chapter_Manager.LoadData(index);

        }
        catch (IOException ex)
        {
            // �ε带 ������ ���
            Debug.LogError("Load failed: " + ex.Message);
        }
    }
    #endregion
}
