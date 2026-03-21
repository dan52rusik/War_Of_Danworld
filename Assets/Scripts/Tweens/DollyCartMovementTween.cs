using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class DollyCartMovementTween : MonoBehaviour
{
    public CinemachineDollyCart dollyCart;
    [SerializeField] private float _duration = 5f; // Duration for a full loop of the track
    [SerializeField] private GameManager _gameManagerObject;
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = _gameManagerObject.GetComponent<GameManager>();
        _gameManager.OnGameStart.AddListener(StartTween);
        _gameManager.OnMainMenuButtonPressed.AddListener(EndTween);
    }

    private void StartTween()
    {
        // Ensure the m_Position starts at 0
        dollyCart.m_Position = 0f;

        // Tween the m_Position property
        DOTween.To(() => dollyCart.m_Position, x => dollyCart.m_Position = x, dollyCart.m_Path.PathLength, _duration)
            .SetEase(Ease.OutBack);
    }


    private void EndTween()
    {
        var initialPosition = 0;
        // Tween the m_Position property
        DOTween.To(() => dollyCart.m_Position, x => dollyCart.m_Position = x, initialPosition, _duration)
            .SetEase(Ease.OutBack);
    }
}