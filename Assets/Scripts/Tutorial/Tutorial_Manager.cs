using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Manager : MonoBehaviour
{
    public static Tutorial_Manager instance;

    // 1. ���� ���̾�α�
    // 2. �⺻ ���� ���� & �̵� ���� - ������
    // 3. ������Ʈ ����
    // 4. �̵� ���� - ���� ��
    // 5. ���� 1�� - �Ϲ� & ���Ž� (�Ϲ� ��ȯ)
    // 6. ���� 2�� - ��ų & ����
    // 7. ���� 2�� - �ʻ��
    // 7. ���� ���� - ����Ʈ �̵�


    [Header("---Tutorial UI---")]
    [SerializeField] private GameObject[] tutorials_Tooltip;
    [SerializeField] private GameObject[] tutorial_UI;
    public bool istutorialOn;
    private bool isItem;
    private bool isSkill;


    [Header("---Tutorial Spawn---")]
    [SerializeField] private Field_Base[] spawn;


    [Header("---Tutorial Item & Skill---")]
    [SerializeField] private GameObject[] tutorial_Item_Skill;
    public System.Action tutorialAction;
    public bool isAction;


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
    }


    #region ��� ���� & ��ųƮ�� ����Ʈ
    /// <summary>
    /// �׼����� ȣ���� �̺�Ʈ 1 - ��� ���� ���� üũ
    /// </summary>
    private void Item_Setting()
    {
        isItem = true;
    }

    /// <summary>
    /// �׼����� ȣ���� �̺�Ʈ 2 - ��ų ȹ�� ���� üũ
    /// </summary>
    private void Skill_Setting()
    {
        isSkill = true;
    }

    /// <summary>
    /// ��� ���� & ��ųƮ�� ����Ʈ ���� �� ȣ��
    /// </summary>
    public void ItemSkill_TutorialOver()
    {
        Stage_Manager.instance.Dialog(4);
    }
    #endregion


    /// <summary>
    /// �� �ϴ� ���� �ؽ�Ʈ UI
    /// </summary>
    /// <param name="isOn"></param>
    /// <param name="index"></param>
    public void Tutorial_Tooltip(bool isOn, int index)
    {
        tutorials_Tooltip[index].SetActive(isOn);
    }

    /// <summary>
    /// ȭ�� ��ü UI
    /// </summary>
    /// <param name="index"></param>
    public void Tutorial_Big(int index)
    {
        Player_Manager.instance.canAction = false;
        istutorialOn = true;

        Time.timeScale = 0f;

        // Ʃ�丮�� ��ü ��Ȱ��ȭ
        foreach (GameObject obj in tutorial_UI)
        {
            obj.SetActive(false);
        }

        // Ʃ�丮�� Ȱ��ȭ
        tutorial_UI[index].SetActive(true);
    }

    /// <summary>
    /// Ʃ�丮�� ���� ��ư �Լ�
    /// </summary>
    public void TutorialOff()
    {
        Time.timeScale = 1f;

        foreach (GameObject obj in tutorial_UI)
        {
            obj.SetActive(false);
        }

        Player_Manager.instance.canAction = true;
        istutorialOn = false;
    }

    /// <summary>
    /// Ʃ�丮�� ���ʹ� ��ȯ ��� - �ʵ� ��ġ�صΰ� ���� ��� ����!
    /// </summary>
    /// <param name="index"></param>
    public void Tutorial_Spawn(int index)
    {
        spawn[index].Field_Start();
    }
}
