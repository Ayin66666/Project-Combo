using System.Buffers;
using UnityEngine;

public class Player_Manager : MonoBehaviour
{
    public static Player_Manager instance;


    [Header("---Player Scripts---")]
    public PlayerAction_Manager action; // ���� �ʿ� -> �̰� �̱��� ��ȯ �� �ؾ��ϳ�?
    public Player_Status status;
    public Skill_Manager skill;
    public Inventory_Manager inventory;
    public Cooldown_Manager cooldown;
    public ShortCut_Manager shortCut;


    [Header("---Player---")]
    [SerializeField] private GameObject playerSet;
    [SerializeField] private GameObject playerMovement;
    [SerializeField] private Animator anim;

    #region ������Ƽ
    public GameObject Player
    {
        get { return playerMovement; } private set { playerMovement = value; }
    }
    #endregion


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
    public void PlayerOnOff_Setting(bool isOn)
    {
        playerSet.SetActive(isOn);
    }

    public void PlayerPos_Setting(Vector3 pos)
    {
        playerMovement.transform.position = pos;
    }

    /// <summary>
    /// ���� ���� ���� �� ȣ��
    /// </summary>
    /// <param name="isOn"></param>
    public void Player_Action_Setting(bool isOn)
    {
        action.canAction = isOn;
        action.canAttack = isOn;
        action.canDash = isOn;
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
