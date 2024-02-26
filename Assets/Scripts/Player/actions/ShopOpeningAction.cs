using System.Collections.Generic;
using UnityEngine;

public class ShopOpeningAction : MonoBehaviour
{
    [SerializeField] private ShopItemsData shopItemsData;
    [field: SerializeField] public string ShopName { get; set; }
    [field: SerializeField] public List<InventoryItem> StoredItems { get; private set; }
    public Dictionary<string, int> BougthItems { get; private set; }

    private bool canOpen = true;

    private void Start()
    {
        RefreshShop();
        EventManager.OnPauseMenuToggled += OnPauseMenuToggled;
    }

    private void OnDestroy()
    {
        EventManager.OnPauseMenuToggled -= OnPauseMenuToggled;
    }

    private void OnPauseMenuToggled(bool obj)
    {
        canOpen = !obj;
    }

    private void RefreshShop()
    {
        StoredItems = shopItemsData.GenerateShopData(transform);
        BougthItems = new Dictionary<string, int>();
        CheckBoughtItemCounts();
    }

    public void CheckBoughtItemCounts()
    {
        foreach (var inventoryItem in InventoryController.Instance.OwnedItems)
        {
            BougthItems.TryGetValue(inventoryItem.name, out var count);
            inventoryItem.sellPrice = (int)Mathf.Floor(inventoryItem.maxSellPrice *
                                                       Mathf.Pow(inventoryItem.priceDropRate,
                                                           count / inventoryItem.stackSize));
        }
    }

    public void OpenShop()
    {
        if (!canOpen) return;
        ShopManager.Instance.SetShopState(this);
        EventManager.OnPauseMenuToggled?.Invoke(true);
        NavBarManager.Instance.OpenTab("Shop");
    }
}