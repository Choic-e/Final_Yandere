using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScreenController : MonoBehaviour
{
    [Header("UI")]
    public Toggle checkToggle;
    public Button startButton;
    public CanvasGroup startButtonCanvasGroup;

    [Header("Fade")]
    public Image fadePanel;
    public float fadeDuration = 1.2f;

    [Header("Sound")]
    public AudioSource sfx;

    [Header("Scene")]
    public string nextSceneName = "NextScene";

    private bool isPlaying = false;

    void Start()
    {
        startButton.interactable = false;
        startButtonCanvasGroup.alpha = 0.3f;

        checkToggle.onValueChanged.AddListener(OnToggleChanged);

        startButton.onClick.AddListener(OnStartButtonClicked);

        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            c.a = 0f;
            fadePanel.color = c;
        }
    }

    void OnToggleChanged(bool isOn)
    {
        startButton.interactable = isOn;
        startButtonCanvasGroup.alpha = isOn ? 1f : 0.3f;
    }

    void OnStartButtonClicked()
    {
        if (isPlaying) return;
        isPlaying = true;

        if (sfx != null) sfx.Play();

        startButton.interactable = false;

    
        StartCoroutine(FadeAndLoad());
    }

    System.Collections.IEnumerator FadeAndLoad()
    {
        float t = 0f;
        Color c = fadePanel.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, t / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
