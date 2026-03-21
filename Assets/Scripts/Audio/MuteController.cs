using UnityEngine;
using UnityEngine.UI;

namespace Audio
{
    public class AudioController : MonoBehaviour
    {
        [SerializeField] private Sprite audioOnIcon;
        [SerializeField] private Sprite audioOffIcon;
        [SerializeField] private Button muteButton;
        [SerializeField] private GameObject icon;
        private Image _buttonImage;
        private bool isMuted;

        private void Start()
        {
            _buttonImage = icon.GetComponent<Image>();
            muteButton.onClick.AddListener(ToggleMute);
            UpdateButtonIcon();
        }

        private void ToggleMute()
        {
            isMuted = !isMuted;
            AudioListener.volume = isMuted ? 0 : 1;
            UpdateButtonIcon();
        }

        private void UpdateButtonIcon()
        {
            _buttonImage.sprite = isMuted ? audioOffIcon : audioOnIcon;
        }
    }
}