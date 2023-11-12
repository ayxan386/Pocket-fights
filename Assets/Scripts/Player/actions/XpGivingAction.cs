using UnityEngine;

public class XpGivingAction : MonoBehaviour
{
    [SerializeField] private int amount;

    public void GrantXp()
    {
        PlayerInputController.Instance.AddXp(amount);
    }
}