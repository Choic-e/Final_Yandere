using UnityEngine;
using UnityEngine.UI;

public class FadeInOnSceneStart : MonoBehaviour
{
    public Image fadePanel;
    public float fadeDuration = 1.2f;

    void Start()
    {
        // 검은 화면에서 시작
        Color c = fadePanel.color;
        c.a = 1f;
        fadePanel.color = c;

        StartCoroutine(FadeIn());
    }

    System.Collections.IEnumerator FadeIn()
    {
        float t = 0f;
        Color c = fadePanel.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1, 0, t / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }
    }
}
