using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class WarriorAttackController : MonoBehaviour
{
    [SerializeField] private ParticleSystem _hitParticle;
    [SerializeField] private Transform _particleTransform;

    [SerializeField] private AudioSource _hitAudio;

    // [SerializeField] private List<GameObject> _activeAllies;
    // [SerializeField] private List<GameObject> _activeEnemies;
    private Animator _animator;
    private float _attackRate;
    private bool _canAttack = true;
    private GameManager _gameManager;

    private Helper _helper;
    private SpawnManager _spawnManager;
    private BattleEntity _target;
    private Warrior _warrior;

    private void Start()
    {
        _warrior = GetComponent<Warrior>();
        _attackRate = _warrior.attackRate;
        _spawnManager = GameObject.FindWithTag("SpawnManager").GetComponent<SpawnManager>();
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        _hitAudio = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
    }


    public void OnTriggerStay(Collider other)
    {
        var isAllyLayer = other.gameObject.layer == LayerMask.NameToLayer("Ally");
        var isEnemyLayer = other.gameObject.layer == LayerMask.NameToLayer("Enemy");

        if ((_warrior.isEnemy && isAllyLayer) || (!_warrior.isEnemy && isEnemyLayer))
        {
            _warrior.SelectTarget(other.gameObject);
            
            if(_warrior.Target == other.gameObject)
                Attack();
        }
    }

    private void Attack()
    {
        if (_canAttack && !_gameManager.GameOver && _warrior.currentHealth > 0)
        {
            CloseAttack();
            StartCoroutine(AttackCooldown());
        }
    }

    private void CloseAttack()
    {
        _animator.CrossFadeInFixedTime("Attack", 0.5f, 0, 0);
    }

    private IEnumerator AttackCooldown()
    {
        _canAttack = false;
        yield return new WaitForSeconds(_attackRate);
        _canAttack = true;
    }

    public void Damage()
    {
        Instantiate(_hitParticle, _particleTransform);
        _hitAudio.pitch = Random.Range(0.9f, 1.1f);
        _hitAudio.Play();
        if (gameObject && _warrior.gameObject && _warrior.Target)
            _warrior?.TargetBattleEntity?.GetDamage(_warrior.power, _warrior.gameObject);
    }
}