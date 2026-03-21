using System.Collections;
using System.Collections.Generic;
using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButtonsController : MonoBehaviour
{
    [SerializeField] private GameObject stickCharSpawnButton;
    [SerializeField] private GameObject spearCharSpawnButton;
    [SerializeField] private GameObject stoneCharSpawnButton;

    [SerializeField] private Button stickPurchaseButton;
    [SerializeField] private Button stickUpgradeButton;
    [SerializeField] private Button spearPurchaseButton;
    [SerializeField] private Button spearUpgradeButton;
    [SerializeField] private Button stonePurchaseButton;
    [SerializeField] private Button stoneUpgradeButton;

    [SerializeField] private GameObject insufficientBalanceAlertText;
    private Dictionary<WarriorType, Warrior> _characterFeatures;
    private GameManager _gameManager;
    private Helper _helper;
    private ShopManager _shopManager;

    private void Start()
    {
        Initialize();
    }


    private void Initialize()
    {
        stickCharSpawnButton.SetActive(true);
        spearCharSpawnButton.SetActive(false);
        stoneCharSpawnButton.SetActive(false);
        insufficientBalanceAlertText.SetActive(false);

        _shopManager = GetComponent<ShopManager>();
        _helper = GetComponent<Helper>();
        _characterFeatures = _helper.CharTypeToFeatureDict;
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        InitializeButtonsAndTexts();
    }

    private void UpdateButtonText(WarriorType type, Button button, bool isPurchaseButton)
    {
        var feature = _characterFeatures[type];

        if (isPurchaseButton)
            button.GetComponentInChildren<TMP_Text>().text = $"{feature.purchasePrice:0.00}";
        else
            button.GetComponentInChildren<TMP_Text>().text =
                $"{feature.upgradePrice:0.00} ({feature.spawnRate:0.00} s)";
    }

    private void InitializeButtonsAndTexts()
    {
        PurchaseStickCharInit();

        stickUpgradeButton.onClick.AddListener(UpgradeStickChar);
        UpdateButtonText(WarriorType.StickCharacter, stickUpgradeButton, false);

        spearPurchaseButton.onClick.AddListener(PurchaseSpareChar);
        UpdateButtonText(WarriorType.SpearCharacter, spearPurchaseButton, true);

        spearUpgradeButton.onClick.AddListener(UpgradeSpareChar);
        UpdateButtonText(WarriorType.SpearCharacter, spearUpgradeButton, false);

        stonePurchaseButton.onClick.AddListener(PurchaseStoneChar);
        UpdateButtonText(WarriorType.StoneCharacter, stonePurchaseButton, true);

        stoneUpgradeButton.onClick.AddListener(UpgradeStoneChar);
        UpdateButtonText(WarriorType.StoneCharacter, stoneUpgradeButton, false);
    }

    private void PurchaseStickCharInit()
    {
        stickPurchaseButton.GetComponentInChildren<TMP_Text>().text = "Purchased!";
        stickPurchaseButton.GetComponent<Image>().color = Color.green;
        stickPurchaseButton.interactable = false;
    }

    private void UpgradeStickChar()
    {
        UpgradeCharacter(WarriorType.StickCharacter, stickUpgradeButton);
    }

    private void PurchaseSpareChar()
    {
        PurchaseCharacter(WarriorType.SpearCharacter, spearCharSpawnButton, spearPurchaseButton, spearUpgradeButton);
    }

    private void UpgradeSpareChar()
    {
        UpgradeCharacter(WarriorType.SpearCharacter, spearUpgradeButton);
    }

    private void PurchaseStoneChar()
    {
        PurchaseCharacter(WarriorType.StoneCharacter, stoneCharSpawnButton, stonePurchaseButton, stoneUpgradeButton);
    }

    private void UpgradeStoneChar()
    {
        UpgradeCharacter(WarriorType.StoneCharacter, stoneUpgradeButton);
    }

    private void UpgradeCharacter(WarriorType type, Button upgradeButton)
    {
        if (_shopManager.CanUpgrade(type))
        {
            _shopManager.UpgradeCharacter(type);
            UpdateButtonText(type, upgradeButton, false);
        }
    }

    private void PurchaseCharacter(WarriorType type, GameObject spawnButton, Button purchaseButton,
        Button upgradeButton)
    {
        if (_shopManager.CanPurchase(type))
        {
            _shopManager.PurchaseCharacter(type);
            spawnButton.SetActive(true);
            upgradeButton.interactable = true;

            purchaseButton.GetComponentInChildren<TMP_Text>().text = "Purchased!";
            purchaseButton.GetComponent<Image>().color = Color.green;
            purchaseButton.interactable = false;
        }
        else
        {
            StartCoroutine(AlertPurchaseFail(purchaseButton));
        }
    }

    private IEnumerator AlertPurchaseFail(Button button)
    {
        var buttonColor = button.GetComponent<Image>().color;
        insufficientBalanceAlertText.SetActive(true);
        button.interactable = false;
        button.GetComponent<Image>().color = Color.red;
        yield return new WaitForSeconds(1);
        button.GetComponent<Image>().color = buttonColor;
        button.interactable = true;
        insufficientBalanceAlertText.SetActive(false);
    }
}