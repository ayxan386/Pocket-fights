using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillsDisplayManager : MonoBehaviour
{
    [SerializeField] private Transform skillCellHolder;
    [SerializeField] private SkillCellManager skillCellManagerPrefab;
    [SerializeField] private ItemDescriptionManager description;
    [SerializeField] private Transform skillOptions;
    [SerializeField] private List<SkillSelectionManager> skillSelectionDisplay;
    [SerializeField] private Button upgradeOption;
    [SerializeField] private Button[] equipOption;

    private List<SkillCellManager> cellManagers;
    private SkillCellManager lastClickedCell;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => PlayerActionManager.Instance != null
                                         && PlayerActionManager.Instance.AllSkills.Count > 0);
        FillSkillCells();

        EventManager.OnSkillCellSelected += OnSkillCellSelected;
        EventManager.OnSkillUpgraded += OnSkillUpgraded;
        EventManager.OnSkillCellClicked += OnSkillCellClicked;
    }

    public void OptionSelected(string option)
    {
        switch (option)
        {
            case "upgrade":
                PlayerActionManager.Instance.UpgradeSkill(lastClickedCell);
                break;
            default: //binding to specific slot
                if (lastClickedCell.RelatedSkill.type == ActionType.Passive) return;
                var equippedSkill =
                    PlayerActionManager.Instance.AllSkills.Find(skill => skill.isSelected && skill.slotName == option);
                if (equippedSkill != null)
                {
                    equippedSkill.isSelected = false;
                    equippedSkill.slotName = "E";
                }

                lastClickedCell.RelatedSkill.isSelected = true;
                lastClickedCell.RelatedSkill.slotName = option;
                foreach (var display in skillSelectionDisplay) display.UpdateUi();

                break;
        }

        skillOptions.gameObject.SetActive(false);
    }

    private void OnSkillCellClicked(SkillCellManager clickedCell)
    {
        lastClickedCell = clickedCell;
        upgradeOption.interactable = clickedCell.RelatedSkill.CanUpgrade
                                     && PlayerInputController.Instance.Stats.SkillPoints >=
                                     clickedCell.RelatedSkill.UpgradeCost;

        var canBeSelected = clickedCell.RelatedSkill.type != ActionType.Passive;
        foreach (var option in equipOption)
        {
            option.interactable = canBeSelected;
        }

        skillOptions.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(skillOptions.GetChild(0).gameObject);
        skillOptions.position = clickedCell.transform.position;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (!skillOptions.gameObject.activeSelf) return;
        StartCoroutine(WaitAndCheckSelection());
    }

    private IEnumerator WaitAndCheckSelection()
    {
        yield return new WaitForSeconds(0.05f);
        if (EventSystem.current.currentSelectedGameObject != null
            && EventSystem.current.currentSelectedGameObject.name.Contains("Option")) yield break;

        skillOptions.gameObject.SetActive(false);
    }


    private void OnSkillUpgraded(SkillCellManager toUpdate)
    {
        toUpdate.Display(toUpdate.RelatedSkill);
        description.DisplaySkill(toUpdate.RelatedSkill);
    }

    private void OnSkillCellSelected(SkillCellManager selectedCell, bool isSelected)
    {
        if (!isSelected) return;

        description.DisplaySkill(selectedCell.RelatedSkill);
    }

    private void FillSkillCells()
    {
        var allSkills = PlayerActionManager.Instance.AllSkills;

        cellManagers = new List<SkillCellManager>();
        foreach (var skill in allSkills)
        {
            var cellManager = Instantiate(skillCellManagerPrefab, skillCellHolder);
            cellManager.Display(skill);
            cellManagers.Add(cellManager);
        }
    }
}