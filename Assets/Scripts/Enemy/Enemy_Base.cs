using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Easing.Tweening;


public abstract class Enemy_Base : MonoBehaviour, IDamageSysteam
{
    [Header("---State---")]
    [SerializeField] private EnemyType enemyType;
    [SerializeField] protected SpawnType spawnType;
    [SerializeField] public State curState;
    [SerializeField] private IDamageSysteam.HitVFX curHit;
    [SerializeField] protected bool canAction;
    [SerializeField] protected bool isAttack;
    public bool isPatten;
    public bool isGroggy;
    private enum EnemyType { Normal, Elite, Boss }
    protected enum SpawnType { None, Movement, CutScene }
    public enum State { None, Spawn, Idle, Think, Chase, Attack, Delay, Groggy, Die }


    [Header("---Status---")]
    public Character_Status_SO statusData;
    public string enemyName;

    // Defence Status
    public int curHp;
    public int maxHp;
    public float curGroggy;
    public float maxGroggy;
    public int physicalDefence;
    public int magicalDefence;

    // Attack Status
    public int physcialDamage;
    public int magicalDamage;
    public float attackSpeed;
    public float criticalhit;
    public float critical_multiplier;

    // Other Status
    public float moveSpeed;
    public float groggyTime;
    public int exp;


    [Header("---Chase---")]
    [SerializeField] protected float attackRange;
    public GameObject target;
    [SerializeField] public Vector3 targetDir;
    [SerializeField] public float targetRange;


    [Header("---Attack---")]
    public List<Attack_Base> attackDatas;


    [Header("---Animation---")]
    [SerializeField] private string[] animTrigger;
    [SerializeField] private string[] animBool;


    [Header("---Item Drop---")]
    [SerializeField] private GameObject dropObject;
    [SerializeField] private Vector2Int dropCount;
    [SerializeField] private List<DropList> itemList;
    [System.Serializable]
    public struct DropList
    {
        public Item_Base item;
        public Vector2Int drop_Count;
        public int drop_Probability;
    }


    [Header("---Component---")]
    public Enemy_UI enemyUI;
    [SerializeField] protected CharacterController controller;
    [SerializeField] protected NavMeshAgent nav;
    [SerializeField] protected Animator anim;
    [SerializeField] protected GameObject body;
    [SerializeField] private GameObject[] hitVFX;
    [SerializeField] private Transform[] knockBackPos;
    [SerializeField] private Transform downPos;
    [SerializeField] protected Transform[] spawnMovePos;
    protected Coroutine movementCoroutine;
    protected Coroutine hitCoroutine;
    private Coroutine attackMovementCoroutine;
    private List<System.Func<IEnumerator>> spawnList;


    /// <summary>
    /// �������� �ʱ�ȭ �� ȣ�� - ��� ���� / ������Ʈ None / ��Ȱ��ȭ
    /// </summary>
    public virtual void Reset_Enemy()
    {
        curState = State.None;
        Hit_Reset();
        gameObject.SetActive(false);
    }

    #region Spawn
    public void Spawn()
    {
        Status_Setting();
        target = PlayerAction_Manager.instance.gameObject;

        spawnList = new List<System.Func<IEnumerator>>()
        {
            Spawn_None, Spawn_Movement, Spawn_CutScene
        };

        StartCoroutine(spawnList[(int)spawnType]());
    }

    private void Status_Setting()
    {
        enemyName = statusData.ObjectName;
        moveSpeed = statusData.MoveSpeed;
        nav.speed = moveSpeed;

        physcialDamage = statusData.PhyScial_Damage;
        magicalDamage = statusData.Magical_Damage;
        criticalhit = statusData.Critical_hit;
        critical_multiplier = statusData.Critical_multiplier;
        attackSpeed = statusData.AttackSpeed;
        anim.SetFloat("AttackSpeed", attackSpeed);

        curHp = statusData.Hp;
        maxHp = statusData.Hp;
        physicalDefence = statusData.Physical_Defence;
        magicalDefence = statusData.Magical_Defence;
        curGroggy = statusData.Groggy;
        maxGroggy = statusData.Groggy;
        groggyTime = statusData.GroggyTime;
        exp = statusData.Exp;
    }

