using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerActionManager : MonoBehaviour
{
    [SerializeField] private Transform skillHolder;
    [SerializeField] private List<Skill> actions;

    public List<Skill> AllSkills => actions.FindAll(action => action.type != ActionType.ReceiveAttack);

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
}