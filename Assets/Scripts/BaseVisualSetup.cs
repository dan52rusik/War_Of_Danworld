using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class BaseVisualSetup : MonoBehaviour
{
    [SerializeField] private GameObject allyBase;
    [SerializeField] private GameObject enemyBase;
    [SerializeField] private string towerResourcePath = "Models/Tower_def_model";
    [SerializeField] private Vector3 allyTowerOffset = new(0.74f, -1.09f, -2.38f);
    [SerializeField] private Vector3 enemyTowerOffset = new(-0.74f, -1.09f, 1.014f);
    [SerializeField] private Vector3 allyTowerEuler = new(0f, 120f, 0f);
    [SerializeField] private Vector3 enemyTowerEuler = new(0f, 300f, 0f);
    [SerializeField] private Vector3 towerScale = new(0.2f, 0.2f, 0.2f);

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

        var towerPrefab = Resources.Load<GameObject>(towerResourcePath);
        if (towerPrefab == null)
            return;

        var allyTower = FindOrCreateTower(towerPrefab, AllyTowerName, "Tower_def_model");
        var enemyTower = FindOrCreateTower(towerPrefab, EnemyTowerName, "Tower_def_model (1)");

        ConfigureTower(allyTower, allyBase.transform.position + allyTowerOffset, allyTowerEuler);
        ConfigureTower(enemyTower, enemyBase.transform.position + enemyTowerOffset, enemyTowerEuler);
    }

    private static void ToggleLegacyVisual(Transform root, string childName, bool isActive)
    {
        var child = root.Find(childName);
        if (child != null && child.gameObject.activeSelf != isActive)
            child.gameObject.SetActive(isActive);
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

    private void ConfigureTower(GameObject tower, Vector3 worldPosition, Vector3 worldEulerAngles)
    {
        if (tower == null)
            return;

        var towerTransform = tower.transform;
        towerTransform.SetPositionAndRotation(worldPosition, Quaternion.Euler(worldEulerAngles));
        towerTransform.localScale = towerScale;
    }
}
