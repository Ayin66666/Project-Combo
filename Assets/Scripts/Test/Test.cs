using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform[] movePos;
    private int currentIndex = 0;

    private void Start()
    {
        StartCoroutine(MoveLoop());
    }

    private IEnumerator MoveLoop()
    {
        while (true)
        {
            Transform targetPos = movePos[currentIndex];
            transform.DOMove(targetPos.position, speed);

            // 다음 목표 인덱스 설정 (0 <-> 1)
            currentIndex = (currentIndex == 0) ? 1 : 0;

            yield return new WaitForSeconds(speed); // 이동 시간만큼 대기
        }
    }
}
