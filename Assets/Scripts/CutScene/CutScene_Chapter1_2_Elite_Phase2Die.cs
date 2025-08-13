using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutScene_Chapter1_2_Elite_Phase2Die : MonoBehaviour
{
    [Header("---Setting---")]
    [SerializeField] private float backstep_Movespeed;


    [Header("---Component---")]
    [SerializeField] private GameObject body;
    [SerializeField] private GameObject[] effects;
    [SerializeField] private Transform[] movePos;
    [SerializeField] private GameObject[] cams;
    private Animator anim;
    private Coroutine coroutine;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(Action());
        }
    }

    private IEnumerator Action()
    {
        // 백스텝
        anim.SetTrigger("Action");
        anim.SetBool("isAction",true);

        // 사격

        // 안개
        yield return null;
    }
}