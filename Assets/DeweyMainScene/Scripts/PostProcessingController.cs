using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoBehaviour
{
    [Header("Volume Reference")]
    public Volume globalVolume;

    private ChromaticAberration aberration;
    private Bloom bloom;
    private ColorAdjustments colorAdj;
    private FilmGrain filmGrain;
    private ShadowsMidtonesHighlights smh;

    private float baseAberration;
    private float baseBloomIntensity;
    private Color baseColorFilter;
    private float baseGrainIntensity;
    private Vector4 baseShadows; // 그림자 밝기

    void Start()
    {
        if (globalVolume.profile.TryGet(out aberration)) 
            baseAberration = aberration.intensity.value;

        if (globalVolume.profile.TryGet(out bloom)) 
            baseBloomIntensity = bloom.intensity.value;

        if (globalVolume.profile.TryGet(out colorAdj)) 
            baseColorFilter = colorAdj.colorFilter.value;

        if (globalVolume.profile.TryGet(out filmGrain)) 
            baseGrainIntensity = filmGrain.intensity.value;

        if (globalVolume.profile.TryGet(out smh)) 
            baseShadows = smh.shadows.value;
    }

    // MP 구간에 따라 시각 효과 업데이트
    public void UpdateVisualsByMP(int currentMP)
    {
        // MP 구간 결정 (0: 안전, 1: 주의, 2: 위험, 3: 치명적)
        int stressLevel = 0;
        if (currentMP >= 75) stressLevel = 0;      // 100 ~ 75
        else if (currentMP >= 50) stressLevel = 1; // 74 ~ 50
        else if (currentMP >= 25) stressLevel = 2; // 49 ~ 25
        else stressLevel = 3;                      // 24 ~ 0

        ApplyEffects(stressLevel);
    }

    void ApplyEffects(int level)
    {
        float addAberration = 0f;
        float addBloom = 0f;
        float addGrain = 0f;
        Color targetColorTint = baseColorFilter; // 기본 색
        Vector4 targetShadows = baseShadows;

        switch (level)
        {
            case 0: // 평화 (변화 없음)
                break;
            case 1: // 약간 불안
                addAberration = 0.2f; // 살짝 지직
                addGrain = 0.2f;      // 살짝 노이즈
                targetColorTint = Color.Lerp(baseColorFilter, new Color(1f, 0.9f, 0.9f), 0.3f); // 아주 살짝 붉음
                break;
            case 2: // 위험
                addAberration = 0.4f;
                addBloom = 1.0f;      // 눈부심 추가
                addGrain = 0.5f;
                targetColorTint = Color.Lerp(baseColorFilter, Color.red, 0.15f); // 붉은끼 돔
                // 그림자를 붉고 어둡게 (Vector4: R, G, B, Weight)
                targetShadows = new Vector4(0.5f, 0.2f, 0.2f, baseShadows.w); 
                break;
            case 3: // 멘붕 (MP 24 이하)
                addAberration = 1.0f; // 최대 왜곡
                addBloom = 1.0f;      // 몽환적/현기증
                addGrain = 1.0f;      // 자글자글
                targetColorTint = Color.Lerp(baseColorFilter, Color.red, 0.4f); // 화면이 피범벅 느낌
                targetShadows = new Vector4(0.2f, 0f, 0f, baseShadows.w); // 그림자가 아주 시꺼멓고 붉음
                break;
        }

        // --- 값 적용 (기본값 + 추가값) ---
        
        if (aberration != null) 
            aberration.intensity.value = baseAberration + addAberration;

        if (bloom != null) 
            bloom.intensity.value = baseBloomIntensity + addBloom;

        if (filmGrain != null) 
            filmGrain.intensity.value = baseGrainIntensity + addGrain;

        if (colorAdj != null) 
            colorAdj.colorFilter.value = targetColorTint;

        if (smh != null)
            smh.shadows.value = targetShadows;
    }
}