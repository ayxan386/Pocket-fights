using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MobDescriptionManager : MonoBehaviour
{
    [SerializeField] private GameObject menuRef;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Transform dropList;
    public static MobDescriptionManager Instance { get; private set; }

    private Image[] dropListItems;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        dropListItems = dropList.GetComponentsInChildren<Image>();
        foreach (var dropListItem in dropListItems)
        {
            dropListItem.gameObject.SetActive(false);
        }
    }

    public void TurnOff()
    {
        menuRef.SetActive(false);
    }

    public void DisplayMob(MobController mobController)
    {
        menuRef.SetActive(true);
        icon.sprite = mobController.DisplayData.icon;
        title.text = mobController.DisplayData.displayName + $" LVL: {mobController.Stats.Level}";

        var k = 0;

        var loots = mobController.PossibleLoots.DistinctBy(loot => loot.itemPrefab.name);
        foreach (var loot in loots)
        {
            dropListItems[k].gameObject.SetActive(true);
            dropListItems[k++].sprite = loot.itemPrefab.icon;
        }

        for (int i = k; i < dropListItems.Length; i++)
        {
            dropListItems[i].gameObject.SetActive(false);
        }
    }

    public void DisplayGeneric(string passedTitle, Sprite passedIcon, List<Sprite> smallIcons)
    {
        menuRef.SetActive(true);
        title.text = passedTitle;
        icon.sprite = passedIcon;

        foreach (var item in dropListItems)
        {
            item.gameObject.SetActive(false);
        }

        for (var i = 0; i < smallIcons.Count; i++)
        {
            dropListItems[i].gameObject.SetActive(true);
            dropListItems[i].sprite = smallIcons[i];
        }
    }
}