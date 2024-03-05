using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobActionManager : MonoBehaviour
{
    [SerializeField] private IntentIndicator intentIndicator;
    [SerializeField] private Skill damagedSkill;
    [SerializeField] private List<MobTurnSequence> turnSequences;

    private int index = -1;

    public void ShowIntentPanel()
    {
        intentIndicator.gameObject.SetActive(true);
    }

    public void NextTurn()
    {
        index = (index + 1) % turnSequences.Count;

        var turn = turnSequences[index];
        intentIndicator.UpdateIntent(turn);
    }

    public void UseAttack(StatController stats, Action<bool> actionComplete)
    {
        var turn = turnSequences[index];
        intentIndicator.UpdateTitle(turn);

        StartCoroutine(ExecuteTurn(turn, stats, actionComplete));
    }

    private IEnumerator ExecuteTurn(MobTurnSequence turn, StatController stats, Action<bool> isTurnComplete)
    {
        var playerStats = PlayerInputController.Instance.Stats;
        foreach (var skill in turn.actionToUse)
        {
            skill.Lock = true;
            skill.usageEffects?.Invoke(skill, stats, playerStats);
            yield return new WaitUntil(() => !skill.Lock);
        }

        intentIndicator.HideTitle();
        isTurnComplete(true);
    }

    public void AttackReceived(StatController statController, float receivedDamage)
    {
        damagedSkill.usageEffects?.Invoke(damagedSkill, statController, null);
    }
}

[Serializable]
public class MobTurnSequence
{
    public Sprite intentIcon;
    public Color intentColor = Color.white;
    public string attackTitle;
    public Color titleColor = Color.white;
    public List<Skill> actionToUse;
}