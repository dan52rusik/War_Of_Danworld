using UnityEngine;
using UnityEngine.UI;

public class ClickSoundPlayer : MonoBehaviour
{
    public AudioClip clickSound; // The click sound effect
    private AudioSource audioSource;

    void Awake()
    {
        // Add an AudioSource component to this GameObject if it doesn't already have one
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clickSound;
    }

    void Start()
    {
        // Find all buttons in the scene
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();

        foreach (Button button in buttons)
        {
            // Add the PlayClickSound method to each button's onClick event
            button.onClick.AddListener(PlayClickSound);
        }
    }

    // Method to play the click sound
    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}