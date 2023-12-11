using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsDisplayManager : MonoBehaviour
{
    [SerializeField] private Transform skillCellHolder;
    [SerializeField] private SkillCellManager skillCellManagerPrefab;
    [SerializeField] private ItemDescriptionManager description;

    private List<SkillCellManager> cellManagers;

    private IEnumerator Start()
    {
        print("waiting");
        yield return new WaitUntil(() => PlayerActionManager.Instance != null
                                         && PlayerActionManager.Instance.AllSkills.Count > 0);
        print("waiting done");
        FillSkillCells();

        EventManager.OnSkillCellSelected += OnSkillCellSelected;
        EventManager.OnSkillUpgraded += OnSkillUpgraded;
    }

    private void OnSkillUpgraded(SkillCellManager toUpdate)
    {
        toUpdate.Display(toUpdate.RelatedSkill);
        description.DisplaySkill(toUpdate.RelatedSkill);
    }

    private void OnSkillCellSelected(SkillCellManager selectedCell, bool isSelected)
    {
        description.gameObject.SetActive(isSelected);

        if (!isSelected) return;

        description.transform.position = selectedCell.transform.position;
        description.DisplaySkill(selectedCell.RelatedSkill);
    }

    private void FillSkillCells()
    {
        var allSkills = PlayerActionManager.Instance.AllSkills;
        print($"Creating skills {allSkills.Count}");

        cellManagers = new List<SkillCellManager>();
        foreach (var skill in allSkills)
        {
            var cellManager = Instantiate(skillCellManagerPrefab, skillCellHolder);
            cellManager.Display(skill);
            cellManagers.Add(cellManager);
        }
    }
}