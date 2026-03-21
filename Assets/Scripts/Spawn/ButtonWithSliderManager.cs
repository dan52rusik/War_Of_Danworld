using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonWithSliderManager : MonoBehaviour
{
    [SerializeField] private GameObject warrior;
    [SerializeField] private Slider spawnSlider;
    [SerializeField] private Button spawnButton;
    private GameManager _gameManager;

    private Helper _helper;
    private ShopManager _shopManager;
    private GameObject _shopManagerGameObject;
    private Warrior _warrior;

    private void Start()
    {
        _warrior = warrior.GetComponent<Warrior>();
        spawnSlider.value = 0f;
        spawnButton.onClick.AddListener(StartLoading);
        _shopManagerGameObject = GameObject.Find("ShopManager");
        _shopManager = _shopManagerGameObject.GetComponent<ShopManager>();
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    private void StartLoading()
    {
        if (_shopManager.CanInstantiate(_warrior.warriorType) && !_gameManager.GameOver)
            StartCoroutine(LoadingCoroutine());
    }

    private IEnumerator LoadingCoroutine()
    {
        var loadingSpeed = _warrior.spawnRate;
        spawnButton.interactable = false;

        var progress = 1f;
        while (progress > 0f)
        {
            progress -= 1 / loadingSpeed * Time.deltaTime;
            spawnSlider.value = progress;
            yield return null;
        }

        if (!_gameManager.GameOver)
            spawnButton.interactable = true;
    }
}