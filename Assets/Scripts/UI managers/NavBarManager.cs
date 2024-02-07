using System.Collections.Generic;
using UnityEngine;

public class NavBarManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> tabs;
    [SerializeField] private Animator pauseMenuAnimator;
    [SerializeField] private Transform questHolder;

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
        if (PlayerInputController.Instance.isPaused == isPaused) return;
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
            case "Inventory":
                InventoryController.Instance.UpdateDisplay();
                DefaultPostBehavior();
                break;
            case "InfoPanel":
                EventManager.OnBaseStatUpdate?.Invoke(0);
                DefaultPostBehavior();
                break;
            case "Skills":
                EventManager.OnSkillDisplayUpdate?.Invoke(true);
                DefaultPostBehavior();
                break;
            case "Quests":
                QuestManager.Instance.OpenUi(questHolder, true);
                DefaultPostBehavior();
                break;
            default:
                DefaultPostBehavior();
                break;
        }
    }

    private void DefaultPostBehavior()
    {
        if (ShopManager.Instance.IsShopOpen)
            pauseMenuAnimator.SetTrigger("shopClose");
        EventManager.OnShopToggled?.Invoke(false);
    }
}