using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using Fungus; 

public class IntroCutsceneManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Image fadePanel; 
    public Image cutsceneImage; 
    public TextMeshProUGUI narrationText; 
    public GameObject clickIndicator; 
    
    [Header("Data and Settings")]
    public CutsceneData cutsceneData;
    public string nextSceneName = "MainGameScene"; 
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
            // 인트로는 애니메이터가 없을 수도 있으니 에러 로그 대신 경고만 하거나 생략 가능
            if (cutsceneAnimator == null) Debug.LogWarning("Animator not found on CutsceneImage");
        }
        
        narrationText.text = "";
        
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
        // 클릭 감지
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                skipTyping = true; // 타이핑 스킵
            }
            else if (waitingForClick)
            {
                waitingForClick = false;
                
                if (clickIndicator != null)
                {
                    clickIndicator.SetActive(false);
                }
                
                // 다음 단계로 진행
                NextStep();
            }
        }
    }
    
    IEnumerator StartCutscene()
    {
        // --- Fungus 잔상 제거 (필요하다면 유지) ---
        Fungus.Flowchart[] flowcharts = FindObjectsOfType<Fungus.Flowchart>();
        foreach (Fungus.Flowchart flowChart in flowcharts)
        {
            if (flowChart.gameObject.scene.buildIndex != SceneManager.GetActiveScene().buildIndex)
            {
                flowChart.gameObject.SetActive(false);
            }
        }
        
        GameObject fungusManager = GameObject.Find("FungusManager"); 
        if (fungusManager != null)
        {
            fungusManager.SetActive(false);
            yield return null; 
        }
        // ---------------------------------------

        yield return StartCoroutine(Fade(0f)); // 페이드 인 (화면 밝아짐)
        
        NextStep(); // 첫 단계 시작
    }
    
    void NextStep()
    {
        currentStepIndex++;
        
        // 데이터의 끝에 도달했으면 인트로 종료 (게임 씬으로 이동)
        if (cutsceneData == null || currentStepIndex >= cutsceneData.steps.Length)
        {
            StartCoroutine(EndCutscene());
            return;
        }
        
        CutsceneStep currentStep = cutsceneData.steps[currentStepIndex];
        
        // 이미지 변경 및 애니메이션
        if (currentStep.changeImage && currentStep.newCutsceneImage != null)
        {
            cutsceneImage.sprite = currentStep.newCutsceneImage;
            if (cutsceneAnimator != null) cutsceneAnimator.SetTrigger(IMAGE_ANIMATION_TRIGGER);
        } 
        else if (currentStepIndex == 0 && cutsceneAnimator != null)
        {
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
            if (skipTyping) break;
            
            narrationText.text += letter;
            if (typingSound != null && audioSource != null) audioSource.Play();
            yield return new WaitForSeconds(typingSpeed);
        }
        
        if (skipTyping) narrationText.text = textToType;
        
        isTyping = false;
        
        if (clickIndicator != null) clickIndicator.SetActive(true);

        waitingForClick = true; // 다 출력되면 클릭 대기
    }

// 인트로 종료 및 씬 이동
     IEnumerator EndCutscene()
    {
        // 텍스트 비우기
        narrationText.text = "";
        if (clickIndicator != null) clickIndicator.SetActive(false);

        // 씬 페이드 아웃 (화면 어두워짐)
        yield return StartCoroutine(Fade(1f));
        
        // BGM 페이드 아웃
        yield return StartCoroutine(FadeOutBGM(bgmFadeDuration));

        
        // 변경: 로딩 화면을 띄우면서 이동
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadSceneAsync(nextSceneName);
        }
        else
        {
            // 혹시 SystemLoader를 깜빡했을 경우를 대비한 보험
            Debug.LogWarning("SceneLoader가 없습니다! 기본 로드로 이동합니다.");
            SceneManager.LoadScene(nextSceneName);
        }
    }
    
    IEnumerator FadeOutBGM(float duration)
    {
        if (bgmAudioSource == null) yield break;

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