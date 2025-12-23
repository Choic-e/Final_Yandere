using UnityEngine;
using UnityEngine.UI;

public class ToggleSound : MonoBehaviour
{
    public Toggle checkToggle;
    public AudioSource audioSource;

    void Start()
    {
        checkToggle.onValueChanged.AddListener(PlaySound);
    }

    void PlaySound(bool isOn)
    {
        audioSource.Play();
    }
}
