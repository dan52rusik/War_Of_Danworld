using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Tweens
{
    public class MainMenuTween : MonoBehaviour
    {
        public TMP_Text gameTitle; // The Text component to animate
        public float targetFontSize = 30f; // The target font size to animate to
        public float fontSizeAnimationDuration = 1f; // The duration of the animation
        public float menuButtonsAnimationDuration = 0.5f;
        [SerializeField] private GameObject menuButtons;
        [SerializeField] private RectTransform menuButtonRectTransform;
        [SerializeField] private GameObject _gameManagerObject;
        private GameManager _gameManager;
        private Vector2 hiddenPosition;
        private Vector2 visiblePosition;

        private void Start()
        {
            menuButtonRectTransform = menuButtons.GetComponent<RectTransform>();
            visiblePosition = menuButtonRectTransform.anchoredPosition;
            hiddenPosition = new Vector2(menuButtonRectTransform.anchoredPosition.x,
                -(Screen.height + menuButtonRectTransform.rect.height * 2));
            menuButtonRectTransform.anchoredPosition = hiddenPosition;

            _gameManager = _gameManagerObject.GetComponent<GameManager>();
            _gameManager.OnGameStart.AddListener(HideMenu);
            _gameManager.OnMainMenuButtonPressed.AddListener(ShowMenuButtons);

            AnimateFontSize();
            ShowMenuButtons();
        }


        // Method to animate the font size
        private void AnimateFontSize()
        {
            gameTitle.fontSize = 0;
            if (gameTitle != null)
                DOTween.To(() => gameTitle.fontSize, x => gameTitle.fontSize = x, targetFontSize,
                    fontSizeAnimationDuration).SetEase(Ease.OutBack);
        }

        private void ShowMenuButtons()
        {
            menuButtons.SetActive(true);
            menuButtonRectTransform.DOAnchorPos(visiblePosition, menuButtonsAnimationDuration).SetEase(Ease.OutBack);
        }

        private void HideMenu()
        {
            menuButtonRectTransform.DOAnchorPos(hiddenPosition, menuButtonsAnimationDuration).SetEase(Ease.InBack)
                .OnComplete(Hide);

            void Hide()
            {
                menuButtons.SetActive(false);
            }
        }
    }
}