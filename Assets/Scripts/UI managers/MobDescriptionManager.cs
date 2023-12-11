using TMPro;
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

        foreach (var loot in mobController.PossibleLoots)
        {
            dropListItems[k].gameObject.SetActive(true);
            dropListItems[k++].sprite = loot.itemPrefab.icon;
        }

        for (int i = k; i < dropListItems.Length; i++)
        {
            dropListItems[i].gameObject.SetActive(false);
        }
    }
}