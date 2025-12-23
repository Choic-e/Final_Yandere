using UnityEngine;
using UnityEngine.UI;
using Fungus;

public class ParameterBarUpdater : MonoBehaviour
{
    public Flowchart flowchart;
    public string variableName;
    public Slider slider;

    void Start()
    {
        slider.minValue = 0f;
        slider.maxValue = 100f;
    }

    void Update()
    {
        slider.value = flowchart.GetFloatVariable(variableName);
    }
}
