using System.Collections;
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
    public int id;
    public InventoryItem storedItem;
    private Coroutine clickCoro;

    private void Start()
    {
        if (storedItem == null)
            SetNoItemState();
    }

    public void SetNoItemState()
    {
        storedItem = null;
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
        storedItem = item;
        var itemIconColor = itemIcon.color;
        itemIconColor.a = 1;
        itemIcon.color = itemIconColor;
        itemIcon.sprite = item.icon;
        countText.text = "x" + item.count;
        countText.alpha = 1;
    }

    public void SetId(int newId)
    {
        id = newId;
    }

    public void ClickStarted()
    {
        if (clickCoro == null)
        {
            SendClick();
            clickCoro = StartCoroutine(ClickRegular());
        }
    }

    public void ClickStopped()
    {
        if (clickCoro != null)
        {
            StopCoroutine(clickCoro);
            clickCoro = null;
        }
    }

    private IEnumerator ClickRegular()
    {
        while (storedItem != null)
        {
            yield return new WaitForSeconds(0.3f);
            SendClick();
        }

        clickCoro = null;
    }

    private void SendClick()
    {
        if (storedItem != null)
        {
            InventoryController.Instance.ItemCellClicked(storedItem);
        }
    }
}