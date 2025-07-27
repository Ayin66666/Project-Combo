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

        // 다이얼로그 호출
        if (haveDialog)
            UI_Manager.instance.Dialog_Fight(dialogData);

        // 마커 호출
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
