using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarTween : MonoBehaviour
{
    public Slider healthSlider;
    [SerializeField] private GameObject laggingHealthbar;
    [SerializeField] private float _fillDuration = 3f;
    [SerializeField] private float _rotationDuration = 1;

    [SerializeField] private GameManager _gameManagerObject;
    private GameManager _gameManager;

    private void Start()
    {
        transform.eulerAngles = new Vector3(90, transform.eulerAngles.x, transform.eulerAngles.z);
        laggingHealthbar.SetActive(false);
        _gameManager = _gameManagerObject.GetComponent<GameManager>();
        _gameManager.OnGameStart.AddListener(StartTween);
        _gameManager.OnGameOver.AddListener(x => EndTween());
    }

    private void StartTween()
    {
        Vector3 targetRotation = new(0f, 0f, 0f);
        healthSlider.value = 0;
        transform.eulerAngles = new Vector3(90, transform.eulerAngles.x, transform.eulerAngles.z);
        var rotationTween = transform.DORotate(targetRotation, _rotationDuration).SetEase(Ease.OutSine);
        rotationTween.OnComplete(FillTween);
    }

    private void EndTween()
    {
        Vector3 targetRotation = new(0f, 90f, 0f);
        transform.DORotate(targetRotation, _rotationDuration).SetEase(Ease.OutSine);
        laggingHealthbar.SetActive(false);
    }

    private void FillTween()
    {
        healthSlider.DOValue(1, _fillDuration).SetEase(Ease.OutSine).OnComplete(ActivateLaggingSlider);
    }

    private void ActivateLaggingSlider()
    {
        laggingHealthbar.SetActive(true);
    }
}