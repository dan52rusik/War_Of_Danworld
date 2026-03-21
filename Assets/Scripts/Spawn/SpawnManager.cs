using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _warriors;
    [SerializeField] private GameObject _allySpawnPoint;
    [SerializeField] private GameObject _enemySpawnPoint;
    [SerializeField] private GameObject _allyBase;
    [SerializeField] private GameObject _enemyBase;

    [SerializeField] private Button spawnStickWarriorButton;
    [SerializeField] private Button spawnSpearWarriorButton;
    [SerializeField] private Button spawnStoneWarriorButton;

    [SerializeField] private GameObject _gameManagerGameObject;
    [SerializeField] private GameObject _shopManagerGameObject;
    public UnityEvent<GameObject> OnWarriorSpawn;

    public readonly ObservableCollection<GameObject> ActiveAllies = new();
    public readonly ObservableCollection<GameObject> ActiveEnemies = new();
    private GameManager _gameManager;
    private ShopManager _shopManager;

    private void Start()
    {
        _gameManager = _gameManagerGameObject.GetComponent<GameManager>();
        _shopManager = _shopManagerGameObject.GetComponent<ShopManager>();
        InitializeSpawnButtons();
        UpdateSpawnButtonText();
        _gameManager.OnGameStart.AddListener(OnGameStart);
        _gameManager.OnGameOver.AddListener(x => OnGameOver());
    }
    
    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        // Check if the action affects the collection size
        if (e.Action == NotifyCollectionChangedAction.Add || 
            e.Action == NotifyCollectionChangedAction.Remove || 
            e.Action == NotifyCollectionChangedAction.Reset)
        {
            OnWarriorSpawn.Invoke(null);
        }
    }


    private void OnGameStart()
    {
        ActiveAllies.Clear();
        ActiveEnemies.Clear();

        ActiveEnemies.CollectionChanged += OnCollectionChanged;
        ActiveAllies.CollectionChanged += OnCollectionChanged;

        ActiveAllies.Add(_allyBase);
        ActiveEnemies.Add(_enemyBase);

        EnableSpawnButtons();
    }

    private void OnGameOver()
    {
        ActiveEnemies.CollectionChanged -= OnCollectionChanged;
        ActiveAllies.CollectionChanged -= OnCollectionChanged;
        DisableSpawnButtons();
    }

    private void EnableSpawnButtons()
    {
        spawnStickWarriorButton.interactable = true;
        spawnSpearWarriorButton.interactable = true;
        spawnStoneWarriorButton.interactable = true;
    }

    private void DisableSpawnButtons()
    {
        spawnStickWarriorButton.interactable = false;
        spawnSpearWarriorButton.interactable = false;
        spawnStoneWarriorButton.interactable = false;
    }

    private void InstantiateStickCharacter()
    {
        InstantiateWarrior(WarriorType.StickCharacter);
    }

    private void InstantiateSpearCharacter()
    {
        InstantiateWarrior(WarriorType.SpearCharacter);
    }

    private void InstantiateStoneCharacter()
    {
        InstantiateWarrior(WarriorType.StoneCharacter);
    }


    private void InitializeSpawnButtons()
    {
        spawnStickWarriorButton.onClick.AddListener(InstantiateStickCharacter);
        spawnSpearWarriorButton.onClick.AddListener(InstantiateSpearCharacter);
        spawnStoneWarriorButton.onClick.AddListener(InstantiateStoneCharacter);
    }

    private void UpdateSpawnButtonText()
    {
        var stickSpawnPrice = _warriors[0].GetComponent<Warrior>().spawnPrice;
        spawnStickWarriorButton.GetComponentInChildren<TMP_Text>().text =
            $"{stickSpawnPrice} ";

        var spearSpawnPrice = _warriors[1].GetComponent<Warrior>().spawnPrice;
        spawnSpearWarriorButton.GetComponentInChildren<TMP_Text>().text =
            $"{spearSpawnPrice}";

        var stoneSpawnPrice = _warriors[2].GetComponent<Warrior>().spawnPrice;
        spawnStoneWarriorButton.GetComponentInChildren<TMP_Text>().text =
            $"{stoneSpawnPrice}";
    }

    private void InstantiateWarrior(WarriorType type)
    {
        if (_shopManager.CanInstantiate(type) && !_gameManager.GameOver)
        {
            var ally = Instantiate(_warriors[(int)type], RandomAllyPosition(), _allySpawnPoint.transform.rotation);
            ActiveAllies.Add(ally);
            _shopManager.PayForInstantiate(type);
            ally.tag = "Ally";
            ally.layer = LayerMask.NameToLayer("Ally");
            ally.name = "Ally" + ally.name;
            OnWarriorSpawn.Invoke(null);
        }
    }

    public void InstantiateEnemy(WarriorType warrior)
    {
        var spawnPoint = _enemySpawnPoint;
        float randomSpawnLoc = Random.Range(-2, 2);

        var spawnPos = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y,
            spawnPoint.transform.position.z + randomSpawnLoc);
        var enemy = Instantiate(_warriors[(int)warrior], spawnPos, spawnPoint.transform.rotation);
        ActiveEnemies.Add(enemy);
        enemy.tag = "Enemy";
        enemy.layer = LayerMask.NameToLayer("Enemy");
        enemy.name = "Enemy" + enemy.name;
        OnWarriorSpawn.Invoke(null);
    }

    private Vector3 RandomAllyPosition()
    {
        var spawnPoint = _allySpawnPoint;
        float randomSpawnLoc = Random.Range(-2, 2);
        return new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y,
            spawnPoint.transform.position.z + randomSpawnLoc);
    }
}