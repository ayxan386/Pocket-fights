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
        if (clickedCell.RelatedSkill.CanUpgrade
            && stat.SkillPoints >= clickedCell.RelatedSkill.UpgradeCost)
        {
            stat.UseSkillPoints(clickedCell.RelatedSkill.UpgradeCost);
            clickedCell.RelatedSkill.Upgrade();
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