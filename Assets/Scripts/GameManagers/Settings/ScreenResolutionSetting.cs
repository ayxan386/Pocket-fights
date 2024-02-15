using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenResolutionSetting : GameSetting
{
    [SerializeField] private string resolutionKeyName;
    [SerializeField] private string resolutionDefaultValue;
    [SerializeField] private TMP_Dropdown resolutionSelectionRef;

    [SerializeField] private string fullscreenKeyName;
    [SerializeField] private bool fullscreenDefaultValue;
    [SerializeField] private Toggle fullscreenToggle;

    private void Start()
    {
        DefaultUpdate();
        resolutionSelectionRef.onValueChanged.AddListener(OnResolutionChanged);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
    }

    private void OnResolutionChanged(int newVal)
    {
        var resolution = resolutionSelectionRef.options[newVal].text;
        PlayerPrefs.SetString(resolutionKeyName, resolution);
    }

    public override void DefaultUpdate()
    {
        var currentSelection = PlayerPrefs.GetString(resolutionKeyName, resolutionDefaultValue);
        var resolutions = Screen.resolutions;
        resolutionSelectionRef.ClearOptions();
        var allOptions = new List<TMP_Dropdown.OptionData>();

        var currentSelectionIndex = 0;
        foreach (var resolution in resolutions)
        {
            var optionData = new TMP_Dropdown.OptionData($"{resolution.width}x{resolution.height}");
            allOptions.Add(optionData);

            if (optionData.text != currentSelection)
            {
                currentSelectionIndex++;
            }
        }

        allOptions.Reverse();
        currentSelectionIndex = allOptions.Count - currentSelectionIndex - 1;
        resolutionSelectionRef.AddOptions(allOptions);
        resolutionSelectionRef.value = currentSelectionIndex;

        var isFullScreen = PlayerPrefs.GetInt(fullscreenKeyName, fullscreenDefaultValue ? 1 : 0) == 1;
        fullscreenToggle.isOn = isFullScreen;

        UpdateScreen();
    }

    private void OnFullscreenToggled(bool isOn)
    {
        PlayerPrefs.SetInt(fullscreenKeyName, isOn ? 1 : 0);
        UpdateScreen();
    }

    private void UpdateScreen()
    {
        var isFullScreen = PlayerPrefs.GetInt(fullscreenKeyName, fullscreenDefaultValue ? 1 : 0) == 1;
        var resolutionJoined = PlayerPrefs.GetString(resolutionKeyName, resolutionDefaultValue).Split("x");

        Screen.SetResolution(int.Parse(resolutionJoined[0]),
            int.Parse(resolutionJoined[1]),
            isFullScreen);
    }
}