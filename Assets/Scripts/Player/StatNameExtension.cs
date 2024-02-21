namespace Player
{
    public static class StatNameExtension
    {
        public static string StatValueName(this StatValue statValue)
        {
            switch (statValue)
            {
                case StatValue.BaseAttack:
                    return "Attack";
                case StatValue.DamageReduction:
                    return "Damage reduction";
                case StatValue.ManaRegen:
                    return "Mana regeneration";
                case StatValue.None:
                    return "";
                case StatValue.Health:
                case StatValue.Mana:
                default:
                    return statValue.ToString();
            }

            return statValue.ToString();
        }
    }
}