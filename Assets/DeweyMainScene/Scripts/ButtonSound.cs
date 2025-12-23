using UnityEngine;
using UnityEngine.EventSystems; // 이게 있어야 마우스를 감지함

public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    public AudioClip hoverSound; // 호버 소리 파일
    public AudioClip clickSound; // 클릭 소리 파일

    private UIManager uiManager;

    void Start()
    {
 
        uiManager = FindObjectOfType<UIManager>();
    }

    // 마우스 올렸을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (uiManager != null) uiManager.PlaySound(hoverSound);
    }

    // 마우스 눌렀을 때 (Down)
    public void OnPointerDown(PointerEventData eventData)
    {
        if (uiManager != null) uiManager.PlaySound(clickSound);
    }
}