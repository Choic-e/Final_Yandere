using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Stat Sliders")]
    public Slider mpSlider;
    public Slider loveSlider;
    public Slider obsessionSlider;
    public Slider angerSlider;

    [Header("Turn Text")]
    public TextMeshProUGUI turnText;

    [Header("Timing Visuals")]
    public Slider timingSlider;
    public Image timingFillImage;
    public Color colorFast = Color.red;          // 0 ~ 3초
    public Color colorGood = Color.green;        // 3.1 ~ 6.5초
    public Color colorSlow = new Color(0.5f, 0, 0.5f); // 6.6 ~ 10초

    [Header("Player Portrait Phases")]
    public Image playerPortraitImage; 
    
    public Sprite portraitPhase1; // MP 100 ~ 76
    public Sprite portraitPhase2; // MP 75 ~ 50
    public Sprite portraitPhase3; // MP 49 ~ 26
    public Sprite portraitPhase4; // MP 25 ~ 11
    public Sprite portraitPhase5; // MP 10 ~ 0 


    [Header("UI Audio")]
    public AudioSource uiAudioSource; 
    
    public void PlaySound(AudioClip clip)
    {
        if (uiAudioSource != null && clip != null)
        {
            uiAudioSource.PlayOneShot(clip);
        }
    }
    
    public void InitUI(int maxMP, int maxTurn)
    {
        if (mpSlider != null) mpSlider.maxValue = maxMP;
        if (loveSlider != null) loveSlider.maxValue = 100;
        if (obsessionSlider != null) obsessionSlider.maxValue = 100;
        if (angerSlider != null) angerSlider.maxValue = 100;
        
        if (timingSlider != null) timingSlider.gameObject.SetActive(false);
    }

    // 모든 스탯 UI 갱신 (GameManage
    public void UpdateAllStats(int turn, int mp, int maxMP, int love, int obsession, int anger)
    {
        if (mpSlider != null) mpSlider.value = mp;
        if (loveSlider != null) loveSlider.value = love;
        if (obsessionSlider != null) obsessionSlider.value = obsession;
        if (angerSlider != null) angerSlider.value = anger;

        // 턴 텍스트 
        if (turnText != null) turnText.text = turn.ToString("D2");

        // 포트레잇 변경 (MP 값만 넘겨줌)
        UpdatePortrait(mp);
    }

    private void UpdatePortrait(int currentMP)
    {
        if (playerPortraitImage == null) return;

        if (currentMP >= 76) 
        {
            // [1단계] 100 ~ 76
            if (portraitPhase1 != null) playerPortraitImage.sprite = portraitPhase1;
        }
        else if (currentMP >= 50) 
        {
            // [2단계] 75 ~ 50
            if (portraitPhase2 != null) playerPortraitImage.sprite = portraitPhase2;
        }
        else if (currentMP >= 26) 
        {
            // [3단계] 49 ~ 26
            if (portraitPhase3 != null) playerPortraitImage.sprite = portraitPhase3;
        }
        else if (currentMP >= 11) 
        {
            // [4단계] 25 ~ 11
            if (portraitPhase4 != null) playerPortraitImage.sprite = portraitPhase4;
        }
        else 
        {
            // [5단계] 10 ~ 0
            if (portraitPhase5 != null) playerPortraitImage.sprite = portraitPhase5;
        }
    }

    // 타이밍 슬라이더 켜기/끄기
    public void SetTimingSliderActive(bool isActive)
    {
        if (timingSlider != null)
        {
            timingSlider.gameObject.SetActive(isActive);
            if (isActive) timingSlider.value = 0;
        }
    }

    public void UpdateTimingSlider(float elapsed, float fastLimit, float slowStartLimit, float timeoutLimit)
    {
        if (timingSlider == null) return;

        // 1. 슬라이더의 전체 길이는 '타임아웃(10초)' 기준
        timingSlider.maxValue = timeoutLimit; 
        timingSlider.value = elapsed;

        if (timingFillImage != null)
        {
            // A. 빠름 구간 (0 ~ fastLimit) -> 빨강
            if (elapsed < fastLimit) 
            {
                timingFillImage.color = colorFast;
            }
            // B. 느림 구간 (slowStartLimit ~ timeoutLimit) -> 보라
            //    (중요: elapsed가 slowStartLimit보다 크면 무조건 보라색)
            else if (elapsed > slowStartLimit)
            {
                timingFillImage.color = colorSlow;
            }
            // C. 좋음 구간 (그 사이) -> 초록
            else 
            {
                timingFillImage.color = colorGood;
            }
        }
    }
}