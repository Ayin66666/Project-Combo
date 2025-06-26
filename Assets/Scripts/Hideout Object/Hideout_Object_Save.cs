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

        // 마우스 커서 설정
        Player_Manager.instance.Cursor_Setting(false);

        // 종료 대기
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
        // 마우스 커서 설정
        Player_Manager.instance.Cursor_Setting(true);

        isUsed = false;
    }
}
