using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hideout_Manager : MonoBehaviour
{
    public static Hideout_Manager instance;

    [Header("---Select UI---")]
    [SerializeField] private GameObject selectSet;


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


    /// <summary>
    /// �������� ���� UI
    /// </summary>
    /// <param name="isOn"></param>
    public void Stage_Select(bool isOn)
    {
        // 1. �÷��̾��� ���൵ �����͸� �����ϴ� ���𰡿��� ������ �޾ƿ���

        // 2. �޾ƿ� ������ ��� UI Ȱ��ȭ
        selectSet.SetActive(isOn);
    }

    /// <summary>
    /// �������� �̵� ���
    /// </summary>
    public void Stage_Move(string sceneName)
    {
        // ���� ���� - ��������?

        // �� �̵�
        SceneLoad_Manager.LoadScene(sceneName);
    }



    /// <summary>
    /// ���� ������ ���̺� ���
    /// </summary>
    public void Save()
    {
        
    }
}
