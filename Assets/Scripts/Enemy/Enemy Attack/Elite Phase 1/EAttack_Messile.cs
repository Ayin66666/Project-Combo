using System.Collections;
using UnityEngine;


public class EAttack_Messile : Attack_Base
{
    [Header("---Setting---")]
    [SerializeField] private Enemy_Elite_Phase1 elite;
    [SerializeField] private GameObject shootVFX;
    [SerializeField] private GameObject[] bullet_Messile;
    [SerializeField] private Transform[] shotPos;

    [SerializeField] private GameObject missliePosSet;
    [SerializeField] private Collider missileSpawnPos;
    [SerializeField] private Collider missileTargetPos;
    private Coroutine followCoroutine;
    private Coroutine misslieCoroutine;

    private bool isOn;

    public override void Use()
    {
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        useCoroutine = StartCoroutine(UseCall());
    }

    private IEnumerator UseCall()
    {
        enemy.curState = Enemy_Base.State.Attack;

        enemy.LookAt(PlayerAction_Manager.instance.gameObject, 0.25f);

        isOn = true;

        // ����
        elite.sound.Sound(Enemy_Elite_Phase1.SoundKey.MisslieCharge.ToString());

        // �ִϸ��̼� - ���� ����
        anim.SetTrigger("Action");
        anim.SetBool("isMisslieReady", true);
        anim.SetBool("isMessile", true);
        while (anim.GetBool("isMisslieReady"))
        {
            yield return null;
        }

        followCoroutine = StartCoroutine(Follow());

        // �߻� �κ� �ִϸ��̼� �̺�Ʈ�� ����
        int ran = Random.Range(7, 10);
        for (int i = 0; i < ran; i++)
        {
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

        // ���� - ���� �ִϸ��̼� ���
        anim.SetTrigger("Action");
        while (anim.GetBool("isMessile"))
        {
            yield return null;
        }


        enemy.Delay();
    }

    private IEnumerator Follow()
    {
        missliePosSet.transform.position = enemy.target.transform.position;
        missliePosSet.SetActive(true);
        while (isOn)
        {
            enemy.LookAt(enemy.target, 0f);
            missliePosSet.transform.position = enemy.target.transform.position;
            yield return null;
        }

        missliePosSet.SetActive(false);
    }

    public void Misslie()
    {
        misslieCoroutine = StartCoroutine(MisslieCall());
    }

    private IEnumerator MisslieCall()
    {
        // ����
        elite.sound.Sound(Enemy_Elite_Phase1.SoundKey.MisslieShoot.ToString());

        // �߻� ����Ʈ & �߻�
        for (int i1 = 0; i1 < shotPos.Length; i1++)
        {
            // �߻� ����Ʈ
            Instantiate(shootVFX, shotPos[i1].position, shotPos[i1].rotation);

            // ź ��ȯ
            GameObject obj = Instantiate(bullet_Messile[Random.Range(0, bullet_Messile.Length)], Bullet_PosSetting(missileSpawnPos), Quaternion.identity);

            // ź ������ ����
            Attack_Collider_Shooting bulletObj = obj.GetComponent<Attack_Collider_Shooting>();
            (bool isCritical, int damage) = enemy.DamageCalculation(value_Normal[0]);
            Skill_Value_SO.Value_Data skillData = value_Normal[0].levelValue.GetData(skillLevel);
            bulletObj.Damage_Setting(skillData.type, skillData.attackEffect, isCritical, skillData.hitCount, damage);

            // ź ���� ������ ����
            (bool isCritical1, int damage1) = enemy.DamageCalculation(value_Normal[1]);
            skillData = value_Normal[1].levelValue.GetData(skillLevel);
            bulletObj.hitVFX.GetComponent<Attack_Collider_AOE>().Damage_Setting(skillData.type, skillData.attackEffect, Attack_Collider_AOE.AttackType.SingleHit, isCritical1, skillData.hitCount, damage1, 0.05f);

            // ź �̵� ����
            Vector3 movePos = Bullet_PosSetting(missileTargetPos) - obj.transform.position;
            obj.GetComponent<Attack_Collider_Shooting>().Movement_Target(Bullet_PosSetting(missileTargetPos), 3f, Random.Range(0.35f, 0.45f));

            yield return null;
        }
    }


    private Vector3 Bullet_PosSetting(Collider coll)
    {
        Vector3 originPosition = coll.transform.position;

        // �ݶ��̴��� ����� �������� bound.size ���
        float range_X = coll.bounds.size.x;
        float range_Y = coll.bounds.size.y;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Y = Random.Range((range_Y / 2) * -1, range_Y / 2);
        Vector3 RandomPostion = new Vector3(range_X, range_Y);

        Vector3 respawnPosition = originPosition + RandomPostion;
        return respawnPosition;
    }

    public override void AttackVFX(int index)
    {
        throw new System.NotImplementedException();
    }

    public override void DamageCal(int index)
    {
        throw new System.NotImplementedException();
    }

    public override void Attack_Reset()
    {
        // ���� ����
        if (useCoroutine != null)
            StopCoroutine(useCoroutine);

        if (followCoroutine != null)
            StopCoroutine(followCoroutine);        
        
        if (misslieCoroutine != null)
            StopCoroutine(misslieCoroutine);

        // ����Ʈ ����
        missliePosSet.SetActive(false);

        isOn = false;

        // ����Ʈ ����
        for (int i = 0; i < value_Normal.Count; i++)
        {
            if (!value_Normal[i].attackCollider)
                value_Normal[i].attackCollider.ListReset();
        }
    }
}
