using DG.Tweening;
using UnityEngine;

public class Cinemascope : MonoBehaviour
{
    [SerializeField] GameObject defaultUI;
    [SerializeField] GameObject cinemaUI;
    [SerializeField] RectTransform topBar;
    [SerializeField] RectTransform bottomBar;
    Vector3 initialPos;
    Vector3 finalPos;
    [SerializeField] float duration = 1f;

    void Start()
    {
        cinemaUI.SetActive(false);
        initialPos = new Vector3(0f, 605f, 0f);
        finalPos = new Vector3(0f, 475f, 0f);
    }

    public void ShowBars()
    {
        cinemaUI.SetActive(true);
        defaultUI.SetActive(false);
        topBar.anchoredPosition3D = initialPos;
        bottomBar.anchoredPosition3D = initialPos * -1f;

        DOTween.To(() => topBar.anchoredPosition3D, (x) => topBar.anchoredPosition3D = x, finalPos, duration);
        DOTween.To(() => bottomBar.anchoredPosition3D, (x) => bottomBar.anchoredPosition3D = x, finalPos * -1f, duration);
    }

    public void HideBars()
    {
        DOTween.To(() => topBar.anchoredPosition3D, (x) => topBar.anchoredPosition3D = x, initialPos, duration);
        DOTween.To(() => bottomBar.anchoredPosition3D, (x) => bottomBar.anchoredPosition3D = x, initialPos * -1f, duration)
            .OnComplete(() =>
            {
                cinemaUI.SetActive(false);
                defaultUI.SetActive(true);
            });
    }
}
