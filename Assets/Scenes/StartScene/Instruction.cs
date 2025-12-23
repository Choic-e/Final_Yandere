using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Instruction : MonoBehaviour
{
    [Header("UI 설정")]
    [Tooltip("설명 UI의 최상위 부모 (HowtoPlay 오브젝트 자체)")]
    public GameObject instructionPanel;
    
    [Tooltip("순서대로 보여줄 이미지 목록")]
    public List<Sprite> instructionImages = new List<Sprite>();
    
    [Tooltip("실제 이미지가 표시될 Image 컴포넌트 (PanelContent 안의 Image)")]
    public Image displayImage; 

    [Header("컴포넌트")]
    public Animator panelAnimator; 
    
    [Header("효과음 설정")]
    public AudioSource audioSource;
    public AudioClip arrowClickSound;
    public AudioClip closeSound;

    private int currentImageIndex = 0;
    private readonly string OpenTrigger = "Open";
    private readonly string CloseTrigger = "Close";

    void Start()
    {
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }
    }

    public void OpenInstruction()
    {
        if (instructionImages.Count == 0)
        {
            Debug.LogWarning("이미지가 없습니다!");
            return;
        }

        instructionPanel.SetActive(true);
        
        if (panelAnimator != null)
        {

            panelAnimator.ResetTrigger("Close"); 
            
            panelAnimator.SetTrigger("Open");
            
        }

        currentImageIndex = 0;
        UpdateImage();
    }

    public void CloseInstruction()
    {
        PlaySound(closeSound);

        if (panelAnimator != null)
        {
            panelAnimator.ResetTrigger(OpenTrigger);
            
            panelAnimator.SetTrigger(CloseTrigger);
        }
        else
        {
            instructionPanel.SetActive(false);
        }
    }
    
    public void DeactivatePanel()
    {
        instructionPanel.SetActive(false);
    }
    
    public void NextImage()
    {
        PlaySound(arrowClickSound);
        currentImageIndex = (currentImageIndex + 1) % instructionImages.Count;
        UpdateImage();
    }
    
    public void PreviousImage()
    {
        PlaySound(arrowClickSound);
        currentImageIndex--;
        if (currentImageIndex < 0)
        {
            currentImageIndex = instructionImages.Count - 1; 
        }
        UpdateImage();
    }
    
    private void UpdateImage()
    {
        if (displayImage != null && instructionImages.Count > 0)
        {
            displayImage.sprite = instructionImages[currentImageIndex];
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}