using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DamageUI : MonoBehaviour
{
    [Header("--- UI ---")]
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image borderImage;
    [SerializeField] private Transform movePos;


    [Header("--- Damage UI Setting ---")]
    [SerializeField] private Sprite[] borderSprite;
    [SerializeField] private Color[] damageColor;


    [Header("--- Recovery UI Setting ---")]
    [SerializeField] private Color[] recoveryColor;


    /// <summary>
    /// 피격 데미지 UI
    /// </summary>
    /// <param name="type"></param>
    /// <param name="isCritical"></param>
    /// <param name="damage"></param>
    public void DamageUI_Setting(IDamageSysteam.DamageType type, bool isCritical, int damage)
    {
        Debug.Log($"타입 : {type} 크리티컬 : {isCritical} 데미지 : {damage}");
        
        // 보더 셋팅
        borderImage.color = type == IDamageSysteam.DamageType.Physical ? damageColor[0] : damageColor[1];
        borderImage.sprite = isCritical ? borderSprite[0] : borderSprite[1];
        borderImage.gameObject.SetActive(true);
        
        // 텍스트 셋팅
        valueText.text = damage.ToString();

        // UI 동작
        StartCoroutine(LookAk());
        StartCoroutine(Movement());
    }

    /// <summary>
    /// 회복 UI
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public void RecoveryUI_Setting(Player_Status.RecoveryType type, int value)
    {
        Debug.Log($"타입 : {type}회복 : {value}");

        // 보더 셋팅
        borderImage.gameObject.SetActive(false);

        // 텍스트 셋팅
        valueText.color = recoveryColor[(int)type];
        valueText.text = value.ToString();

        // UI 동작
        StartCoroutine(LookAk());
        StartCoroutine(Movement());
    }


    #region UI 이동 로직
    private IEnumerator Movement()
    {
        // 대기
        yield return new WaitForSeconds(1f);

        // 위로 올라가며 사라지기
        transform.DOMove(movePos.position, 1f).SetEase(Ease.OutQuad);
        canvasGroup.DOFade(0, 1f).SetEase(Ease.OutQuad).OnComplete(() => Destroy(gameObject));
    }

    private IEnumerator LookAk()
    {
        while (true)
        {
            Vector3 lookDir = (PlayerAction_Manager.instance.cam.position - transform.position).normalized;
            lookDir.y = 0;
            transform.rotation = Quaternion.LookRotation(-lookDir);
            yield return null;
        }
    }
    #endregion
}
