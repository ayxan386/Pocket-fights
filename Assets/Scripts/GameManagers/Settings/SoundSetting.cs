using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSetting : GameSetting
{
    [SerializeField] private string keyName;
    [SerializeField] private Slider sliderRef;
    [SerializeField] private float defaultValue = 0.4f;
    [Space(5)] [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private string groupName;

    private void Start()
    {
        DefaultUpdate();
    }

    private void OnDestroy()
    {
        sliderRef.onValueChanged.RemoveListener(OnValueChange);
    }

    private void OnValueChange(float newVal)
    {
        sliderRef.value = newVal;
        mainMixer.SetFloat(groupName, ConvertToDb(newVal));
        PlayerPrefs.SetFloat(keyName, newVal);
    }


    private float ConvertToDb(float volume)
    {
        return Mathf.Log10(volume) * 20f;
    }

    public override void DefaultUpdate()
    {
        var currentVolume = PlayerPrefs.GetFloat(keyName, defaultValue);
        mainMixer.SetFloat(groupName, ConvertToDb(currentVolume));

        sliderRef.minValue = 0.0001f;
        sliderRef.maxValue = 1f;
        sliderRef.value = currentVolume;
        sliderRef.onValueChanged.AddListener(OnValueChange);
    }
}