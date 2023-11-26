using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptionManager : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI desc;

    public static ItemDescriptionManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void DisplayItem(InventoryItem item)
    {
        if (item == null) return;
        icon.sprite = item.icon;
        title.text = item.name;
        desc.text = item.description;
    }
}