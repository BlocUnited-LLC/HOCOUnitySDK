using UnityEngine;
using Newtonsoft.Json;

namespace Hoco.Samples
{
    [System.Serializable]

    public class ItemStats
    {
        public ItemStats()
        {
            PrimaryStats = new PrimaryStats();
            SecondaryStats = new SecondaryStats();
        }

        public ItemStats(PrimaryStats primaryStats, SecondaryStats secondaryStats)
        {
            PrimaryStats = primaryStats;
            SecondaryStats = secondaryStats;
        }

        [field: SerializeField]
        public PrimaryStats PrimaryStats { get; protected set; } = new PrimaryStats();
        [field: SerializeField]
        public SecondaryStats SecondaryStats { get; protected set; } = new SecondaryStats();

        [JsonIgnore]
        /// <summary> An example on how calculate HealthRegen from Endurance </summary>
        public float CompoundSecondary_HealthRegen { get { return GetCompoundSecondaryStat(nameof(SecondaryStats.HealthRegen), SecondaryStats.k_healthRegenModifier, SecondaryStats.k_healthRegenMultiplier); } }

        [JsonIgnore]

        /// <summary> An example on how calculate MaxHealth from Endurance </summary>
        public float CompoundSecondary_MaxHealth { get { return GetCompoundSecondaryStat(nameof(SecondaryStats.MaxHealth), SecondaryStats.k_maxHealthModifier, SecondaryStats.k_maxHealthMultiplier); } }

        /// <summary>
        /// This version of <see cref="GetStat(string)"/> is used to easily calculate stats based on other stats, for example, HealthRegen is calculated from Endurance
        /// </summary>
        /// <param name="name"></param>
        /// <param name="statModifier">The name of the value to use to modify the stat</param>
        /// <param name="statMultiplier">The multiplier of that value to add to the result</param>
        /// <returns>Whatever was in the stat, + the modifier value by the other stats</returns>
        public float GetCompoundSecondaryStat(string name, string statModifier, float statMultiplier)
        {
            var baseStat = SecondaryStats.GetStat(name);
            return baseStat + (baseStat * statMultiplier * PrimaryStats.GetStat(statModifier));
        }
    }
}