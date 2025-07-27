using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "Item Effect", menuName = "Item Effect/Bullet", order = int.MaxValue)]
public class Item_Effect_BulletShooting : Item_Effect_SO
{
    [Header("---Damage Setting---")]
    [SerializeField] private IDamageSysteam.DamageType damageType;
    [SerializeField] private IDamageSysteam.HitVFX hitVFX;
    [SerializeField] private Vector2Int minMaxDamage;
    [SerializeField] private int hitCount;
    [SerializeField] private Vector2Int spawnCount;

    [Header("---Movement Setting---")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float lifeTime;
    [SerializeField] private float shootDelay;
    [SerializeField] private GameObject bulletObj;


    public override void Effect()
    {
        Player_Manager.instance.cooldown.EffectUse(Key, Shooting(), Cooldown);
    }

    private GameObject FindEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(Player_Manager.instance.Player.transform.position, 15f, LayerMask.GetMask("Enemy")); // "Enemy" 레이어 사용 시
        List<GameObject> nearbyEnemies = new List<GameObject>();

        foreach (var col in colliders)
        {
            if (col.CompareTag("Enemy"))
                nearbyEnemies.Add(col.gameObject);
        }

        if (nearbyEnemies.Count == 0)
            return null;

        GameObject targetEnemy = nearbyEnemies[Random.Range(0, nearbyEnemies.Count)];
        return targetEnemy;
    }


    private IEnumerator Shooting()
    {
        Debug.Log($"Bullet Call!");
        for (int i = 0; i < Random.Range(spawnCount.x, spawnCount.y); i++)
        {
            Debug.Log($"Bullet! {i}");
            Vector3 pos = Player_Manager.instance.action.HitVFXPos();
            GameObject obj = Instantiate(bulletObj, pos, Quaternion.identity);
            Attack_Collider_Shooting shoot = obj.GetComponent<Attack_Collider_Shooting>();

            // 데미지 셋팅
            shoot.Damage_Setting(damageType, hitVFX, false, hitCount, Random.Range(minMaxDamage.x, minMaxDamage.y));

            // 이동 셋팅
            GameObject enemy = FindEnemy();
            Vector3 moveDir = enemy != null ? 
                enemy.transform.position - pos : Player_Manager.instance.transform.forward;
            shoot.Movement_Setting(moveDir.normalized, moveSpeed, lifeTime);

            yield return new WaitForSeconds(shootDelay);
        }
    }
}
