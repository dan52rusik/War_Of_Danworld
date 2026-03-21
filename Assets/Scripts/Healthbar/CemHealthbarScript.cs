// using DG.Tweening;
// using TMPro;
// using Unity.VisualScripting;
// using UnityEngine;
// using UnityEngine.UI;
//
// public class HealthVisualizer : MonoBehaviour
// {
//     [Header("References")]
//     [SerializeField] private Image _outerBackground;
//     [SerializeField] private Image _innerBackground;
//     [SerializeField] private Image _bufferImage;
//     [SerializeField] private Image _foregroundImage;
//
//     [Header("Config")] 
//     [SerializeField] private float _bufferWaitTime = 0.25f;
//     [SerializeField] private float _bufferLerpDuration = 0.1f;
//     [SerializeField] private TMP_Text _damageTextPrefab;
//     [SerializeField] private float _damageTextInitialY = 50f;
//     [SerializeField] private float _damageTextUpAmount = 100f;
//     [SerializeField] private float _damageTextFadeTime = 0.3f;
//     [SerializeField] private float _damageTextFadeDelay = 0.5f;
//     [SerializeField] private float _damageTextScaleTime = 0.3f;
//     [SerializeField] private float _damageTextScaleAmount = 1;
//
//     private HealthModule _healthModule;
//     private Transform _mainCameraTransform;
//     private float _bufferTimer;
//     private bool _isBufferingCompleted = true;
//     private Tween _bufferTween;
//     private Plane _cameraPlane;
//    
//     private void Start()
//     {
//         _mainCameraTransform = Camera.main.transform;
//         
//         var cameraPosition = _mainCameraTransform.position;
//         var planeNormal = -_mainCameraTransform.forward;
//         _cameraPlane = new Plane(planeNormal, cameraPosition)
//         {
//             distance = 100000
//         };
//
//         _healthModule = GetComponentInParent<HealthModule>();
//         _healthModule.OnCurrentHPChanged.AddListener(RefreshHealthBarWithCurrentHP);
//         _healthModule.OnDamageTaken += ShowDamageText;
//         SetActive(false); // This kinda assumes everyone starts with full health
//     }
//
//     private void ShowDamageText(CharacterMain character, float damage, bool isCritical)
//     {
//         if (_damageTextPrefab)
//         {
//             var damageText = Instantiate(_damageTextPrefab, transform, false);
//             if (isCritical)
//             {
//                 damageText.color = new Color(1f,121f/256f,131f/256f);
//             }
//
//             damageText.text = Mathf.CeilToInt(damage) + (isCritical ? "!":"");
//             damageText.rectTransform.localRotation = Quaternion.Euler(0, 180, 0);
//             damageText.rectTransform.anchoredPosition = Vector2.up * _damageTextInitialY;
//             damageText.rectTransform.localScale = Vector3.zero;
//             damageText.rectTransform.DOScale(_damageTextScaleAmount, _damageTextScaleTime).SetEase(Ease.OutBack);
//             damageText.rectTransform.DOAnchorPosY(damageText.rectTransform.anchoredPosition.y + _damageTextUpAmount, _damageTextFadeTime).SetDelay(_damageTextFadeDelay);
//             damageText.DOFade(0, _damageTextFadeTime).SetDelay(_damageTextFadeDelay).OnComplete(()=> Destroy(damageText.gameObject,0.5f));
//         }
//     }
//
//     private void OnDestroy()
//     {
//         _bufferTween?.Kill();
// 		if (_healthModule)
// 		{
// 			_healthModule.OnCurrentHPChanged.RemoveListener(RefreshHealthBarWithCurrentHP);
// 			_healthModule.OnDamageTaken -= ShowDamageText;
// 		}
//     }
//
//     private void RefreshHealthBarWithCurrentHP(float currentHP) => SetHealthBarPercentage(currentHP / _healthModule.GetMaxHP());
//
//     private void SetHealthBarPercentage(float percentage)
//     {
//         _foregroundImage.fillAmount = percentage;
//         
//         _bufferTimer = _bufferWaitTime;
//         _isBufferingCompleted = false;
//         
//         SetActive(percentage < 1 - double.Epsilon && percentage > 0);
//     }
//     
//     private void LateUpdate()
//     {
//         var nearestPoint = _cameraPlane.ClosestPointOnPlane(transform.position);
//         transform.rotation = Quaternion.LookRotation(-nearestPoint, Vector3.up);
//
//         if (_bufferTimer < 0 && !_isBufferingCompleted)
//         {
//             _bufferTween?.Kill();
//             
//             _bufferTween = DOTween.To(
//                 () => _bufferImage.fillAmount,
//                 x => _bufferImage.fillAmount = x,
//                 _foregroundImage.fillAmount,
//                 _bufferLerpDuration);
//             
//             _isBufferingCompleted = true;
//         }
//         else
//         {
//             _bufferTimer -= Time.deltaTime;
//         }
//     }
//
//     public void SetActive(bool active) => _outerBackground.gameObject.SetActive(active);
//
//     public void SetHealthBarSprites(TeamConfig teamConfig)
//     {
//         _outerBackground.sprite = teamConfig.HealthBarBG;
//         _innerBackground.sprite = teamConfig.HealthBarBGInside;
//         _foregroundImage.color = teamConfig.TeamColor;
//     }
// }
