using UnityEngine;


public class Camera_WayPoint_Controller : Camera_WayPoint_Base
{
    private void Awake()
    {
        WayPoint_Setting();
    }

    private void Update()
    {
        if (data.wayPoints != null && data.wayPoints.Count > 0) // 여기 에러 발생 - 아마 스테이지 내에 웨이포인트 관련 기능이 없어서 그런듯?
        {
            UpdateUI();
        }
    }

    public void WayPoint_Setting()
    {
        data.wayPoints.Clear();
        data.worldWayPoints = GameObject.Find("WorldWayPoints");
    }

    private void UpdateUI()
    {
        foreach (WayPoint_Controller wayPoint in data.wayPoints)
        {
            wayPoint.wayPoint_Base.image.transform.position = UI_Image_Position(wayPoint.wayPoint_Base);
            wayPoint.wayPoint_Base.text.text = WayPointDistance(wayPoint.wayPoint_Base) + "M"; // 여기 에러 발생 - 스테이지 넘어가면 발생하는거 봐선 저 foreach 내에 오브젝트가 없어서 그런듯?
        }    
    }
}
