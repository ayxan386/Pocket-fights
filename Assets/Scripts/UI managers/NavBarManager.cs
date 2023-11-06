using System.Collections.Generic;
using UnityEngine;

public class NavBarManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> tabs;

    public void OpenTab(string tabName)
    {
        tabs.ForEach(tab => tab.SetActive(false));
        tabs.Find(tab => tab.name == tabName).SetActive(true);
    }

    private void OnEnable()
    {
        InventoryController.Instance.UpdateDisplay();
    }
}