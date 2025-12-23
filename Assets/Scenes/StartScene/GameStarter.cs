using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameStarter : MonoBehaviour
{
    [Header("시작 설정")]
    [Tooltip("애니메이터 컴포넌트가 있는 오브젝트")]
    public GameObject tore1Object;
    
    [Tooltip("이동할 씬의 이름")]
    public string nextSceneName = "GameScene";

    [Header("페이드 설정")]
    [Tooltip("페이드 아웃을 위한 Canvas Group 컴포넌트가 있는 오브젝트")]
    public CanvasGroup fadePanelCanvasGroup; 
    
    [Tooltip("페이드 아웃이 완료되는 데 걸리는 시간 (초)")]
    public float fadeDuration = 0.5f; 

    [Header("컴포넌트")]
    [Tooltip("효과음을 재생할 AudioSource 컴포넌트")]
    public AudioSource audioSource;
    public AudioClip startClickSound;

    private Animator tore1Animator;
    private readonly string startGameTrigger = "Startgame";
    private bool isFading = false;

    void Start()
    {
        // 1. 시작할 때 페이드 패널 끄기 (비활성화 상태로 시작)
        if (fadePanelCanvasGroup != null)
        {
            // CanvasGroup의 자식들은 Raycast를 막지 않도록 설정하고, 투명하게 만듭니다.
            fadePanelCanvasGroup.alpha = 0f;
            fadePanelCanvasGroup.blocksRaycasts = false;
        }

        if (tore1Object != null)
        {
            tore1Animator = tore1Object.GetComponent<Animator>();
        }

        if (tore1Animator == null)
        {
            Debug.LogError("Tore1 오브젝트에 Animator 컴포넌트가 없거나, tore1Object 변수가 할당되지 않았습니다.");
        }
    }

    // Menu_Start 버튼의 OnClick 이벤트에 연결할 함수
    public void StartGameSequence()
    {
        if (isFading) return; 
        isFading = true;

        // 1. 효과음 재생 (페이드와 동시 시작)
        if (audioSource != null && startClickSound != null)
        {
            audioSource.PlayOneShot(startClickSound);
        }

        // 2. 애니메이션 트리거 활성화
        if (tore1Animator != null)
        {
            tore1Animator.SetTrigger(startGameTrigger);
            Debug.Log($"'{startGameTrigger}' 트리거 활성화.");
        }
        
        // 3. 페이드 아웃 코루틴 시작
        if (fadePanelCanvasGroup != null)
        {
            StartCoroutine(FadeOutAndLoad());
        }
        else
        {
            // 페이드 패널이 없으면 바로 씬 이동
            SceneChange(); 
        }
    }

    // =======================================================
    // 씬 이동 코루틴 (페이드 아웃 완료 후 씬 이동)
    // =======================================================
    IEnumerator FadeOutAndLoad()
    {
        if (fadePanelCanvasGroup == null) yield break;

        fadePanelCanvasGroup.blocksRaycasts = true; // 페이드 중 클릭 방지
        float timer = 0f;

        // 알파 값을 0(투명)에서 1(불투명, 검은색)로 서서히 증가시킵니다.
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeDuration);
            fadePanelCanvasGroup.alpha = alpha;
            yield return null;
        }

        // 확실하게 1로 설정 (완전한 검은 화면)
        fadePanelCanvasGroup.alpha = 1f;

        // 페이드 아웃이 완료된 후 씬 이동을 호출
        SceneChange(); 
    }


    // 씬 이동 함수 (이제 애니메이션 이벤트와는 독립적으로 호출됨)
    public void SceneChange()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("이동할 씬 이름(nextSceneName)이 설정되지 않았습니다!");
            return;
        }
        
        SceneManager.LoadScene(nextSceneName);
    }
}