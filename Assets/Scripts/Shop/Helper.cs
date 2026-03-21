using System.Collections.Generic;
using Enums;
using UnityEngine;

public class Helper : MonoBehaviour
{
    public List<GameObject> warriorsGameObjects;
    public readonly Dictionary<WarriorType, Warrior> CharTypeToFeatureDict = new();
    public static readonly Dictionary<WarriorType, GameObject> CharTypeToObjectsDict = new();

    // Start is called before the first frame update
    private void Awake()
    {
        PopulateCharTypeToFeatureDict();
    }

    private void PopulateCharTypeToFeatureDict()
    {
        foreach (var warrior in warriorsGameObjects)
        {
            var charFeature = warrior.GetComponent<Warrior>();
            CharTypeToFeatureDict.Add(charFeature.warriorType, charFeature);
            CharTypeToObjectsDict.Add(charFeature.warriorType, warrior);
        }
    }
}