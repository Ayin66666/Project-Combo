using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class InputData
{
    public string keyName;
    public KeyCode key;
    public bool isInput;

    public Action inputAction;
}


public class Input_Manager : MonoBehaviour
{
    public static Input_Manager instance;

    [Header("--- Input Setting ---")]
    public List<InputData> inputDatas;
    public List<InputData> shortcutInputDatas;
    public Vector3 movementInput;

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



    private void Update()
    {
        Shortcut_Check();
        KeyDown_Check();
        KeyUp_Check();
    }

    /// <summary>
    /// 인풋 액션 함수에 넣을 함수 전달
    /// </summary>
    /// <param name="index">0 = 일반 / 1 = 스메쉬 / 2 = 카운터 / 3 = 각성 / 4 = 대쉬 / 5 = 대쉬 슬래쉬</param>
    /// <param name="name">액션으로 호출할 함수</param>
    public void Action_Setting(int index, Action name)
    {
        // 혹시 모를 함수 중복 등록 방지용
        if(inputDatas[index].inputAction != null)
        {
            Debug.Log($"중복 등록 감지 {index} 번째 데이터 / {name} 함수 / {inputDatas[index].inputAction} 등록되어 있었음");
            inputDatas[index].inputAction = null;
        }

        inputDatas[index].inputAction += name;
    }

    /// <summary>
    /// 쇼트컷 용 인풋 액션 함수에 넣을 함수 전달
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="actonIndex"></param>
    public void ShortCut_Setting(int index, Action name)
    {
        shortcutInputDatas[index].inputAction += name;
    }

    private void Shortcut_Check()
    {
        foreach (InputData data in shortcutInputDatas)
        {
            if (Input.GetKeyDown(data.key))
            {
                data.isInput = true;
                data.inputAction?.Invoke();
            }
        }
    }

    private void KeyDown_Check()
    {
        movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        // 전투 입력
        foreach (InputData data in inputDatas)
        {
            if (Input.GetKeyDown(data.key))
            {
                data.isInput = true;
                data.inputAction?.Invoke();
            }
        }
    }

    private void KeyUp_Check()
    {
        foreach (InputData data in inputDatas)
        {
            if (Input.GetKeyUp(data.key))
            {
                data.isInput = false;
            }
        }
    }
}
