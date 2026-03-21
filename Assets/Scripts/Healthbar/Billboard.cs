using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private bool _useCameraPlaneInstead;
    private Plane _cameraPlane;

    private Transform _mainCameraTransform;

    private void Start()
    {
        if (Camera.main != null) _mainCameraTransform = Camera.main.transform;

        var cameraPosition = _mainCameraTransform.position;
        var planeNormal = -_mainCameraTransform.forward;
        _cameraPlane = new Plane(planeNormal, cameraPosition)
        {
            distance = 100000
        };
    }

    private void LateUpdate()
    {
        if (_useCameraPlaneInstead)
        {
            var nearestPoint = _cameraPlane.ClosestPointOnPlane(transform.position);
            transform.rotation = Quaternion.LookRotation(nearestPoint, Vector3.up);
        }
        else
        {
            var position = transform.position;
            transform.LookAt(position + (position - _mainCameraTransform.position).normalized);
        }
    }
}