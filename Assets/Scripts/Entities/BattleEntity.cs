using DG.Tweening;
using Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class BattleEntity : MonoBehaviour
{
    [SerializeField] public float maxHealth;
    [SerializeField] public float currentHealth;

    public UnityEvent<GameObject> onDestroy;
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private Slider laggingHealthBar;
    [SerializeField] public bool isEnemy;
    [SerializeField] public int destroyReward;
    [SerializeField] public WarriorType warriorType;
    [SerializeField] public EntityType EntityType;
    [SerializeField] private float lagDuration = 0.8f;

    [FormerlySerializedAs("GameManager")] [SerializeField]
    protected GameManager _gameManager;

    protected ShopManager _shopManager;
    private GameObject _shopManagerGameObject;
    protected SpawnManager SpawnManager;


    protected virtual void Start()
    {
        isEnemy = gameObject.layer == LayerMask.NameToLayer("Enemy");
        currentHealth = maxHealth;
        _shopManagerGameObject = GameObject.FindWithTag("ShopManager");
        _shopManager = _shopManagerGameObject.GetComponent<ShopManager>();
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        var spawnManagerGameObject = GameObject.FindWithTag("SpawnManager");
        SpawnManager = spawnManagerGameObject.GetComponent<SpawnManager>();
        // UpdateHealthBar();
    }

    protected void UpdateHealthBar()
    {
        var healthPercentage = currentHealth / maxHealth;
        healthBarSlider.value = healthPercentage;
        laggingHealthBar.DOValue(healthPercentage, lagDuration).SetEase(Ease.OutCubic);
    }

    public virtual void GetDamage(float damage, GameObject attacker = null)
    {
        currentHealth -= damage;
        UpdateHealthBar();
        if (currentHealth <= 0) onDestroy.Invoke(gameObject);
    }
}