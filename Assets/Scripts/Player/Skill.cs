using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Skill : MonoBehaviour
{
    public string animationName;
    public float multiplier => effects[currentLevel];

    public float manaConsumption;
    public ActionType type;
    public SkillDisplayDetails displayDetails;
    public List<float> effects;
    public int activationPrice;
    public int currentLevel;
    public int maxLevel;
    public string slotName;
    public bool isSelected;
    public UnityEvent<Skill, StatController, StatController> usageEffects;

    public string Description => $"{displayDetails.descriptionBase} \n Mana cost: {manaConsumption}" +
                                 $"\n Current effect: {multiplier}"
                                 + (CanUpgrade ? $"-> {effects[currentLevel + 1]}" : "")
                                 + (CanUpgrade ? $" \n Upgrade cost: x{UpgradeCost}" : "\n Max LVL");

    public bool IsActive => currentLevel > 0;

    public int UpgradeCost => IsActive ? (currentLevel + 1) : activationPrice;

    public bool CanUpgrade => currentLevel + 1 < maxLevel;

    public void Upgrade()
    {
        if (currentLevel + 1 < maxLevel) currentLevel++;
    }
}

[Serializable]
public class SkillDisplayDetails
{
    public Sprite icon;
    public string displayName;
    public string descriptionBase;
}

[Serializable]
public enum ActionType
{
    Attack,
    ReceiveAttack
}