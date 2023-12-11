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
}

[Serializable]
public enum ActionType
{
    Attack,
    ReceiveAttack
}