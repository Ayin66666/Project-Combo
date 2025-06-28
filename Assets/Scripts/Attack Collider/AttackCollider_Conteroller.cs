using System.Collections.Generic;
using UnityEngine;


public class AttackCollider_Controller : MonoBehaviour
{

    [Header("---Damage Status---")]
    [SerializeField] private IDamageSysteam.DamageType damageType;
    [SerializeField] private IDamageSysteam.HitVFX hitType;
    [SerializeField] private int damage;
    [SerializeField] private int attackCount;
    [SerializeField] private bool isCritical;

    [Header("--- Setting ---")]
    [SerializeField] private Owner owner;
    [SerializeField] private int currentIndex;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private List<GameObject> attackColliders;
    private HashSet<GameObject> hitObjects = new HashSet<GameObject>();
    public enum Owner { Player, Enemy }


    /// <summary>
    /// 데미지 계산 후 해당 함수에 데이터를 넣어 콜라이더에 적용!
    /// </summary>
    /// <param name="damageType"></param>
    /// <param name="hitType"></param>
    /// <param name="isCritical"></param>
    /// <param name="attackCount"></param>
    /// <param name="damage"></param>
    public void Damage_Setting(IDamageSysteam.DamageType damageType, IDamageSysteam.HitVFX hitType, bool isCritical, int attackCount, int damage, Owner owner)
    {
        // Debug.Log($"Call Damage Setting {damageType} : {hitType} : {isCritical} :{attackCount} : {damage}");
        this.owner = owner;
        this.damageType = damageType;
        this.hitType = hitType;
        this.isCritical = isCritical;
        this.attackCount = attackCount;
        this.damage = damage;
    }

    /// <summary>
    /// 데미지를 받을 오브젝트
    /// </summary>
    /// <param name="obj"></param>
    public void TargetCheck(GameObject obj)
    {
        // 타겟 체크
        foreach (GameObject hit in hitObjects)
        {
            // 이미 데미지를 받았다면?
            if(obj == hit)
            {
                return;
            }
        }

        if(obj.GetComponent<IDamageSysteam>() != null)
        {
            // 장비 효과 호출
            if(owner == Owner.Player)
                Player_Manager.instance.equipment.Use_ItemEffect();

            // 데미지
            hitObjects.Add(obj);
            obj.GetComponent<IDamageSysteam>().Take_Damage(gameObject, damageType, hitType, isCritical, attackCount, damage);
        }
        else
        {
            Debug.Log($"공격 불가 / 판정 인터페이스 없음! / 오브젝트 : {obj}");
        }
    }

    /// <summary>
    /// 공격 종료 후 리스트 초기화
    /// </summary>
    public void ListReset()
    {
        hitObjects.Clear();
    }


    public void AttackColliderOn(int index)
    {
        Vector3 boxCenter = attackColliders[index].transform.position;
        Quaternion boxRotation = attackColliders[index].transform.rotation;

        // Transform의 크기 사용
        Vector3 boxSize = attackColliders[index].transform.lossyScale * 0.5f;

        // OverlapBox 실행
        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxSize, boxRotation, targetLayer);
        foreach (Collider collider in hitColliders)
        {
            TargetCheck(collider.gameObject);
        }

        currentIndex = index;
        Invoke(nameof(Re), 0.5f); // -> 이거 화면 확인용인데 성능이슈 있을수도 있어서 일단 비활성화
    }

    private void Re()
    {
        currentIndex = -1;
    }

    private void OnDrawGizmos()
    {
        /*
        for (int i = 0; i < attackColliders.Count; i++)
        {
            Vector3 boxCenter = attackColliders[i].transform.position;
            Quaternion boxRotation = attackColliders[i].transform.rotation;
            Vector3 boxSize = attackColliders[i].transform.lossyScale * 0.5f;

            Gizmos.color = Color.blue;
            Gizmos.matrix = Matrix4x4.TRS(boxCenter, boxRotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize * 2f);
        }
        */

        
        if (currentIndex < 0 || currentIndex >= attackColliders.Count)
        {
            return; // 유효한 인덱스가 아닐 경우 그리지 않음
        }

        Vector3 boxCenter = attackColliders[currentIndex].transform.position;
        Quaternion boxRotation = attackColliders[currentIndex].transform.rotation;
        Vector3 boxSize = attackColliders[currentIndex].transform.lossyScale * 0.5f;

        Gizmos.color = Color.blue;
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, boxRotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize * 2f);
        
    }
}
