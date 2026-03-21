using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ShopTween : MonoBehaviour
{
    [SerializeField] private Button shopToggleButton;
    [SerializeField] private Button shopCloseButton;
    [SerializeField] private bool isVisible;
    public GameObject shopUI;
    public RectTransform shopPanel;
    public float animationDuration = 0.5f;

    private Vector2 hiddenPosition;
    private Vector2 visiblePosition;

    void Start()
    {
        shopPanel = shopUI.GetComponent<RectTransform>();
        visiblePosition = shopPanel.anchoredPosition;
        hiddenPosition = new Vector2(shopPanel.anchoredPosition.x, -(Screen.height + shopPanel.rect.height * 2));
        shopPanel.anchoredPosition = hiddenPosition;
        shopUI.SetActive(false);
        shopToggleButton.onClick.AddListener(ToggleShopUI);
        shopCloseButton.onClick.AddListener(ToggleShopUI);
    }
    
    
    public void ToggleShopUI()
    {
        isVisible = !isVisible;
        if (!isVisible)
        {
            ShowShop();
        }
        else
        {
            HideShop();
        }
    }
    
    
    private void ShowShop()
    {
        shopUI.SetActive(true);
        shopPanel.DOAnchorPos(visiblePosition, animationDuration).SetEase(Ease.OutBack);
    }

    private void HideShop()
    {
        shopPanel.DOAnchorPos(hiddenPosition, animationDuration).SetEase(Ease.InBack).OnComplete(Hide);

        void Hide()
        {
            shopUI.SetActive(false);
        }
    }
}
