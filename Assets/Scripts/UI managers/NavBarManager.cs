using System.Collections.Generic;
using UnityEngine;

public class NavBarManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> tabs;
    [SerializeField] private Animator pauseMenuAnimator;

    public static NavBarManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EventManager.OnPauseMenuToggled += OnPauseMenuToggled;
        tabs.Find(tab => tab.name == "Shop").SetActive(false);
        OpenTab("Inventory");
    }

    private void OnDestroy()
    {
        EventManager.OnPauseMenuToggled -= OnPauseMenuToggled;
    }

    private void OnPauseMenuToggled(bool isPaused)
    {
        pauseMenuAnimator.SetTrigger(isPaused ? "open" : "close");
    }

    public void OpenTab(string tabName)
    {
        var nextTab = tabs.Find(tab => tab.name == tabName);
        if (nextTab)
        {
            tabs.ForEach(tab => tab.SetActive(false));
            nextTab.SetActive(true);
        }

        switch (tabName)
        {
            case "Shop":
                tabs.Find(tab => tab.name == "Inventory").SetActive(true);
                if (!ShopManager.Instance.IsShopOpen)
                {
                    pauseMenuAnimator.SetTrigger("shopOpen");
                    EventManager.OnShopToggled?.Invoke(true);
                }

                break;
            case "Save":
                DataManager.Instance.SaveEventTrigger();
                break;
            default:
                if (ShopManager.Instance.IsShopOpen)
                    pauseMenuAnimator.SetTrigger("shopClose");
                EventManager.OnShopToggled?.Invoke(false);
                break;
        }
    }
}