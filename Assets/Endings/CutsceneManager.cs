using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using Fungus; 

public class CutsceneManager : MonoBehaviour
{

    [Header("UI Elements")]
    public Image fadePanel; 
    public Image cutsceneImage; 
    public TextMeshProUGUI narrationText; 
    public TextMeshProUGUI gameOverText; 
    public GameObject clickIndicator;
    
    [Header("Data and Settings")]
    public CutsceneData cutsceneData;
    public string startSceneName = "StartScene"; 
    public float typingSpeed = 0.05f; 
    public float fadeDuration = 1.5f; 
    public AudioClip typingSound; 
    
    [Header("Audio Settings")]
    public AudioSource bgmAudioSource;
    public float bgmFadeDuration = 1.0f;
    
    private int currentStepIndex = -1;
    private bool isTyping = false;
    private bool waitingForClick = false;
    private bool skipTyping = false;
    private AudioSource audioSource;
    private Animator cutsceneAnimator;
    
    private const string IMAGE_ANIMATION_TRIGGER = "StartCutsceneAnimation";

    void Awake()
    {
        // 컴포넌트 설정
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = typingSound;
        audioSource.loop = false;
        
        // 이미지 애니메이터 설정
        if (cutsceneImage != null)
        {
            cutsceneAnimator = cutsceneImage.GetComponent<Animator>();
            if (cutsceneAnimator == null)
            {
                Debug.LogError("CutsceneImage에 Animator 컴포넌트가 없습니다!");
            }
        }
                narrationText.text = "";
        gameOverText.gameObject.SetActive(false);
        
        if (clickIndicator != null)
        {
            clickIndicator.SetActive(false);
        }
        
        // Fade Panel 초기 상태: 완전 불투명 (Alpha 1)
        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            c.a = 1f;
            fadePanel.color = c;
        }
    }
    
    void Start()
    {
        StartCoroutine(StartCutscene());
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                skipTyping = true;
            }
            else if (waitingForClick)
            {
                waitingForClick = false;
                
                if (clickIndicator != null)
                {
                    clickIndicator.SetActive(false);
                }
                
                // Game Over 텍스트가 활성화되었으면 씬 전환(EndCutscene), 아니면 다음 단계(NextStep)
                if (gameOverText.gameObject.activeSelf)
                {
                    StartCoroutine(EndCutscene());
                }
                else
                {
                    NextStep();
                }
            }
        }
    }
    

    IEnumerator StartCutscene()
    {
        Fungus.Flowchart[] flowcharts = FindObjectsOfType<Fungus.Flowchart>();
        foreach (Fungus.Flowchart flowChart in flowcharts)
        {
            if (flowChart.gameObject.scene.buildIndex != SceneManager.GetActiveScene().buildIndex)
            {
                flowChart.gameObject.SetActive(false);
                Debug.Log($"DontDestroyOnLoad Flowchart 비활성화: {flowChart.name}");
            }
        }
        
        GameObject fungusManager = GameObject.Find("FungusManager"); 
        if (fungusManager != null)
        {
            fungusManager.SetActive(false);
            Debug.Log("FungusManager를 찾아 비활성화했습니다.");
            yield return null; 
        }

        yield return StartCoroutine(Fade(0f)); 
        
        // 컷씬의 첫 번째 단계로 이동
        NextStep();
    }
    
    void NextStep()
    {
        currentStepIndex++;
        
        if (cutsceneData == null || currentStepIndex >= cutsceneData.steps.Length)
        {
            StartCoroutine(EndNarration());
            return;
        }
        
        CutsceneStep currentStep = cutsceneData.steps[currentStepIndex];
        
        // 이미지 변경 및 애니메이션 트리거
        if (currentStep.changeImage && currentStep.newCutsceneImage != null)
        {
            cutsceneImage.sprite = currentStep.newCutsceneImage;
            
            if (cutsceneAnimator != null)
            {
                cutsceneAnimator.SetTrigger(IMAGE_ANIMATION_TRIGGER);
            }
        } 
        else if (currentStepIndex == 0 && cutsceneAnimator != null)
        {
            // 첫 단계 애니메이션
            cutsceneAnimator.SetTrigger(IMAGE_ANIMATION_TRIGGER);
        }

        StartCoroutine(TypeText(currentStep.narrationText));
    }
    
    IEnumerator TypeText(string textToType)
    {
        isTyping = true;
        skipTyping = false;
        narrationText.text = "";
        
        foreach (char letter in textToType.ToCharArray())
        {
            if (skipTyping)
            {
                break;
            }
            
            narrationText.text += letter;
            
            if (typingSound != null && audioSource != null)
            {
                audioSource.Play();
            }
            
            yield return new WaitForSeconds(typingSpeed);
        }
        
        if (skipTyping)
        {
            narrationText.text = textToType;
        }
        
        isTyping = false;
        
        if (clickIndicator != null)
        {
            clickIndicator.SetActive(true);
        }

        waitingForClick = true;
    }
    
    IEnumerator EndNarration()
    {
        narrationText.text = ""; 

        if (clickIndicator != null)
        {
            clickIndicator.SetActive(false);
        }
        
        gameOverText.gameObject.SetActive(true);
        
        Animator gameOverAnimator = gameOverText.GetComponent<Animator>();

        if (gameOverAnimator != null)
        {
            gameOverAnimator.SetTrigger("StartGameOver");
        }

        yield return new WaitForSeconds(1.0f);
        
        waitingForClick = true; 
        
        yield break;
    }
            
    IEnumerator EndCutscene()
    {
        // 씬 페이드인 (검은 화면으로)
        yield return StartCoroutine(Fade(1f));
        
        // BGM 페이드 아웃
        yield return StartCoroutine(FadeOutBGM(bgmFadeDuration));
        
        // StartScene으로 복귀
        SceneManager.LoadScene(startSceneName);
    }
    
    IEnumerator FadeOutBGM(float duration)
    {
        if (bgmAudioSource == null) 
        {
            Debug.LogWarning("BGM AudioSource가 연결되지 않았습니다. 음악 페이드 아웃을 건너뜁니다.");
            yield break;
        }

        float startVolume = bgmAudioSource.volume;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            bgmAudioSource.volume = Mathf.Lerp(startVolume, 0, time / duration);
            yield return null;
        }

        bgmAudioSource.volume = 0;
        bgmAudioSource.Stop(); 
    }

    IEnumerator Fade(float targetAlpha)
    {
        if (fadePanel == null) yield break;

        float startAlpha = fadePanel.color.a;
        float time = 0;
        
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            Color c = fadePanel.color;
            c.a = alpha;
            fadePanel.color = c;
            yield return null;
        }
        
        Color finalColor = fadePanel.color;
        finalColor.a = targetAlpha;
        fadePanel.color = finalColor;
    }
}