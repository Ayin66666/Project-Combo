using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooldown_Manager : MonoBehaviour
{
    [Header("---Setting---")]
    private Dictionary<string, Coroutine> coroutine_Dictionary = new();


    public void Cooldown(string key, IEnumerator coroutine)
    {
        coroutine_Dictionary.Add(key, StartCoroutine(coroutine));
    }

    public void Remove(string key)
    {
        coroutine_Dictionary.Remove(key);
    }
}
