using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


public class Enemy_Elite_Phase1 : Enemy_Base
{
    [Header("---Component---")]
    [SerializeField] private Enemy_Elite_Controller phaseController;
    [SerializeField] private VideoClip[] clips;
    private VideoPlayer video;

    public enum SoundKey 
    { 
        GroundStrike1_1, GroundStrike1_2, 
        FlameCharge, Flame, 
        MachineGunCharge, MachineGun,
        MisslieCharge, MisslieShoot
    }


    [Header("---Dead Setting---")]
    [SerializeField] private List<DeadExplosion> explosions;
    [System.Serializable]
    public struct DeadExplosion
    {
        public GameObject explosion;
        public float delayTime;
    }


    private void Start()
    {
        video = GetComponent<VideoPlayer>();
        Spawn();
    }

    private void Update()
    {
        if (curState == State.Die)
        {
            return;
        }

        if (curState == State.Idle)
        {
            Think();
        }
    }


    protected override void Think()
    {
        curState = State.Think;

        Check_Target();
        if (targetRange <= 5)
        {
            int ran = Random.Range(0, attackDatas.Count);
            attackDatas[0].Use();
        }

        if (targetRange > 5)
        {
            int ran = Random.Range(1, attackDatas.Count);
            attackDatas[ran].Use();
        }
    }

    protected override IEnumerator DelayMovement()
    {
        curState = State.Delay;

        // 딜레이 이동 - 걷기
        float ranDelay = Random.Range(1.3f, 1.6f);
        int ran = Random.Range(0, 1);
        Vector3 moveDir = ((ran == 0 ? transform.right : -transform.right) + -transform.forward);

        float timer = 0;
        float animValue = 0;
        while (timer < ranDelay)
        {
            if (animValue > -1)
            {
                animValue -= Time.deltaTime * 2.5f;
                anim.SetFloat("Movement", animValue);
            }

            timer += Time.deltaTime;
            LookAt(target, 0);
            controller.Move(3f * Time.deltaTime * moveDir.normalized);
            yield return null;
        }
        anim.SetFloat("Movement", 0);

        curState = State.Idle;
    }

    protected override IEnumerator Spawn_CutScene()
    {
        // 플레이어 동작 제어
        Player_Manager.instance.Player_Action_Setting(false);

        // 컷신
        video.clip = clips[0];
        video.Play();
        yield return new WaitForSeconds(0.1f);
        while (video.isPlaying)
        {
            yield return null;
        }

        curState = State.Idle;

        // 플레이어 동작 제어
        Player_Manager.instance.Player_Action_Setting(true);
    }

    public override void Die()
    {
        base.Die();
        StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        curState = State.Die;
        enemyUI.gameObject.SetActive(false);

        // 사운드
        sound.Sound(Enemy_Sound.SoundKey.Die.ToString());

        // 애니메이션
        anim.SetTrigger("Hit");
        anim.SetBool("isDie", true);
        while (anim.GetBool("isDie"))
        {
            yield return null;
        }

        // 컷신
        video.clip = clips[1];
        video.Play();
        yield return new WaitForSeconds(0.1f);
        while (video.isPlaying)
        {
            yield return null;
        }

        // 2페이즈 전환
        phaseController.Stage_Spawn(1);
    }
}
