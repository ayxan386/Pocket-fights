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

    public IEnumerator Start()
    {
        yield return new WaitUntil(() => PlayerActionManager.Instance != null
                                         && PlayerActionManager.Instance.AllSkills.Count > 0);

        UpdateUi();
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
                    skillDisplays[0].icon.sprite = selectedSkill.displayDetails.icon;
                    flag += 1;
                    break;
                case "2":
                    skillDisplays[1].icon.sprite = selectedSkill.displayDetails.icon;
                    flag += 2;
                    break;
                case "3":
                    skillDisplays[2].icon.sprite = selectedSkill.displayDetails.icon;
                    flag += 4;
                    break;
                case "4":
                    skillDisplays[3].icon.sprite = selectedSkill.displayDetails.icon;
                    flag += 8;
                    break;
            }
        }

        for (var i = 0; i < skillDisplays.Count; i++)
        {
            var isSelected = (flag >> i) & 1;
            if (isSelected == 1) continue;

            skillDisplays[i].icon.sprite = emptySlotIcon;
        }
    }
}

[Serializable]
public class SkillSelectionDisplay
{
    public TextMeshProUGUI slotNameText;
    public Image icon;
}