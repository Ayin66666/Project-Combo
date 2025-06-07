using System.Buffers;
using UnityEngine;

public class Player_Manager : MonoBehaviour
{
    public static Player_Manager instance;


    [Header("---Player Scripts---")]
    public PlayerAction_Manager action; // ���� �ʿ� -> �̰� �̱��� ��ȯ �� �ؾ��ϳ�?
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
    /// �÷��̾� On/Off
    /// </summary>
    /// <param name="isOn"></param>
    public void Player_Setting(bool isOn, Vector3 pos)
    {
        player.transform.position = pos;
        player.SetActive(isOn);
    }

    /// <summary>
    /// ���� ���� ���� �� ȣ��
    /// </summary>
    /// <param name="isOn"></param>
    public void Player_Action_Setting(bool isOn)
    {
        action.canAction = isOn;
        action.canAttack = isOn;
        action.canMovement = isOn;
    }

    /// <summary>
    /// ����Ʈ ���� �� ȣ��
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
