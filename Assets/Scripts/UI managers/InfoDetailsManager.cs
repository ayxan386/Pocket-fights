using UnityEngine;

public class InfoDetailsManager : MonoBehaviour
{
    [SerializeField] private InfoRowIndicator baseAttack;
    [SerializeField] private InfoRowIndicator health;
    [SerializeField] private InfoRowIndicator maxMana;
    [SerializeField] private InfoRowIndicator damageReduction;
    [SerializeField] private InfoRowIndicator manaRegen;

    void OnEnable()
    {
        UpdateValues();
        EventManager.OnBaseStatUpdate += OnBaseStatUpdate;
        EventManager.OnPlayerCoreUpdate += OnPlayerCoreUpdate;
    }

    private void OnDisable()
    {
        EventManager.OnBaseStatUpdate += OnBaseStatUpdate;
        EventManager.OnPlayerCoreUpdate += OnPlayerCoreUpdate;
    }

    private void OnPlayerCoreUpdate(int obj)
    {
        UpdateValues();
    }

    private void OnBaseStatUpdate(float obj)
    {
        UpdateValues();
    }

    private void UpdateValues()
    {
        var statController = PlayerInputController.Instance.Stats;
        //Base attack
        var baseAttack = statController.GetStatValue(StatValue.BaseAttack);
        this.baseAttack.UpdateDisplay("Base attack", baseAttack.maxValue.ToString("N0"));

        //Health
        var health = statController.GetStatValue(StatValue.Health);
        this.health.UpdateDisplay("Health", health.maxValue.ToString("N0"));

        //Mana
        var mana = statController.GetStatValue(StatValue.Mana);
        this.maxMana.UpdateDisplay("Mana", mana.maxValue.ToString("N0"));

        //Damage Reduction
        var damageReduction = statController.GetStatValue(StatValue.DamageReduction);
        this.damageReduction.UpdateDisplay("Damage reduction", damageReduction.maxValue.ToString("N0"));

        //Mana Regen
        var manaRegen = statController.GetStatValue(StatValue.ManaRegen);
        this.manaRegen.UpdateDisplay("Mana regen.", manaRegen.maxValue.ToString("N0"));
    }
}