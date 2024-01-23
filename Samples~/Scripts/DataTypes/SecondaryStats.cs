using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hoco.Samples
{
    /// <summary>
    /// The Secondary Stats for the game world that are calculated from the Primary Stats
    /// </summary>
    [System.Serializable]
    public class SecondaryStats : BaseStats
    {
        public const float k_healthRegenMultiplier = 10f;
        public const string k_healthRegenModifier = nameof(PrimaryStats.Endurance);
        public const float k_maxHealthMultiplier = 5f;
        public const string k_maxHealthModifier = nameof(PrimaryStats.Endurance);
        public SecondaryStats() : base(new Dictionary<string, float>())
        {
            Stats = new Dictionary<string, float>();
        }
        public SecondaryStats(Dictionary<string, float> stats) : base(stats)
        {
            Stats = stats;
        }

        /// <summary> Use as the Base value for adding health over time to characters when out of combat </summary>
        [JsonIgnore] public float HealthRegen { get => GetStat(nameof(HealthRegen)); set => SetStat(nameof(HealthRegen), value); }
        /// <summary> The max the Health can regen to from Healing Skills or HPS </summary>
        [JsonIgnore] public float MaxHealth { get => GetStat(nameof(MaxHealth)); set => SetStat(nameof(MaxHealth), value); }
        /// <summary> Use as the Base value for adding mana over time to characters when out of combat </summary>
        [JsonIgnore] public float ManaRegen { get => GetStat(nameof(ManaRegen)); set => SetStat(nameof(ManaRegen), value); }
        /// <summary> The max the Mana can regen to </summary>
        [JsonIgnore] public float MaxMana { get => GetStat(nameof(MaxMana)); set => SetStat(nameof(MaxMana), value); }
        /// <summary> Use as the Base value for adding stamina over time to characters when out of combat </summary>
        [JsonIgnore] public float StaminaRegen { get => GetStat(nameof(StaminaRegen)); set => SetStat(nameof(StaminaRegen), value); }
        /// <summary> The max the Stamina can regen to </summary>
        [JsonIgnore] public float MaxStamina { get => GetStat(nameof(MaxStamina)); set => SetStat(nameof(MaxStamina), value); }

        /// <summary> Make this a value between 0 and 100 to rep a percentage </summary>
        [JsonIgnore] public float CritChance { get => GetStat(nameof(CritChance)); set => SetStat(nameof(CritChance), value); }
        /// <summary> A Hard multiplier against output power for Damage Buffs and Healing </summary>
        [JsonIgnore] public float CritPower { get => GetStat(nameof(CritPower)); set => SetStat(nameof(CritPower), value); }
    }
}