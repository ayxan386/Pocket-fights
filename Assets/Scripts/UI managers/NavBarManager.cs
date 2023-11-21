using System.Collections.Generic;
using UnityEngine;

public class NavBarManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> tabs;
    [SerializeField] private Animator pauseMenuAnimator;
    public bool isShopOpen = false;

    private void Start()
    {
        EventManager.OnPauseMenuToggled += OnPauseMenuToggled;
        tabs.Find(tab => tab.name == "Shop").SetActive(false);
        OpenTab("Inventory");
    }

    private void OnPauseMenuToggled(bool isPaused)
    {
        pauseMenuAnimator.SetTrigger(isPaused ? "open" : "close");
        isShopOpen = isShopOpen && isPaused;
    }

    public void OpenTab(string tabName)
    {
        tabs.ForEach(tab => tab.SetActive(false));
        tabs.Find(tab => tab.name == tabName).SetActive(true);
        switch (tabName)
        {
            case "Shop":
                tabs.Find(tab => tab.name == "Inventory").SetActive(true);
                if (!isShopOpen)
                {
                    pauseMenuAnimator.SetTrigger("shopOpen");
                    EventManager.OnShopOpened?.Invoke(true);
                }

                isShopOpen = true;
                break;
            default:
                if (isShopOpen)
                    pauseMenuAnimator.SetTrigger("shopClose");
                isShopOpen = false;
                break;
        }
    }
}