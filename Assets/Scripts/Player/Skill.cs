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

    public string Description => $"{displayDetails.descriptionBase} \n Mana cost: {manaConsumption}" +
                                 $"\n Current effect {multiplier}"
                                 + (currentLevel < maxLevel ? $" \n Upgrade cost: x{currentLevel + 1}" : "");

    public bool Upgrade()
    {
        if (currentLevel < maxLevel)
        {
            currentLevel++;
            return true;
        }

        return false;
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