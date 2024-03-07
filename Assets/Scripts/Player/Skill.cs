using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Skill : MonoBehaviour
{
    public float multiplier => effects[currentLevel];
    public float manaConsumption;
    public ActionType type;
    public DisplayDetails displayDetails;
    public List<float> effects;
    public List<int> upgradeCost;
    public int currentLevel;
    public int maxLevel;
    public string slotName;
    public bool isSelected;
    public bool canBeUsed;
    public UnityEvent<Skill, StatController, StatController> usageEffects;


    public string Description => IsActive
        ? ($"{displayDetails.descriptionBase} \n Mana cost: {manaConsumption}" +
           $"\n Current effect: {multiplier}"
           + (CanUpgrade ? $"-> {effects[currentLevel + 1]} {displayDetails.effectSuffix}" : "")
           + (CanUpgrade ? $" \n Upgrade cost: x{UpgradeCost}" : "\n Max LVL"))
        : $" {displayDetails.descriptionBase}\n Activation cost: x{ActivationPrice} \n Effect: x{effects[0]}";

    public bool IsActive => currentLevel >= 0;

    public int UpgradeCost => IsActive ? upgradeCost[currentLevel + 1] : ActivationPrice;
    public int ActivationPrice => upgradeCost[0];

    public bool CanUpgrade => currentLevel + 1 < maxLevel;

    public bool Lock { get; set; }

    public void Upgrade()
    {
        if (currentLevel + 1 < maxLevel)
        {
            currentLevel++;
        }
    }

    public void Disable()
    {
        canBeUsed = false;
        EventManager.OnPlayerVictory += OnPlayerVictory;

        void OnPlayerVictory(bool obj)
        {
            canBeUsed = true;
            EventManager.OnPlayerVictory -= OnPlayerVictory;
        }
    }
}

[Serializable]
public class DisplayDetails
{
    public Sprite icon;
    public string displayName;
    public string descriptionBase;
    public string effectSuffix = "";
}

[Serializable]
public enum ActionType
{
    Attack,
    Passive,
    ReceiveAttack,
    Defense,
    Buff
}