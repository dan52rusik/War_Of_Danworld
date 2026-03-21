using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform _mainCameraTransform;

    private void Start()
    {
        TryCacheMainCamera();
    }

    private void LateUpdate()
    {
        if (!TryCacheMainCamera())
            return;

        if (!IsFiniteVector3(_mainCameraTransform.forward) || !IsFiniteVector3(_mainCameraTransform.up))
            return;

        transform.rotation = Quaternion.LookRotation(-_mainCameraTransform.forward, _mainCameraTransform.up);
    }

    private bool TryCacheMainCamera()
    {
        if (_mainCameraTransform == null && Camera.main != null)
            _mainCameraTransform = Camera.main.transform;

        if (_mainCameraTransform == null)
            return false;

        var cameraForward = _mainCameraTransform.forward;
        var cameraUp = _mainCameraTransform.up;
        if (!IsFiniteVector3(cameraForward) || !IsFiniteVector3(cameraUp) ||
            cameraForward.sqrMagnitude <= Mathf.Epsilon || cameraUp.sqrMagnitude <= Mathf.Epsilon)
            return false;

        return true;
    }

    private static bool IsFiniteVector3(Vector3 value)
    {
        return !float.IsNaN(value.x) && !float.IsInfinity(value.x) &&
               !float.IsNaN(value.y) && !float.IsInfinity(value.y) &&
               !float.IsNaN(value.z) && !float.IsInfinity(value.z);
    }
}
