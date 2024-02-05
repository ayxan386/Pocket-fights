using UnityEngine;

public class ConsumablesSelection : MonoBehaviour
{
    [SerializeField] private InventoryCell cellPrefab;
    [SerializeField] private Transform cellHolder;

    public void ShowSelection()
    {
        var inventoryData = InventoryController.Instance.OwnedItems;

        var consumables = inventoryData.FindAll(item => item.type == ItemType.Consumable);
    }
}