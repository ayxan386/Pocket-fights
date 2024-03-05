using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptionManager : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI desc;

    [SerializeField] private DescriptionType type = DescriptionType.Inventory;

    public static ItemDescriptionManager InventoryInstance { get; private set; }
    public static ItemDescriptionManager SkillInstance { get; private set; }

    private void Awake()
    {
        switch (type)
        {
            case DescriptionType.Inventory:
                InventoryInstance = this;
                break;
            case DescriptionType.Skill:
                SkillInstance = this;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void DisplayItem(InventoryItem item)
    {
        if (item == null) return;
        icon.sprite = item.icon;
        title.text = item.name;
        desc.text = item.GetDescription();
    }

    public void DisplaySkill(Skill skill)
    {
        if (skill == null) return;
        icon.sprite = skill.displayDetails.icon;
        title.text = skill.displayDetails.displayName;
        desc.text = skill.Description;
    }

    public void DisplayStatusEffect(StatEffect statEffect)
    {
        if (statEffect == null) return;
        icon.sprite = statEffect.displayDetails.icon;
        title.text =
            $"{statEffect.displayDetails.displayName} ({(statEffect.isDamageBased ? statEffect.DamageBuffer : statEffect.numberOfTurns):N0})";
        desc.text = statEffect.GetDescription();
    }

    public enum DescriptionType
    {
        Inventory,
        Skill
    }
}