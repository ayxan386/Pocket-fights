using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class NavBarManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> tabs;
    [SerializeField] private Animator pauseMenuAnimator;
    [SerializeField] private Transform questHolder;

    public static NavBarManager Instance { get; private set; }
    private bool lastPauseState;
    private string lastTabName;

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
        if (lastPauseState == isPaused) return;
        lastPauseState = isPaused;
        pauseMenuAnimator.SetTrigger(isPaused ? "open" : "close");

        if (isPaused && lastTabName != null)
        {
            UpdateTabContent(lastTabName == "Shop" ? "Inventory" : lastTabName);
            SelectTab();
        }
    }

    public void SelectTab(string tabName = "Inventory")
    {
        for (int index = 0; index < transform.childCount; index++)
        {
            var child = transform.GetChild(index);
            if (child.gameObject.name == tabName)
            {
                EventSystem.current.SetSelectedGameObject(child.gameObject);
            }
        }
    }

    public void OpenTab(string tabName)
    {
        var nextTab = tabs.Find(tab => tab.name == tabName);
        if (nextTab)
        {
            tabs.ForEach(tab => tab.SetActive(false));
            nextTab.SetActive(true);
        }

        lastTabName = tabName;

        UpdateTabContent(tabName);
        DefaultPostBehavior();
    }

    private void UpdateTabContent(string tabName)
    {
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
                DataManager.Instance.SaveEventTrigger("Navbar");
                Invoke(nameof(LoadMainMenu), 1.5f);
                break;
            case "Inventory":
                InventoryController.Instance.UpdateDisplay();
                break;
            case "InfoPanel":
                EventManager.OnBaseStatUpdate?.Invoke(0);
                break;
            case "Skills":
                EventManager.OnSkillDisplayUpdate?.Invoke(true);
                break;
            case "Quests":
                QuestManager.Instance.OpenUi(questHolder, true);
                break;
        }
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void DefaultPostBehavior()
    {
        if (lastTabName is "Shop" or "Save") return;
        var stateCopy = ShopManager.Instance.IsShopOpen;
        if (stateCopy)
            pauseMenuAnimator.SetTrigger("shopClose");
        EventManager.OnShopToggled?.Invoke(false);
    }
}