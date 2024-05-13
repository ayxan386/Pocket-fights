using System.Collections.Generic;
using UnityEngine;

public class DetailDisplayManager : MonoBehaviour
{
    [SerializeField] private List<MobDescriptionDisplay> displays;

    private int lastIndex;
    public static DetailDisplayManager Instance { get; private set; }

    private HashSet<MobController> displayedMobs;
    private HashSet<string> displaysGeneric;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EventManager.OnUiReduced += OnUiReduced;
    }

    private void OnDestroy()
    {
        EventManager.OnUiReduced -= OnUiReduced;
    }

    private void OnUiReduced(bool turnOff)
    {
        foreach (var display in displays)
        {
            display.gameObject.SetActive(!turnOff);
        }
    }

    public void TurnOff()
    {
        lastIndex = 0;

        foreach (var display in displays)
        {
            display.TurnOff();
        }
    }

    public void DisplayNextMob(MobController mob)
    {
        displays[lastIndex++].DisplayMob(mob);
        if (lastIndex >= displays.Count) lastIndex--;
    }

    public void DisplayGeneric(string passedTitle, Sprite passedIcon, List<Sprite> smallIcons)
    {
        displays[lastIndex++].DisplayGeneric(passedTitle, passedIcon, smallIcons);
        if (lastIndex >= displays.Count) lastIndex--;
    }
}