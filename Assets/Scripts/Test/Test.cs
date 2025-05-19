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

            // ���� ��ǥ �ε��� ���� (0 <-> 1)
            currentIndex = (currentIndex == 0) ? 1 : 0;

            yield return new WaitForSeconds(speed); // �̵� �ð���ŭ ���
        }
    }
}
