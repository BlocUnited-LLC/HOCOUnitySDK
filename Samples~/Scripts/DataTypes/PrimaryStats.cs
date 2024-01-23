using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hoco.Samples
{
    /// <summary>
    /// The Primary Stats for the game world.
    /// </summary>
    [System.Serializable]
    public class PrimaryStats : BaseStats
    {
        public PrimaryStats() : base(new Dictionary<string, float>())
        {
            Stats = new Dictionary<string, float>();
        }
        public PrimaryStats(Dictionary<string, float> stats) : base(stats)
        {
            Stats = stats;
        }
        [JsonIgnore] public int Strength { get => (int)GetStat(nameof(Strength)); set => SetStat(nameof(Strength), value); }
        [JsonIgnore] public int Wisdom { get => (int)GetStat(nameof(Wisdom)); set => SetStat(nameof(Wisdom), value); }
        [JsonIgnore] public int WillPower { get => (int)GetStat(nameof(WillPower)); set => SetStat(nameof(WillPower), value); }
        [JsonIgnore] public int Perception { get => (int)GetStat(nameof(Perception)); set => SetStat(nameof(Perception), value); }
        [JsonIgnore] public int Intelligence { get => (int)GetStat(nameof(Intelligence)); set => SetStat(nameof(Intelligence), value); }
        [JsonIgnore] public int Endurance { get => (int)GetStat(nameof(Endurance)); set => SetStat(nameof(Endurance), value); }
        [JsonIgnore] public int Agility { get => (int)GetStat(nameof(Agility)); set => SetStat(nameof(Agility), value); }
        [JsonIgnore] public int Charisma { get => (int)GetStat(nameof(Charisma)); set => SetStat(nameof(Charisma), value); }
    }
}