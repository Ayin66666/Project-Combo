using System.Collections;
using UnityEngine;


public class Hideout_Object_Save : Hideout_Object_Base
{
    [Header("---Setting---")]
    [SerializeField] private GameObject uiSet;


    public override void Use()
    {
        isUsed = true;
        
        // UI On
        SaveLoad_Manager.instance.SaveLoadUI(true);

        // ���콺 Ŀ�� ����
        Player_Manager.instance.Cursor_Setting(false);

        // ���� ���
        StartCoroutine(Waiting());
    }


    private IEnumerator Waiting()
    {
        while(SaveLoad_Manager.instance.isUIOn)
        {
            yield return null;
        }

        Out();
    }


    public override void Out()
    {
        // ���콺 Ŀ�� ����
        Player_Manager.instance.Cursor_Setting(true);

        isUsed = false;
    }
}
