using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatusEffectDisplayManager : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI remainingText;
    [SerializeField] private Color positiveEffectColor;
    [SerializeField] private Color negativeEffectColor;

    private StatEffect lastEffect;

    public Color TextColor { get; set; } = Color.black;

    public void UpdateDisplay(StatEffect statEffect)
    {
        if (statEffect == null) return;
        lastEffect = statEffect;
        backgroundImage.color = statEffect.isPositive ? positiveEffectColor : negativeEffectColor;

        remainingText.color = TextColor;
        remainingText.text =
            "" + (statEffect.isDamageBased ? statEffect.DamageBuffer : statEffect.numberOfTurns).ToString("N0");
        iconImage.sprite = statEffect.displayDetails.icon;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (lastEffect == null) return;
        print($"Hovering over {lastEffect.displayDetails.displayName}");
        ItemDescriptionManager.InventoryInstance.DisplayStatusEffect(lastEffect);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (lastEffect == null) return;
        ItemDescriptionManager.InventoryInstance.DisplayStatusEffect(lastEffect);
    }
}