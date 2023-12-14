using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectionManager : MonoBehaviour
{
    [SerializeField] private List<SkillSelectionDisplay> skillDisplays;
    [SerializeField] private Sprite emptySlotIcon;
    [SerializeField] private bool displayManaCost;

    public IEnumerator Start()
    {
        yield return new WaitUntil(() => PlayerActionManager.Instance != null
                                         && PlayerActionManager.Instance.AllSkills.Count > 0);
        if(displayManaCost)
            EventManager.OnStatChanged += OnStatChanged;

        UpdateUi();
    }

    private void OnStatChanged(StatValue statValue, StatData obj)
    {
        if (statValue == StatValue.Mana)
        {
            UpdateUi();
        }
    }

    public void UpdateUi()
    {
        var selectedSkills = PlayerActionManager.Instance.AllSkills.FindAll(skill => skill.isSelected);
        int flag = 0;

        foreach (var selectedSkill in selectedSkills)
        {
            if (!selectedSkill.isSelected) continue;

            switch (selectedSkill.slotName)
            {
                case "1":
                    UpdateSkillDisplay(skillDisplays[0], selectedSkill);
                    flag += 1;
                    break;
                case "2":
                    UpdateSkillDisplay(skillDisplays[1], selectedSkill);
                    flag += 2;
                    break;
                case "3":
                    UpdateSkillDisplay(skillDisplays[2], selectedSkill);
                    flag += 4;
                    break;
                case "4":
                    UpdateSkillDisplay(skillDisplays[3], selectedSkill);
                    flag += 8;
                    break;
            }
        }

        for (var i = 0; i < skillDisplays.Count; i++)
        {
            var isSelected = (flag >> i) & 1;
            if (isSelected == 1) continue;

            skillDisplays[i].icon.sprite = emptySlotIcon;
            skillDisplays[i].slotNameText.text = skillDisplays[i].slotNameText.text[0].ToString();
            skillDisplays[i].slotNameText.color = Color.black;
        }
    }

    private void UpdateSkillDisplay(SkillSelectionDisplay display, Skill skill)
    {
        display.icon.sprite = skill.displayDetails.icon;
        if (displayManaCost)
        {
            display.slotNameText.text = display.slotNameText.text[0]+ $" MP {skill.manaConsumption}";
            var currentMana = PlayerInputController.Instance.Stats.GetStatValue(StatValue.Mana).currentValue;
            var notEnoughMana = currentMana < skill.manaConsumption;
            display.notEnoughManaImage.gameObject.SetActive(notEnoughMana);
            display.slotNameText.color = notEnoughMana ? Color.red : Color.black;
        }
    }
}

[Serializable]
public class SkillSelectionDisplay
{
    public TextMeshProUGUI slotNameText;
    public Image notEnoughManaImage;
    public Image icon;
}