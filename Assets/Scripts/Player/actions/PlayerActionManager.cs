using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerActionManager : MonoBehaviour
{
    [SerializeField] private Transform skillHolder;
    [SerializeField] private List<Skill> actions;
    public List<Skill> AllSkills => actions.FindAll(action => action.type != ActionType.ReceiveAttack);

    public SkillSaveDataWrapper SaveData => new SkillSaveDataWrapper(AllSkills);

    public static PlayerActionManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        actions = new List<Skill>();
    }

    private void Start()
    {
        actions = skillHolder.GetComponentsInChildren<Skill>().ToList();
    }

    public void UpgradeSkill(SkillCellManager clickedCell)
    {
        var stat = PlayerInputController.Instance.Stats;
        var targetSkill = clickedCell.RelatedSkill;
        if (targetSkill.CanUpgrade && stat.SkillPoints >= targetSkill.UpgradeCost)
        {
            stat.UseSkillPoints(targetSkill.UpgradeCost);
            targetSkill.Upgrade();

            if (targetSkill.type == ActionType.Passive)
            {
                targetSkill.usageEffects?.Invoke(targetSkill, PlayerInputController.Instance.Stats, null);
            }

            EventManager.OnSkillUpgraded?.Invoke(clickedCell);
        }
    }

    public Skill GetAction(int actionIndex)
    {
        foreach (var skill in actions)
        {
            if (skill.type != ActionType.ReceiveAttack
                && skill.isSelected
                && skill.slotName == actionIndex.ToString())
                return skill;
        }

        return null;
    }

    public void LoadData(SkillSaveDataWrapper data)
    {
        foreach (var skillSaveData in data.skills)
        {
            var skill = actions.Find(action => action.name == skillSaveData.id);
            if (skill == null) continue;
            print("Found relevant skill");
            skill.currentLevel = skillSaveData.level;
            if (skillSaveData.isSelected)
            {
                skill.slotName = skillSaveData.slotName;
                skill.isSelected = true;
            }
        }

        EventManager.OnSkillDisplayUpdate?.Invoke(true);
    }
}

[Serializable]
public class SkillSaveData
{
    public string id;
    public int level;
    public string slotName;
    public bool isSelected;

    public SkillSaveData(string id, int level, bool inputIsSelected, string inputSlotName)
    {
        this.id = id;
        this.level = level;
        isSelected = inputIsSelected;
        this.slotName = isSelected ? inputSlotName : "E";
    }
}

[Serializable]
public class SkillSaveDataWrapper
{
    public List<SkillSaveData> skills;

    public SkillSaveDataWrapper(List<Skill> allSkills)
    {
        skills = allSkills.ConvertAll(skill =>
            new SkillSaveData(skill.name, skill.currentLevel, skill.isSelected, skill.slotName));
    }
}