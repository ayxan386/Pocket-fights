using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCell : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private GameObject descGm;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image innerPart;
    [SerializeField] private Color[] innerColors;

    private void Start()
    {
        SetNoItemState();
    }

    public void SetNoItemState()
    {
        var itemIconColor = itemIcon.color;
        itemIconColor.a = 0;
        itemIcon.color = itemIconColor;
        descGm.SetActive(false);
        countText.alpha = 0;
    }

    [ContextMenu("Set color")]
    public void SetColor()
    {
        if (name.Contains("("))
        {
            var substring = name.Substring("InventoryCell (".Length);
            var index = (int.Parse(substring.Remove(substring.Length - 1, 1)) % 9 / 3);
            innerPart.color = innerColors[index];
        }
    }

    public void UpdateDisplay(InventoryItem item)
    {
        var itemIconColor = itemIcon.color;
        itemIconColor.a = 1;
        itemIcon.color = itemIconColor;
        itemIcon.sprite = item.icon;
        countText.text = "x" + item.count;
        // descGm.SetActive(false);
        countText.alpha = 1;
    }
}