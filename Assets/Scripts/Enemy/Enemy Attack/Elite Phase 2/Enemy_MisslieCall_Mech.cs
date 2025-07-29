using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Enemy_MisslieCall_Mech : MonoBehaviour
{
    [Header("---Damage Setting---")]
    [SerializeField] private List<Damage> damage;
    private GameObject target;

    [System.Serializable]
    public struct Damage
    {
        public IDamageSysteam.DamageType damageType;
        public IDamageSysteam.HitVFX hitVFX;
        public bool isCritical;
        public int hitCount;
        public int damage;
    }


    [Header("---Setting---")]
    [SerializeField] private GameObject missliePosSet;
    [SerializeField] private Collider misslieSpawnCollider;
    [SerializeField] private Collider misslieTargetCollider;
    [SerializeField] private AudioClip[] clips;


    [Header("---VFX---")]
    [SerializeField] private GameObject shootVFX;
    [SerializeField] private GameObject destoryVFX;
    [SerializeField] private GameObject[] misslies;


    [Header("---Component---")]
    private AudioSource audio;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform[] shotPos;
    private bool isOn;


    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }


    public void Damage_Setting(IDamageSysteam.DamageType dType, IDamageSysteam.HitVFX hVFX, bool isCri, int count, int dam, GameObject target)
    {
        Damage newDamage = new Damage
        {
            damageType = dType,
            hitVFX = hVFX,
            isCritical = isCri,
            hitCount = count,
            damage = dam,
        };

        damage.Add(newDamage);
        this.target = target;
    }

    public void Use()
    {
        StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        LookAt(PlayerAction_Manager.instance.gameObject, 0.25f);
        isOn = true;

        // 애니메이션 - 비행 시작
        anim.SetTrigger("Action");
        anim.SetBool("isMisslieReady", true);
        anim.SetBool("isMessile", true);
        while (anim.GetBool("isMisslieReady"))
        {
            yield return null;
        }

        StartCoroutine(Follow());

        // 발사 부분 애니메이션 이벤트로 제어
        int ran = Random.Range(7, 10);
        for (int i = 0; i < ran; i++)
        {
            // 사운드
            audio.clip = clips[0];
            audio.Play();

            // 애니메이션
            anim.SetBool("isMisslieShooting", true);
            anim.SetTrigger("Action");
            Misslie();
            while (anim.GetBool("isMisslieShooting"))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }

        isOn = false;

        // 사운드
        audio.clip = clips[1];
        audio.Play();

        // 종료 - 폭발 애니메이션 대기
        anim.SetTrigger("Action");
        anim.SetBool("isMessile", true);
        while(anim.GetBool("isMessile"))
        {
            yield return null;
        }
        Instantiate(destoryVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private IEnumerator Follow()
    {
        missliePosSet.transform.position = target.transform.position;
        missliePosSet.SetActive(true);
        while (isOn)
        {
            LookAt(target, 0f);
            missliePosSet.transform.position = target.transform.position;
            yield return null;
        }

        missliePosSet.SetActive(false);
    }

    public void Misslie()
    {
        StartCoroutine(MisslieCall());
    }

    private IEnumerator MisslieCall()
    {
        // 발사 이펙트 & 발사
        for (int i1 = 0; i1 < shotPos.Length; i1++)
        {
            // 발사 이펙트
            Instantiate(shootVFX, shotPos[i1].position, shotPos[i1].rotation);

            // 탄 소환
            GameObject obj = Instantiate(misslies[Random.Range(0, misslies.Length)], Bullet_PosSetting(misslieSpawnCollider), Quaternion.identity);

            // 탄 데미지 셋팅
            Attack_Collider_Shooting bulletObj = obj.GetComponent<Attack_Collider_Shooting>();
            bulletObj.Damage_Setting(damage[0].damageType, damage[0].hitVFX, damage[0].isCritical, damage[0].hitCount, damage[0].damage);

            // 탄 폭발 데미지 셋팅
            bulletObj.hitVFX.GetComponent<Attack_Collider_AOE>().Damage_Setting(damage[1].damageType, damage[1].hitVFX, Attack_Collider_AOE.AttackType.SingleHit, damage[1].isCritical, damage[1].hitCount, damage[1].damage, 0.05f);

            // 탄 이동 셋팅
            Vector3 movePos = Bullet_PosSetting(misslieTargetCollider) - obj.transform.position;
            obj.GetComponent<Attack_Collider_Shooting>().Movement_Target(Bullet_PosSetting(misslieTargetCollider), 3f, Random.Range(0.35f, 0.45f));

            yield return null;
        }
    }

    private Vector3 Bullet_PosSetting(Collider coll)
    {
        Vector3 originPosition = coll.transform.position;

        // 콜라이더의 사이즈를 가져오는 bound.size 사용
        float range_X = coll.bounds.size.x;
        float range_Y = coll.bounds.size.y;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Y = Random.Range((range_Y / 2) * -1, range_Y / 2);
        Vector3 RandomPostion = new Vector3(range_X, range_Y);

        Vector3 respawnPosition = originPosition + RandomPostion;
        return respawnPosition;
    }

    public void LookAt(GameObject target, float lookSpeed)
    {
        Vector3 lookDir = (target.transform.position - transform.position).normalized;
        lookDir.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(lookDir);

        transform.DOKill();  // 기존 트위닝 중단
        transform.DORotateQuaternion(lookRotation, lookSpeed);  // Quaternion을 사용하여 부드럽게 회전
    }

    public void Groggy()
    {
        Instantiate(destoryVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
