using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    [SerializeField] private Image icon = default;
    [SerializeField] private Image outlineIcon = default;
    [SerializeField] private Image fillImage = default;
    public string title;

    public void SetIcon(Sprite s)
    {
        icon.sprite = s;
    }
    public void SetOutlineIcon(Sprite s)
    {
        outlineIcon.sprite = s;
    }
    public void SetColour(Color c)
    {
        fillImage.color = c;
    }
    public void SetTransform((int, int) position)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(position.Item1, position.Item2);
    }
    public void ShowCoolDown(float cooldown, bool reversed = false)
    {
        fillImage.fillAmount = reversed ? 1f : 0f;
        DOTween.To(() => fillImage.fillAmount, x => fillImage.fillAmount = x, reversed ? 0f : 1f, cooldown)
            .SetEase(Ease.Linear).OnComplete(() =>
            {
                if (reversed)
                {
                    gameObject.SetActive(false);
                }
            });
    }
}
