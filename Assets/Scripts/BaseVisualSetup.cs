using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class BaseVisualSetup : MonoBehaviour
{
    [SerializeField] private GameObject allyBase;
    [SerializeField] private GameObject enemyBase;
    [SerializeField] private string allyTowerResourcePath = "Prefabs/Tower_def_model_Ally";
    [SerializeField] private string enemyTowerResourcePath = "Prefabs/Tower_def_model_Enemy";
    [SerializeField] private Vector3 allyTowerOffset = new(0.74f, -1.09f, -2.38f);
    [SerializeField] private Vector3 enemyTowerOffset = new(-0.74f, -1.09f, 1.014f);

    private const string AllyTowerName = "Tower_def_model_Ally";
    private const string EnemyTowerName = "Tower_def_model_Enemy";

    private void OnEnable()
    {
        EnsureSetup();
    }

    private void OnValidate()
    {
        EnsureSetup();
    }

    private void EnsureSetup()
    {
        if (allyBase == null || enemyBase == null)
            return;

        ToggleLegacyVisual(allyBase.transform, "AllyCastle", false);
        ToggleLegacyVisual(enemyBase.transform, "EnemyCastle", false);

        var allyTowerPrefab = Resources.Load<GameObject>(allyTowerResourcePath);
        var enemyTowerPrefab = Resources.Load<GameObject>(enemyTowerResourcePath);
        if (allyTowerPrefab == null || enemyTowerPrefab == null)
            return;

        var allyTower = FindOrCreateTower(allyTowerPrefab, AllyTowerName, "Tower_def_model");
        var enemyTower = FindOrCreateTower(enemyTowerPrefab, EnemyTowerName, "Tower_def_model (1)");

        ConfigureTower(allyTower, allyBase.transform.position + allyTowerOffset);
        ConfigureTower(enemyTower, enemyBase.transform.position + enemyTowerOffset);

        DisableLooseLegacyTower(allyTower, enemyTower, "Tower_def_model");
        DisableLooseLegacyTower(allyTower, enemyTower, "Tower_def_model (1)");
    }

    private static void ToggleLegacyVisual(Transform root, string childName, bool isActive)
    {
        var child = root.Find(childName);
        if (child != null && child.gameObject.activeSelf != isActive)
            child.gameObject.SetActive(isActive);
    }

    private static void DisableLooseLegacyTower(GameObject allyTower, GameObject enemyTower, string legacyName)
    {
        var legacyTower = GameObject.Find(legacyName);
        if (legacyTower != null && legacyTower != allyTower && legacyTower != enemyTower)
            legacyTower.SetActive(false);
    }

    private GameObject FindOrCreateTower(GameObject towerPrefab, string desiredName, string legacyName)
    {
        var existingTower = GameObject.Find(desiredName);
        if (existingTower != null)
            return existingTower;

        var legacyTower = GameObject.Find(legacyName);
        if (legacyTower != null)
        {
            legacyTower.name = desiredName;
            return legacyTower;
        }

#if UNITY_EDITOR
        GameObject towerInstance;
        if (!Application.isPlaying)
            towerInstance = (GameObject)PrefabUtility.InstantiatePrefab(towerPrefab);
        else
            towerInstance = Instantiate(towerPrefab);
#else
        var towerInstance = Instantiate(towerPrefab);
#endif

        towerInstance.name = desiredName;
        return towerInstance;
    }

    private static void ConfigureTower(GameObject tower, Vector3 worldPosition)
    {
        if (tower == null)
            return;

        tower.transform.position = worldPosition;
    }
}
