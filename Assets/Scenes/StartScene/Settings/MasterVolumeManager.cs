using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MasterVolumeManager : MonoBehaviour
{
    public static MasterVolumeManager Instance;

    public AudioMixer audioMixer;
    public Slider volumeSlider;

    const string VOLUME_KEY = "MasterVolume";

    void Awake()
    {
        // 싱글톤 처리: 씬 이동해도 파괴되지 않음
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 1f);

        SetVolume(savedVolume);

        if (volumeSlider != null)
            volumeSlider.value = savedVolume;

        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float value)
    {
        // AudioMixer는 dB 단위 사용하므로 변환 필요
        float dB = Mathf.Log10(value <= 0 ? 0.0001f : value) * 20f;

        audioMixer.SetFloat("Volume", dB);

        PlayerPrefs.SetFloat(VOLUME_KEY, value);
        
        bool success = audioMixer.SetFloat("Volume", dB);
        Debug.Log("Mixer 적용됨? " + success + " / dB=" + dB);


    }
}
