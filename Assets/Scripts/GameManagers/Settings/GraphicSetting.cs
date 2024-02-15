using TMPro;
using UnityEngine;

public class GraphicSetting : GameSetting
{
    [SerializeField] private TMP_Dropdown selectionRef;
    [SerializeField] private string keyName;
    [SerializeField] private int defaultValue;

    void Start()
    {
        DefaultUpdate();
        selectionRef.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(int newVal)
    {
        PlayerPrefs.SetInt(keyName, newVal);
        QualitySettings.SetQualityLevel(newVal);
    }


    public override void DefaultUpdate()
    {
        var qualityLevel = PlayerPrefs.GetInt(keyName, defaultValue);
        QualitySettings.SetQualityLevel(qualityLevel);
        selectionRef.value = qualityLevel;
    }
}