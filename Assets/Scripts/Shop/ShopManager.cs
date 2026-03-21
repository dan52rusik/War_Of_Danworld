using System.Collections;
using System.Collections.Generic;
using Enums;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public int playerGold;
    public TMP_Text playerGoldUI;
    [SerializeField] private GameObject _goldCountAdd;
    [SerializeField] private TMP_Text _goldCountAddText;
    [SerializeField] private AudioClip _earnGoldSound;
    [SerializeField] private AudioClip _purchaseSound;
    private AudioSource _audioSource;
    private Dictionary<WarriorType, Warrior> _characterFeatures;
    private GameManager _gameManager;
    private Helper _helper;


    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _goldCountAdd.SetActive(false);
        playerGoldUI.text = playerGold.ToString();
        _helper = GetComponent<Helper>();
        _characterFeatures = _helper.CharTypeToFeatureDict;
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        _gameManager.OnGameOver.AddListener(OnGameOver);
    }

    private void OnGameOver(Castle castle)
    {
        if (castle)
            EarnGold(castle.destroyReward);
    }

    public void EarnGold(int earning)
    {
        _goldCountAdd.SetActive(true);

        _goldCountAddText.text = "+ " + earning;
        StartCoroutine(EarnMoneyAnimation(earning));


        // playerGold += earning;
        // playerGoldUI.text = playerGold.ToString();
    }

    private IEnumerator EarnMoneyAnimation(int earning)
    {
        _audioSource.clip = _earnGoldSound;
        yield return new WaitForSeconds(2);

        var valueToAdd = earning;
        var waitSecond = earning > 50 ? 0.03f : 0.05f;
        for (var i = valueToAdd; i > 0; i--)
        {
            _audioSource.Play();
            yield return new WaitForSeconds(waitSecond);
            playerGold += 1;
            playerGoldUI.text = playerGold.ToString();
            valueToAdd -= 1;
            _goldCountAddText.text = valueToAdd.ToString();
        }

        _goldCountAdd.SetActive(false);
    }

    private void PayGold(int payment)
    {
        _audioSource.clip = _purchaseSound;
        _audioSource.Play();
        playerGold -= payment;
        playerGoldUI.text = playerGold.ToString();
    }

    public bool CanInstantiate(WarriorType warriorType)
    {
        return playerGold >= _characterFeatures[warriorType].spawnPrice;
    }

    public void PayForInstantiate(WarriorType warriorType)
    {
        PayGold(_characterFeatures[warriorType].spawnPrice);
    }

    public void PurchaseCharacter(WarriorType warriorType)
    {
        var price = _characterFeatures[warriorType].purchasePrice;
        PayGold(price);
    }

    public bool CanPurchase(WarriorType warriorType)
    {
        return playerGold >= _characterFeatures[warriorType].purchasePrice;
    }

    public bool CanUpgrade(WarriorType type)
    {
        var isMaxed = _characterFeatures[type].spawnRate - 0.05f < 0;
        return playerGold >= _characterFeatures[type].upgradePrice && !isMaxed;
    }

    public void UpgradeCharacter(WarriorType type)
    {
        var price = _characterFeatures[type].upgradePrice;
        PayGold(price);
        _characterFeatures[type].spawnRate -= 0.05f;
        _characterFeatures[type].upgradePrice += 2;
    }
}