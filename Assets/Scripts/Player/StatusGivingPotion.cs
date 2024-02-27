using UnityEngine;

public class StatusGivingPotion : MonoBehaviour
{
    [SerializeField] private StatEffect effect;

    public void AddStatusToTarget()
    {
        CombatModeGameManager.Instance.SelectedEnemy.Stats.StatusManager.AddStatusEffect(Instantiate(effect));
    }
}