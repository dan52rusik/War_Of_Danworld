using System.Collections;
using DG.Tweening;
using UnityEngine;

public class SpawnButtonTween : MonoBehaviour
{
    [SerializeField] private GameObject _gameManagerObject;
    [SerializeField] private GameObject _spawnButtonsGroup;
    [SerializeField] private float _rotationDuration = 1;
    private GameManager _gameManager;


    private void Start()
    {
        _spawnButtonsGroup.SetActive(false);
        _gameManager = _gameManagerObject.GetComponent<GameManager>();
        _gameManager.OnGameStart.AddListener(ActivateSpawnButtons);
        _gameManager.OnGameOver.AddListener(x => EndTween());
    }

    private void ActivateSpawnButtons()
    {
        StartCoroutine(SpawnButtonsShowDelay());
    }

    private IEnumerator SpawnButtonsShowDelay()
    {
        Vector3 targetRotation = new(0f, 0f, 0f);
        yield return new WaitForSeconds(2);
        _spawnButtonsGroup.SetActive(true);
        _spawnButtonsGroup.transform.eulerAngles = new Vector3(-90, transform.eulerAngles.x, transform.eulerAngles.z);
        _spawnButtonsGroup.transform.DORotate(targetRotation, _rotationDuration).SetEase(Ease.OutSine);
    }

    private void EndTween()
    {
        Vector3 targetRotation = new(-90f, 0f, 0f);
        _spawnButtonsGroup.transform.DORotate(targetRotation, _rotationDuration).SetEase(Ease.OutSine);
    }
}