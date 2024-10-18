using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineConfiner2D _confiner2d;
    private float _minDistance = 5;
    private float _maxDistance = 15;
    private float _distance;

    public float Distance
    {
        get => _distance;
    }

    public void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _confiner2d = GetComponent<CinemachineConfiner2D>();
        SetDistance(_virtualCamera.m_Lens.OrthographicSize);
    }

    public void Init(MapController mapController)
    {
        _confiner2d.m_BoundingShape2D = mapController.PolygonCollider;
        mapController.MapChanged += _ => _confiner2d.InvalidateCache();
    }

    public void SetFollowTarget(Transform transform)
    {
        _virtualCamera.Follow = transform;
    }

    public void SetLookAtTarget(Transform transform)
    {
        _virtualCamera.LookAt = transform;
    }

    public void SetDistance(float distance)
    {
        distance = Mathf.Clamp(distance, _minDistance, _maxDistance);
        _virtualCamera.m_Lens.OrthographicSize = distance;
        _distance = distance;
        _confiner2d.InvalidateCache();
    }
}
