using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WarriorMoveController : MonoBehaviour
{
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private float navmeshUpdateInterval = 1;
    [SerializeField] private double _pathRecalculationTolerance = 1.0f;
    [SerializeField] private float _walkAnimationMultiplier;
    private Animator _animator;
    private GameManager _gameManager;
    private Helper _helper;
    private Vector3 _lastTargetPosition;
    private bool _moreWarriorsAround;
    private Warrior _warrior;

    private void Start()
    {
        _warrior = GetComponent<Warrior>();
        _spawnManager = GameObject.FindWithTag("SpawnManager").GetComponent<SpawnManager>();
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        _animator = GetComponent<Animator>();
        _gameManager.OnGameOver.AddListener(x => OnGameOver());
        StartCoroutine(LateStart());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ShouldStopMoving(other)) _agent.speed = 0;

        var isAllyLayer = other.gameObject.layer == LayerMask.NameToLayer("Ally");
        var isEnemyLayer = other.gameObject.layer == LayerMask.NameToLayer("Enemy");

        if ((_warrior.isEnemy && isAllyLayer) || (!_warrior.isEnemy && isEnemyLayer))
        {
            other.GetComponent<BattleEntity>().onDestroy.AddListener(x =>
            {
                _warrior.SelectTarget(towardCastle: true);
                Move();
            });

            if (_warrior.currentHealth > 0)
                StartCoroutine(RotateTowardsTarget());
        }
    }
    //
    // private void OnTriggerStay(Collider other)
    // {
    //     var isAllyLayer = other.gameObject.layer == LayerMask.NameToLayer("Ally");
    //     var isEnemyLayer = other.gameObject.layer == LayerMask.NameToLayer("Enemy");
    //
    //     if ((_warrior.isEnemy && isAllyLayer) || (!_warrior.isEnemy && isEnemyLayer)) _agent.speed = 0;
    // }

    private IEnumerator RotateTowardsTarget()
    {
        var rotationSpeed = _warrior.speed * 50;
        var target = _warrior.Target;
        var rotationDuration = 2;
        while (rotationDuration > 0)
            if (target && _warrior.TargetBattleEntity.currentHealth > 0)
            {
                var targetDirection = (target.transform.position - transform.position).normalized;
                var targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                rotationDuration -= 1;
                yield return new WaitForSeconds(1);
            }
            else
            {
                yield break;
            }
    }

    private void OnGameOver()
    {
        _agent.speed = 0;
        _animator.CrossFade("Idle", 0.1f, 0);
    }

    private void Move()
    {
        if (!_gameManager.GameOver && _warrior.currentHealth > 0)
        {
            _agent.speed = _warrior.speed;
            _animator.CrossFadeInFixedTime("Walk", 0.1f, 0);
        }
    }

    private bool ShouldStopMoving(Collider other)
    {
        return _warrior.Target == other.gameObject;
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(UpdateDestination());
        SetWalkAnimationSpeed();
        Move();
    }

    private IEnumerator UpdateDestination()
    {
        while (true)
        {
            if (_gameManager.GameOver)
                break;

            if (!gameObject)
                break;

            if (!_warrior.Target)
                _warrior.SelectTarget();

            var target = _warrior?.Target?.transform;

            if (_agent.enabled && gameObject && _warrior.Target)
                if (Vector3.Distance(target.position, _lastTargetPosition) > _pathRecalculationTolerance)
                {
                    _agent.SetDestination(target.position);
                    _lastTargetPosition = target.position;
                }

            yield return new WaitForSeconds(navmeshUpdateInterval);
        }
    }

    private void SetWalkAnimationSpeed()
    {
        _animator.SetFloat("AnimMultiplier", _walkAnimationMultiplier * _warrior.speed);
    }
}