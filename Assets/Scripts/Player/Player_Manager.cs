using System.Buffers;
using UnityEngine;

public class Player_Manager : MonoBehaviour
{
    public static Player_Manager instance;


    [Header("---Player Scripts---")]
    public PlayerAction_Manager action; // 연결 필요 -> 이거 싱글톤 전환 꼭 해야하나?
    public Player_Status status;


    [Header("---Player---")]
    [SerializeField] private GameObject player;
    [SerializeField] private Animator anim;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // DontDestroyOnLoad(gameObject);
    }


    /// <summary>
    /// 플레이어 On/Off
    /// </summary>
    /// <param name="isOn"></param>
    public void Player_Setting(bool isOn, Vector3 pos)
    {
        player.transform.position = pos;
        player.SetActive(isOn);
    }

    /// <summary>
    /// 전투 지역 진입 시 호출
    /// </summary>
    /// <param name="isOn"></param>
    public void Player_Action_Setting(bool isOn)
    {
        action.canAction = isOn;
        action.canAttack = isOn;
        action.canMovement = isOn;
    }

    /// <summary>
    /// 아지트 진입 시 호출
    /// </summary>
    public void Player_Hideout_Setting()
    {
        action.canAction = true;
        action.canMovement = true;
        action.canAttack = false;
    }

    public void Cursor_Setting(bool isOn)
    {
        Cursor.lockState = isOn ? CursorLockMode.Locked : CursorLockMode.None;
        action.canAction = isOn;

        if (!isOn)
        {
            anim.SetBool("isMove", false);
            anim.SetBool("isStop", true);
        }
    }
}
