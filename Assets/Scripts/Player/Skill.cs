using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Skill : MonoBehaviour
{
    public string animationName;
    public float multiplier => effects[currentLevel];

    public float manaConsumption;
    public ActionType type;
    public SkillDisplayDetails displayDetails;
    public List<float> effects;
    public int currentLevel;
    public int maxLevel;
    public string slotName;
    public bool isSelected;

    public string Description => $"{displayDetails.descriptionBase} \n Mana cost: {manaConsumption}" +
                                 $"\n Current effect {multiplier}"
                                 + (CanUpgrade ? $" \n Upgrade cost: x{UpgradeCost}" : "Max LVL");

    public int UpgradeCost => currentLevel + 1;

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