using System.Collections.Generic;
using UnityEngine;

namespace Hoco.Samples
{
    [System.Serializable]
    public class BaseStats
    {
        public BaseStats(Dictionary<string, float> stats)
        {
            Stats = stats;
        }

        [field: SerializeField]
        public Dictionary<string, float> Stats { get; protected set; } = new Dictionary<string, float>();

        public float GetStat(string name)
        {
            return Stats.TryGetValue(name, out float value) ? value : 0f;
        }

        public void SetStat(string name, float value)
        {
            Stats[name] = RoundToDecimalPlaces(value, 3);
        }

        private float RoundToDecimalPlaces(float value, int decimalPlaces)
        {
            float multiplier = Mathf.Pow(10, decimalPlaces);
            return Mathf.Round(value * multiplier) / multiplier;
        }
    }
}