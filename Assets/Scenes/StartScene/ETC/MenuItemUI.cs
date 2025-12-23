using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MenuItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TextMeshProUGUI label;
    public GameObject arrow;
    public AudioSource sfxHover;
    public AudioSource sfxClick;

    public float normalAlpha = 0.5f;
    public float highlightAlpha = 1f;

    void Start()
    {
        SetAlpha(normalAlpha);
        arrow.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetAlpha(highlightAlpha);
        arrow.SetActive(true);

        if (sfxHover != null)
            sfxHover.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetAlpha(normalAlpha);
        arrow.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (sfxClick != null)
            sfxClick.Play();

    }

    private void SetAlpha(float a)
    {
        Color c = label.color;
        c.a = a;
        label.color = c;
    }
}
