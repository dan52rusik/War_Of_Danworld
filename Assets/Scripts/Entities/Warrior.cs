using System.Collections;
using System.Linq;
using UnityEngine.Events;
using Enums;
using UnityEngine;
using UnityEngine.AI;

public class Warrior : BattleEntity
{
    [SerializeField] public float power;
    [SerializeField] public float attackRate;
    [SerializeField] public float speed;
    [SerializeField] public float spawnRate;
    [SerializeField] public int spawnPrice;
    [SerializeField] public int purchasePrice;
    [SerializeField] public int upgradePrice;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private ParticleSystem _deathParticle;
    public GameObject Target;
    public BattleEntity TargetBattleEntity;
    private Animator _animator;
    private bool _anyOpponentAround;
    private bool _canGetDamage = true;
    private SpawnManager _spawnManager;
    private GameObject _spawnManagerGameObject;
    private BattleEntity _observedTargetBattleEntity;
    private UnityAction<GameObject> _targetDestroyedListener;
    private NavMeshObstacle _navMeshObstacle;

    protected override void Start()
    {
        base.Start();
        _animator = GetComponent<Animator>();
        _navMeshObstacle = GetComponentInChildren<NavMeshObstacle>();
        if (_navMeshObstacle != null)
            _navMeshObstacle.enabled = false;

        _spawnManagerGameObject = GameObject.FindWithTag("SpawnManager");
        _spawnManager = _spawnManagerGameObject.GetComponent<SpawnManager>();
        _spawnManager.OnWarriorSpawn.AddListener(x =>
        {
            if (gameObject && !AnyOpponentAround())
                SelectTarget();
        });
        _gameManager.OnMainMenuButtonPressed.AddListener(OnMainMenuPressed);
        _gameManager.OnGameOver.AddListener(OnGameOver);

        SelectTarget();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_anyOpponentAround)
        {
            var isAllyLayer = other.gameObject.layer == LayerMask.NameToLayer("Ally");
            var isEnemyLayer = other.gameObject.layer == LayerMask.NameToLayer("Enemy");

            if ((isEnemy && isAllyLayer) || (!isEnemy && isEnemyLayer))
            {
                SelectTarget(other.gameObject);
            }
        }
    }

    private void OnMainMenuPressed()
    {
        Destroy(gameObject);
    }

    private void OnGameOver(Castle castle)
    {
        if (!castle)
            return;

        if (castle.isEnemy != isEnemy) StartCoroutine(AsyncDance());
    }

    private IEnumerator AsyncDance()
    {
        yield return new WaitForSeconds(Random.Range(0, 2));
        _animator.CrossFadeInFixedTime("Dance", 0.1f, 0);
    }

    public override void GetDamage(float damage, GameObject attacker = null)
    {
        currentHealth -= damage;

        UpdateHealthBar();

        if (currentHealth <= 0 && _canGetDamage)
        {
            _canGetDamage = false;
            if (isEnemy)
                _spawnManager.ActiveEnemies.Remove(gameObject);
            else
                _spawnManager.ActiveAllies.Remove(gameObject);

            if (isEnemy) _shopManager.EarnGold(destroyReward);
            onDestroy.Invoke(gameObject);
            healthBar.SetActive(false);
            gameObject.layer = LayerMask.NameToLayer("Default");
            GetComponent<SphereCollider>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
            if (_navMeshObstacle != null)
                _navMeshObstacle.enabled = false;
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            _animator.CrossFadeInFixedTime("Death", 0.1f, 0);
            Instantiate(_deathParticle, gameObject.transform.position, _deathParticle.transform.rotation);
        }
    }


    public void SelectTarget(GameObject target = null, GameObject discardTarget = null, bool towardCastle = false)
    {
        if (_spawnManager.ActiveAllies.Count <= 0 && _spawnManager.ActiveEnemies.Count <= 0)
            return;

        if (target)
        {
            SetTarget(target);
            return;
        }

        var targetEntities = isEnemy ? SpawnManager.ActiveAllies : SpawnManager.ActiveEnemies;

        if (towardCastle)
        {
            var castle = targetEntities.FirstOrDefault(warrior =>
                warrior.GetComponent<BattleEntity>().EntityType == EntityType.Castle);
            SetTarget(castle);
            return;
        }

        var anyWarrior = targetEntities.Any(warrior =>
            warrior.GetComponent<BattleEntity>().EntityType == EntityType.Warrior);

        if (!Target && !anyWarrior)
        {
            target = targetEntities.First();
            SetTarget(target);
            return;
        }

        target = targetEntities
            .FirstOrDefault(warrior =>
            {
                var battleEntity = warrior.GetComponent<BattleEntity>();

                return battleEntity.EntityType == EntityType.Warrior && warrior != discardTarget &&
                       battleEntity.currentHealth > 0;
            });


        SetTarget(target);
    }

    private void SetTarget(GameObject target)
    {
        UnsubscribeFromTargetDestroyed();

        if (!target)
        {
            Target = null;
            TargetBattleEntity = null;
            return;
        }

        Target = target;
        TargetBattleEntity = target.GetComponent<BattleEntity>();
        _observedTargetBattleEntity = TargetBattleEntity;

        if (_observedTargetBattleEntity == null)
            return;

        _targetDestroyedListener = destroyedTarget => { SelectTarget(discardTarget: destroyedTarget); };
        _observedTargetBattleEntity.onDestroy.AddListener(_targetDestroyedListener);
    }

    private void UnsubscribeFromTargetDestroyed()
    {
        if (_observedTargetBattleEntity != null && _targetDestroyedListener != null)
            _observedTargetBattleEntity.onDestroy.RemoveListener(_targetDestroyedListener);

        _observedTargetBattleEntity = null;
        _targetDestroyedListener = null;
    }

    private void OnDestroy()
    {
        UnsubscribeFromTargetDestroyed();
    }

    private bool AnyOpponentAround()
    {
        var layer = isEnemy ? "Ally" : "Enemy";
        var layerNo = LayerMask.GetMask(layer);
        var hitColliders = Physics.OverlapSphere(transform.position, 1, layerNo);

        if (hitColliders.Length > 0)
        {
            _anyOpponentAround = true;
            return true;
        }

        _anyOpponentAround = false;
        return false;
    }
}
