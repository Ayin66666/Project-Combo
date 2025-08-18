using System.Collections;
using UnityEngine;


public class CutScene_Shooting : MonoBehaviour
{
    [Header("--- Movement Setting ---")]
    [SerializeField] private CutScene_ElitePhase2End end;
    [SerializeField] private Vector3 moveDir;
    [SerializeField] private float speed;
    [SerializeField] private float lifeTimer;
    private Coroutine hitCoroutine;


    [Header("---VFX---")]
    public GameObject hitVFX;


    public void Movement_Setting(Vector3 moveDir, float moveSpeed, float lifeTime, CutScene_ElitePhase2End end)
    {
        this.end = end;
        this.moveDir = moveDir;
        speed = moveSpeed;
        lifeTimer = lifeTime;

        hitCoroutine = StartCoroutine(Movement());
    }

    private IEnumerator Movement()
    {
        while (lifeTimer > 0)
        {
            transform.position += moveDir * speed * Time.deltaTime;
            lifeTimer -= Time.deltaTime;
            yield return null;
        }

        Hit();
    }

    private void Hit()
    {
        end.BulletHit();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(hitCoroutine != null) 
                StopCoroutine(hitCoroutine);

            Hit();
        }
    }
}
