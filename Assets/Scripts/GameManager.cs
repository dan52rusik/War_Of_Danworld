using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public UnityEvent<Castle> OnGameOver;
    public UnityEvent OnGameStart;
    public UnityEvent OnMainMenuButtonPressed;
    [SerializeField] private GameObject _startButtonObject;
    [SerializeField] private GameObject _exitButton;
    [SerializeField] private GameObject _mainMenuButton;
    [SerializeField] private GameObject _giveUpButton;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip _winAudio;
    [SerializeField] private AudioClip _defeatAudio;
    [SerializeField] private AudioClip _introAudio;
    [SerializeField] private AudioClip _warStartAudio;
    [SerializeField] private AudioClip _birdChirping;


    [SerializeField] private GameObject victoryText;

    [SerializeField] private GameObject defeatText;

    [SerializeField] public bool GameOver;
    private Castle _allyCastle;
    private Castle _enemyCastle;
    private Button _startButton;

    // Start is called before the first frame update
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = _introAudio;
        audioSource.Play();


        _allyCastle = GameObject.FindWithTag("AllyBase").GetComponent<Castle>();
        _enemyCastle = GameObject.FindWithTag("EnemyBase").GetComponent<Castle>();
        _mainMenuButton.SetActive(false);
        _startButtonObject.SetActive(true);
        _startButton = _startButtonObject.GetComponentInChildren<Button>();
        _startButton.onClick.AddListener(StartGame);
        _giveUpButton.GetComponentInChildren<Button>().onClick.AddListener(GiveUpButtonPressed);
        _exitButton.GetComponentInChildren<Button>().onClick.AddListener(Quit);
        _giveUpButton.SetActive(false);
        _mainMenuButton.GetComponentInChildren<Button>().onClick.AddListener(MainMenuButtonPressed);
    }

    private void Quit()
    {
        // If we are running in a standalone build of the game
#if UNITY_STANDALONE
        // Quit the application
        Application.Quit();
#endif

        // If we are running in the editor
#if UNITY_EDITOR
        // Stop playing the scene
        EditorApplication.isPlaying = false;
#endif
    }

    private void GiveUpButtonPressed()
    {
        _startButton.interactable = true;
        audioSource.loop = false;
        audioSource.Stop();
        _giveUpButton.SetActive(false);
        GameOver = true;
        OnGameOver.Invoke(null);
        OnMainMenuButtonPressed.Invoke();
    }

    private void MainMenuButtonPressed()
    {
        _startButton.interactable = true;
        _mainMenuButton.SetActive(false);
        victoryText.SetActive(false);
        defeatText.SetActive(false);
        OnMainMenuButtonPressed.Invoke();
    }

    private void ShowEndGameComponents(Castle defeatedCastle)
    {
        if (defeatedCastle.isEnemy)
        {
            victoryText.SetActive(true);
            victoryText.GetComponentInChildren<TMP_Text>().text =
                "Victory";
        }
        else
        {
            defeatText.SetActive(true);
            defeatText.GetComponentInChildren<TMP_Text>().text =
                "Defeat";
        }
    }

    private void StartGame()
    {
        _startButtonObject.GetComponentInChildren<Button>().interactable = false;
        audioSource.Stop();
        audioSource.clip = _warStartAudio;
        audioSource.Play();
        StartCoroutine(BirdChirping());

        GameOver = false;
        _enemyCastle.onDestroy.AddListener(x =>
        {
            GameOver = true;
            audioSource.loop = false;
            audioSource.clip = _winAudio;
            audioSource.Play();
            _mainMenuButton.SetActive(true);
            OnGameOver.Invoke(_enemyCastle);
            _giveUpButton.SetActive(false);
            ShowEndGameComponents(_enemyCastle);
        });
        _allyCastle.onDestroy.AddListener(x =>
        {
            GameOver = true;
            audioSource.loop = false;
            audioSource.clip = _defeatAudio;
            audioSource.Play();
            _mainMenuButton.SetActive(true);
            OnGameOver.Invoke(_allyCastle);
            _giveUpButton.SetActive(false);
            ShowEndGameComponents(_allyCastle);
        });
        OnGameStart.Invoke();
    }

    private IEnumerator BirdChirping()
    {
        yield return new WaitForSeconds(2);
        audioSource.clip = _birdChirping;
        audioSource.loop = true;
        audioSource.Play();
        _giveUpButton.SetActive(true);
    }
}