    protected IEnumerator Spawn_None()
    {
        curState = State.Spawn;
        enemyUI.UI_Setting();
        enemyUI.UI_OnOff(true);

        // ��ȯ �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isSpawn", true);
        while (anim.GetBool("isSpawn"))
        {
            yield return null;
        }

        // UI ����
        enemyUI.UI_Setting();
        enemyUI.UI_OnOff(true);

        // ���� - FSM
        curState = State.Idle;
        Think();
    }

    protected IEnumerator Spawn_Movement()
    {
        curState = State.Spawn;

        for (int i = 0; i < spawnMovePos.Length; i++)
        {
            // ���� ���� �ٶ󺸱�
            LookAt(spawnMovePos[i].gameObject, 0.15f);

            // ���� ���
            yield return spawnMovePos[i].DOJump(spawnMovePos[i].position, 3, 1, 1.25f).SetEase(Ease.OutQuad).WaitForCompletion();

            // ���� ���� ������
            yield return new WaitForSeconds(0.15f);
        }

        enemyUI.UI_Setting();
        enemyUI.UI_OnOff(true);
        curState = State.Idle;
    }

    protected abstract IEnumerator Spawn_CutScene();
    #endregion


    #region Attack
    protected abstract void Think();

    public (bool isCritical, int damage) DamageCalculation(Attack_Base.Value attackData)
    {
        // ũ��Ƽ�� ���
        bool isCirtical = criticalhit >= Random.Range(0, 100) ? true : false;

        // ������ ���
        Skill_Value_SO.Value_Data sBase = attackData.levelValue.GetData(0);
        int calDamage = (int)((sBase.type == IDamageSysteam.DamageType.Physical ? physcialDamage : magicalDamage) * Random.Range(sBase.motionValue.x, sBase.motionValue.y) * (isCirtical ? critical_multiplier : 1));

        return (isCirtical, calDamage);
    }

    public void Attack_Movement(Transform movePos, float speed)
    {
        if (attackMovementCoroutine != null)
            StopCoroutine(attackMovementCoroutine);

        StartCoroutine(AttackMovementCall(movePos, speed));
    }

    private IEnumerator AttackMovementCall(Transform movePos, float speed)
    {
        float duration = 1f / speed;
        float elapsed = 0f;

        Vector3 start = transform.position;
        Vector3 target = movePos.position;

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            float easedT = EasingFunctions.OutExpo(t);

            Vector3 current = Vector3.Lerp(start, target, easedT);
            Vector3 delta = current - transform.position;

            controller.Move(delta);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // ������ ��ġ ����
        Vector3 finalOffset = target - transform.position;
        controller.Move(finalOffset);
    }

    public void Check_Target()
    {
        targetDir = PlayerAction_Manager.instance.transform.position - transform.position;
        targetRange = targetDir.magnitude;
    }

