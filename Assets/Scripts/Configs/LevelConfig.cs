using System.Collections.Generic;
using UnityEngine;
using Wave;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Game/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    public int LevelId;
    public List<WaveConfig> Waves;
    private string _levelName;
}