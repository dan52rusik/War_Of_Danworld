using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Wave
{
    [CreateAssetMenu(fileName = "NewWave", menuName = "Game/WaveConfig")]
    public class WaveConfig : ScriptableObject
    {
        public int waveId;
        public string waveName;
        public List<WarriorType> warriorTypes;
        public float waveDelay;
    }
}