    public void LookAt(GameObject target, float lookSpeed)
    {
        /*
        // ���� ���� ��� (Ʃ�丮�� ���� 130~150)
        Vector3 lookDir = (target.transform.position - transform.position).normalized;
        lookDir.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(lookDir);

        transform.DOKill();  // ���� Ʈ���� �ߴ�
        transform.DORotateQuaternion(lookRotation, lookSpeed);  // Quaternion�� ����Ͽ� �ε巴�� ȸ��
        */

        // �ű� ��� (Ʃ�丮�� ���� 150~170)
        if (target == null) return;

        Vector3 dir = target.transform.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        if (lookSpeed <= 0f)
        {
            // ��� ȸ��
            transform.rotation = targetRot;
        }
        else
        {
            // �ε巴�� ȸ�� (1������ ����)
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                lookSpeed * Time.deltaTime
            );
        }
    }

    public void Delay()
    {
        StartCoroutine(DelayMovement());
    }

    protected abstract IEnumerator DelayMovement();
    #endregion


    #region Hit
    public void Take_Damage(GameObject attackObj, IDamageSysteam.DamageType type, IDamageSysteam.HitVFX hit, bool isCirtical, int hitCount, int damage)
    {
        if (curState == State.Die)
        {
            return;
        }

        // ������ ���
        for (int i = 0; i < hitCount; i++)
        {
            int calDamage = damage - (type == IDamageSysteam.DamageType.Physical ? physicalDefence : magicalDefence);
            if (calDamage > 0)
            {
                // ������ & �׷α�
                curHp -= calDamage;
                curGroggy -= (calDamage / 10);

                // ī�޶� ��鸲
                CameraEffect_Manager.instance.Camera_Shack(1, 0.1f);

                // �÷��̾� ���� ������
                if (!Player_Manager.instance.action.isAwakning)
                    Player_Manager.instance.status.Recovery(Player_Status.RecoveryType.Awakening, (int)(calDamage * 0.25f));

                // ��� üũ
                if (curHp <= 0)
                {
                    curHp = 0;
                    Hit_Reset();
                    Die();

                    // ����ġ �߰�
                    Player_Manager.instance.status.ExpAdd(exp);
                    return;
                }
                else
                {
                    // �ǰ� ����Ʈ
                    HitEffect(type, isCirtical, damage);
                }
            }
        }

        // �����̻� üũ - �Ϲ� ���ʹ� ��� / ����Ʈ & ���� ���ʹ� ������ �Ҹ� ��
        if (enemyType == EnemyType.Normal)
        {
            // �Ϲ� ���� üũ
            switch (hit)
            {
                case IDamageSysteam.HitVFX.None:
                    CameraEffect_Manager.instance.Camera_Shack(1.5f, 0.05f);
                    break;

                case IDamageSysteam.HitVFX.KnockBack:
                    if (isGroggy) return;
                    CameraEffect_Manager.instance.Camera_Shack(3, 0.05f);
                    if (hitCoroutine != null) StopCoroutine(hitCoroutine);
                    hitCoroutine = StartCoroutine(Hit_KnockBack(attackObj));
                    break;

                case IDamageSysteam.HitVFX.Down:
                    CameraEffect_Manager.instance.Camera_Shack(5, 0.05f);
                    if (hitCoroutine != null) StopCoroutine(hitCoroutine);
                    hitCoroutine = StartCoroutine(Hit_Down(attackObj));
                    break;
            }
        }
        else
        {
            // ����Ʈ & ���� ���� üũ
            if (curGroggy <= 0 && !isGroggy)
            {
                StartCoroutine(Hit_Groggy());
            }
        }
    }

    private void HitEffect(IDamageSysteam.DamageType type, bool isCirtial, int damage)
    {
        // �ǰ� ����Ʈ
        // Instantiate((type == IDamageSysteam.DamageType.Physical ? hitVFX[0] : hitVFX[1]), transform.position, Quaternion.identity);

        // �ǰ� �� �ٵ� ����
        ShakeEffect(0.1f, isCirtial ? 0.3f : 0.15f);

        // ī�޶� ����ŷ

        // UI �ֽ�ȭ
        enemyUI.Hp();
        enemyUI.Groggy();

        // ������ UI - ����
        // ������ ġ��Ÿ ǥ��ó�� ������ ǥ��
        // ���� : ��� + ������ �ƿ����� / ���� : ��� + �Ķ��� �ƿ�����
        // ġ��Ÿ : �� �������� + (����ü & �׵θ�)
        enemyUI.DamageUI(type, isCirtial, damage);
    }

    private void ShakeEffect(float duration, float strength)
    {
        body.transform.DOKill(); // ���� ��鸲�� �����ϰ� ���ο� ��鸲 ����
        body.transform.DOShakePosition(duration, strength, 10, 90, false, true);
    }

    protected void Hit_Reset()
    {
        nav.enabled = false;

        // �̵� �ڷ�ƾ ����
        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        // �̵� �ʱ�ȭ
        if (attackMovementCoroutine != null)
            StopCoroutine(attackMovementCoroutine);

        // ���� ����
        for (int i = 0; i < attackDatas.Count; i++)
        {
            attackDatas[i].Attack_Reset();
        }

        // �ִϸ��̼� Ʈ���� ����
        for (int i = 0; i < animTrigger.Length; i++)
        {
            anim.ResetTrigger(animTrigger[i]);
        }

        // �ִϸ��̼� ����
        for (int i = 0; i < animBool.Length; i++)
        {
            anim.SetBool(animBool[i], false);
        }
    }

    public IEnumerator Hit_KnockBack(GameObject attackObj)
    {
        curState = State.Groggy;

        // �ൿ ���� ���� -> �ڷ�ƾ�� �����Ű�� ����
        Hit_Reset();

        // �˹� �ִϸ��̼�
        anim.SetTrigger("Hit");
        anim.SetBool("isKnockBack", true);
        anim.SetFloat("AnimValue", 0);
        int ran = Random.Range(0, knockBackPos.Length);
        anim.SetInteger("HitType", ran);

        // �˹� �̵�
        transform.DOKill();
        Vector3 startPos = transform.position;
        Vector3 endPos = knockBackPos[ran].position;
        float timer = 0;
        float moveTime = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime / groggyTime;
            anim.SetFloat("AnimValue", timer);

            moveTime = moveTime < 1 ? moveTime += Time.deltaTime * 2f : 1;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(moveTime));

            yield return null;
        }
        anim.SetFloat("AnimValue", 1);
        anim.SetBool("isKnockBack", false);

        // �絿��
        curState = State.Idle;
    }

    public IEnumerator Hit_Down(GameObject attackObj)
    {
        curState = State.Groggy;
        isGroggy = true;

        // �ൿ ���� ���� -> �ڷ�ƾ�� �����Ű�� ����
        Hit_Reset();

        // �ٿ� �ִϸ��̼�
        anim.SetTrigger("Hit");
        anim.SetBool("isDown", true);

        // �ٿ� �̵�
        transform.DOKill();
        Vector3 startPos = transform.position;
        Vector3 endPos = downPos.position;
        float timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime / groggyTime;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        // �ٿ� ��� -> �ٿ� ���� �ð� �ʿ� -> �������ͽ��� �߰�
        yield return new WaitForSeconds(statusData.GroggyTime);

        // ��� �ִϸ��̼�
        anim.SetTrigger("Action");
        while (anim.GetBool("isDown"))
        {
            yield return null;
        }

        // �絿��
        isGroggy = false;
        curState = State.Idle;
    }

    public IEnumerator Hit_Groggy()
    {
        curState = State.Groggy;
        isGroggy = true;

        // �ൿ ���� ���� -> �ڷ�ƾ�� �����Ű�� ����
        Hit_Reset();

        // �ٿ� �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isDown", true);

        // �ٿ� ��� -> �ٿ� ���� �ð� �ʿ� -> �������ͽ��� �߰�
        yield return new WaitForSeconds(statusData.GroggyTime);

        // ��� �ִϸ��̼�
        anim.SetTrigger("Action");
        while (anim.GetBool("isDown"))
        {
            yield return null;
        }

        // �絿��
        isGroggy = false;
        curGroggy = maxGroggy;
        curState = State.Idle;
    }

    /// <summary>
    /// ������ ��� ����ġ ��� / -1�� ��� �� ����
    /// </summary>
    /// <returns></returns>
    public void Item_Drop()
    {
        if (itemList.Count == 0)
        {
            return;
        }

        // ��� Ƚ����ŭ ������ ���
        for (int i = 0; i < Random.Range(dropCount.x, dropCount.y); i++)
        {
            // ��ü �� ���ϱ�
            List<int> drop = new();
            int total = 0;
            for (int i2 = 0; i2 < itemList.Count; i2++)
            {
                total += itemList[i2].drop_Probability;
                drop.Add(total);
            }

            // �� ����
            int ran = Random.Range(0, total);
            for (int i2 = 0; i2 < drop.Count; i2++)
            {
                if (ran < drop[i2])
                {
                    // ������ ����
                    GameObject obj = Instantiate(dropObject, transform.position, Quaternion.identity);

                    // ���� ����
                    int dropAmount = Random.Range(itemList[i2].drop_Count.x, itemList[i2].drop_Count.y + 1);

                    // ������ ����
                    obj.GetComponent<Item_Drop>().Spawn(itemList[i2].item, dropAmount);
                    break;
                }
            }
        }
    }

    public virtual void Die()
    {
        Item_Drop();
    }
    #endregion
}
