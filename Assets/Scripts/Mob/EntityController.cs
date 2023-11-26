using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EntityController : MonoBehaviour
{
    [SerializeField] private UnityEvent onInteractedSequence;

    [Header("Looting action")] [SerializeField]
    private Animator lootPanelPrefab;

    [SerializeField] private LootItemPanel lootItemPanelPrefab;
    [SerializeField] private List<PossibleLoot> possibleLoots;


    public void Interact()
    {
        onInteractedSequence?.Invoke();
    }

    public void GiveLoot()
    {
        var generatedLoot = LootManager.GenerateLoot(new List<List<PossibleLoot>> { possibleLoots });

        var canvas = GameObject.FindWithTag("MainCanvas");

        var lootPanel = Instantiate(lootPanelPrefab, canvas.transform);
        var lootHolder = lootPanel.GetComponentInChildren<VerticalLayoutGroup>().transform;

        Destroy(lootPanel, 360);
        foreach (var valueTuple in generatedLoot)
        {
            var newDrop = Instantiate(valueTuple.Item1);
            newDrop.count = valueTuple.Item2;
            var lootItemPanel = Instantiate(lootItemPanelPrefab, lootHolder);
            lootItemPanel.UpdateDisplay(newDrop);
        }
        lootPanel.SetTrigger("open");
    }
}