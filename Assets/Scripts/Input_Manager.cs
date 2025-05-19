using System;
using System.Collections;
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
        KeyDown_Check();
        KeyUp_Check();
    }

    /// <summary>
    /// ��ǲ �׼� �Լ��� ���� �Լ� ����
    /// </summary>
    /// <param name="index">0 = �Ϲ� / 1 = ���޽� / 2 = ī���� / 3 = ���� / 4 = �뽬 / 5 = �뽬 ������</param>
    /// <param name="name">�׼����� ȣ���� �Լ�</param>
    public void Action_Setting(int index, Action name)
    {
        // Ȥ�� �� �Լ� �ߺ� ��� ������
        if(inputDatas[index].inputAction != null)
        {
            Debug.Log($"�ߺ� ��� ���� {index} ��° ������ / {name} �Լ� / {inputDatas[index].inputAction} ��ϵǾ� �־���");
            inputDatas[index].inputAction = null;
        }

        inputDatas[index].inputAction += name;
    }

    private void KeyDown_Check()
    {
        movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

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
