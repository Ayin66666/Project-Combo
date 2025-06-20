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
    /// �ǰ� ������ UI
    /// </summary>
    /// <param name="type"></param>
    /// <param name="isCritical"></param>
    /// <param name="damage"></param>
    public void DamageUI_Setting(IDamageSysteam.DamageType type, bool isCritical, int damage)
    {
        borderImage.color = type == IDamageSysteam.DamageType.Physical ? damageColor[0] : damageColor[1];
        borderImage.sprite = isCritical ? borderSprite[0] : borderSprite[1];
        valueText.text = damage.ToString();

        StartCoroutine(LookAk());
        StartCoroutine(Movement());
    }

    /// <summary>
    /// ȸ�� UI
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public void RecoveryUI_Setting(Player_Status.RecoveryType type, int value)
    {
        borderImage.color = new Color(0, 0, 0, 0);
        valueText.text = value.ToString();
        valueText.color = recoveryColor[(int)type];

        StartCoroutine(LookAk());
        StartCoroutine(Movement());
    }


    #region UI �̵� ����
    private IEnumerator Movement()
    {
        // ���
        yield return new WaitForSeconds(1f);

        // ���� �ö󰡸� �������
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
