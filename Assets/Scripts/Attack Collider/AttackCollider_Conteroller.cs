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
    /// ������ ��� �� �ش� �Լ��� �����͸� �־� �ݶ��̴��� ����!
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
    /// �������� ���� ������Ʈ
    /// </summary>
    /// <param name="obj"></param>
    public void TargetCheck(GameObject obj)
    {
        // Ÿ�� üũ
        foreach (GameObject hit in hitObjects)
        {
            // �̹� �������� �޾Ҵٸ�?
            if(obj == hit)
            {
                return;
            }
        }

        if(obj.GetComponent<IDamageSysteam>() != null)
        {
            // ��� ȿ�� ȣ��
            if(owner == Owner.Player)
                Player_Manager.instance.equipment.Use_ItemEffect();

            // ������
            hitObjects.Add(obj);
            obj.GetComponent<IDamageSysteam>().Take_Damage(gameObject, damageType, hitType, isCritical, attackCount, damage);
        }
        else
        {
            Debug.Log($"���� �Ұ� / ���� �������̽� ����! / ������Ʈ : {obj}");
        }
    }

    /// <summary>
    /// ���� ���� �� ����Ʈ �ʱ�ȭ
    /// </summary>
    public void ListReset()
    {
        hitObjects.Clear();
    }


    public void AttackColliderOn(int index)
    {
        Vector3 boxCenter = attackColliders[index].transform.position;
        Quaternion boxRotation = attackColliders[index].transform.rotation;

        // Transform�� ũ�� ���
        Vector3 boxSize = attackColliders[index].transform.lossyScale * 0.5f;

        // OverlapBox ����
        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxSize, boxRotation, targetLayer);
        foreach (Collider collider in hitColliders)
        {
            TargetCheck(collider.gameObject);
        }

        currentIndex = index;
        Invoke(nameof(Re), 0.5f); // -> �̰� ȭ�� Ȯ�ο��ε� �����̽� �������� �־ �ϴ� ��Ȱ��ȭ
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
            return; // ��ȿ�� �ε����� �ƴ� ��� �׸��� ����
        }

        Vector3 boxCenter = attackColliders[currentIndex].transform.position;
        Quaternion boxRotation = attackColliders[currentIndex].transform.rotation;
        Vector3 boxSize = attackColliders[currentIndex].transform.lossyScale * 0.5f;

        Gizmos.color = Color.blue;
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, boxRotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize * 2f);
        
    }
}
