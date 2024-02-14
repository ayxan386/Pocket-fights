using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillCellManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler,
    IPointerClickHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private Transform upgradeHolder;
    [SerializeField] private Image upgradeCircle;
    [SerializeField] private Color emptyColor;
    [SerializeField] private Color filledColor;

    private List<Image> upgradeCircles;
    public Skill RelatedSkill { get; private set; }

    public void Display(Skill skill)
    {
        icon.sprite = skill.displayDetails.icon;
        RelatedSkill = skill;
        if (upgradeCircles == null || upgradeCircles.Count != skill.maxLevel)
        {
            CreateCircles(skill);
        }

        for (int i = 0; i < upgradeCircles.Count; i++)
        {
            upgradeCircles[i].color = i <= skill.currentLevel ? filledColor : emptyColor;
        }
    }

    private void CreateCircles(Skill skill)
    {
        if (upgradeCircles != null)
        {
            foreach (var circle in upgradeCircles)
            {
                Destroy(circle.gameObject);
            }
        }

        upgradeCircles = new List<Image>();

        for (int i = 0; i < skill.maxLevel; i++)
        {
            upgradeCircles.Add(Instantiate(upgradeCircle, upgradeHolder));
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventManager.OnSkillCellSelected?.Invoke(this, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventManager.OnSkillCellSelected?.Invoke(this, false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        EventManager.OnSkillCellSelected?.Invoke(this, true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SkillClicked();
    }

    public void SkillClicked()
    {
        EventManager.OnSkillCellClicked?.Invoke(this);
    }
}