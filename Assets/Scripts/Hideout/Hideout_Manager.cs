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
    /// 스테이지 선택 UI
    /// </summary>
    /// <param name="isOn"></param>
    public void Stage_Select(bool isOn)
    {
        // 1. 플레이어의 진행도 데이터를 저장하는 무언가에서 데이터 받아오기

        // 2. 받아온 데이터 기반 UI 활성화
        selectSet.SetActive(isOn);
    }

    /// <summary>
    /// 스테이지 이동 기능
    /// </summary>
    public void Stage_Move(string sceneName)
    {
        // 선행 연출 - 넣을건지?

        // 씬 이동
        SceneLoad_Manager.LoadScene(sceneName);
    }



    /// <summary>
    /// 게임 데이터 세이브 기능
    /// </summary>
    public void Save()
    {
        
    }
}
