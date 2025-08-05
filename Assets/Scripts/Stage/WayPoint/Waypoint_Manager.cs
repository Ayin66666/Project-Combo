using System.Collections.Generic;
using UnityEngine;

public class Waypoint_Manager : MonoBehaviour
{
    [Header("---Setting---")]
    public Camera_WayPoint_Base camera_WayPoint_Base;
    [SerializeField] Data data;

    [System.Serializable]
    struct Data
    {
        public int totalWayPoints;
        public GameObject waypointCanvas;
        public GameObject worldWaypoints;

        public List<WayPoint_Controller> wayPoint_Controller;
    }
    

    private void Start()
    {
        camera_WayPoint_Base = PlayerAction_Manager.instance.cam.GetComponent<Camera_WayPoint_Base>();
    }

    /// <summary>
    /// ������ �ε����� ��������Ʈ�� Ȱ��ȭ�ϰ�, ������ ��������Ʈ�� ��Ȱ��ȭ �ϴ� ���
    /// </summary>
    /// <param name="isOn">Ȱ��ȭ ����</param>
    /// <param name="index">Ȱ��ȭ �� ��������Ʈ�� �ε���</param>
    public void Waypoint_Setting(bool isOn, int index)
    {
        if(isOn)
        {
            camera_WayPoint_Base.data.wayPoints.Add(data.wayPoint_Controller[index]);

            for (int i = 0; i < data.wayPoint_Controller.Count; i++)
            {
                data.wayPoint_Controller[i].gameObject.SetActive(false);
                data.wayPoint_Controller[i].wayPoint_UI.gameObject.SetActive(false);

                data.wayPoint_Controller[i].wayPoint_UI.gameObject.transform.SetParent(data.wayPoint_Controller[i].gameObject.transform);
            }

            data.wayPoint_Controller[index].gameObject.SetActive(true);
            data.wayPoint_Controller[index].wayPoint_UI.gameObject.SetActive(true);
        }
        else
        {
            for (int i = 0; i < data.wayPoint_Controller.Count; i++)
            {
                data.wayPoint_Controller[i].gameObject.SetActive(false);
                data.wayPoint_Controller[i].wayPoint_UI.gameObject.SetActive(false);

                data.wayPoint_Controller[i].wayPoint_UI.gameObject.transform.SetParent(data.wayPoint_Controller[i].gameObject.transform);
            }
        }

    }
}
