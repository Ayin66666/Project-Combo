using UnityEngine;


public class CutScene_Chapter1_2_Elite_Phase2Die_AnimationEvent : MonoBehaviour
{
    [SerializeField] private CutScene_Chapter1_2_Elite_Phase2_Yuria yuria;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    public void ActionOver()
    {
        anim.SetBool("isAction", false);
    }

    public void Shoot()
    {
        yuria.Shoot_Yuria();
    }

    public void Shoot_Eunha()
    {

    }

    public void Evnet()
    {
        yuria.Action_Yuria();
    }

    public void Remove()
    {
        yuria.Remove_Yuria();
    }
}
