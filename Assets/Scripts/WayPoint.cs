using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WayPoint : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Transform target;

    public void Update()
    {
        float minX = image.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = image.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.width - minY;
        Vector2 pos = Camera.main.WorldToScreenPoint(target.position);

        // Ÿ���� ī�޶�� ���ֺ����� Ȥ�� �ݴ�������� üũ
        if (Vector3.Dot((target.position - transform.position), transform.forward) < 0)
        {
            // �ڿ� ���� ���
            pos.x = pos.x < Screen.width / 2 ? maxX : minX;
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        image.transform.position = pos;
    }
}
