using System.Collections;
using UnityEngine;


public class Hideout_Object_Save : Hideout_Object_Base
{
    [Header("---Setting---")]
    [SerializeField] private GameObject uiSet;


    public override void Use()
    {
        isUsed = true;
        SaveLoad_Manager.instance.SaveLoadUI(true);
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
        isUsed = false;
    }
}
