using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_EvnetPoint : MonoBehaviour
{
    private bool isUsed;

    [Header("---Dialog---")]
    [SerializeField] private bool haveDialog;
    [SerializeField] private Dialog_Data_SO dialogData;

    [Header("---Mark---")]
    [SerializeField] private bool haveMark;
    [SerializeField] private int markIndex;


    public void Use()
    {
        isUsed = true;

        // ���̾�α� ȣ��
        if (haveDialog)
            UI_Manager.instance.Dialog_Fight(dialogData);

        // ��Ŀ ȣ��
        if (haveMark)
        {

        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isUsed)
        {
            Use();
        }
    }
}
