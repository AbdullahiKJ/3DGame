using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    [SerializeField] private Image icon = default;
    [SerializeField] private Image fillImage = default;

    public void SetIcon(Sprite s)
    {
        icon.sprite = s;
    }
    public void ShowCoolDown(float cooldown)
    {
        fillImage.fillAmount = 0f;
        DOTween.To(() => fillImage.fillAmount, x => fillImage.fillAmount = x, 1f, cooldown)
            .SetEase(Ease.Linear);
    }
}
