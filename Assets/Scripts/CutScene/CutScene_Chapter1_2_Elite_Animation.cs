using UnityEngine;


public class CutScene_Chapter1_2_Elite_Animation : MonoBehaviour
{
    [SerializeField] private CutScene_Chapter1_2_Elite_Spawn elite;
    [SerializeField] private CutScene_Chapter1_2_Elite_Die elite_Die;
    [SerializeField] private Animator anim_Yuria;
    private Animator anim_Mech;


    private void Awake()
    {
        anim_Mech = GetComponent<Animator>();
    }

    /// <summary>
    /// Spawn
    /// </summary>
    public void Dust()
    {
        elite.effects[2].SetActive(true);
    }

    /// <summary>
    /// Die
    /// </summary>
    public void StepEffect(int index)
    {
        elite_Die.Effect(index);
    }

    public void CamMove()
    {
        elite_Die.CamMove();
    }

    /// <summary>
    /// Die
    /// </summary>
    public void Cam(int index)
    {
        elite_Die.Cam(index);
    }
    /// <summary>
    /// Die
    /// </summary>
    public void Explosion()
    {
        elite_Die.CamExMove();
        elite_Die.explosion_Big.SetActive(true);
    }

    public void ActionOver()
    {
        anim_Mech.SetBool("isAction", false);
    }

    public void ActionOverYuria()
    {
        anim_Yuria.SetBool("isAction", false);
    }
}
