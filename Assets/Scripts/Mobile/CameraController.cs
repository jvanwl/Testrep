using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static CameraController _instance;
    public static CameraController Instance => _instance;

    [Header("Movement Settings")]
    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float smoothTime = 0.3f;
    [SerializeField] private float boundaryMargin = 50f;

    [Header("Zoom Limits")]
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 10f;

    [Header("Boundaries")]
    [SerializeField] private float minX = -100f;
    [SerializeField] private float maxX = 100f;
    [SerializeField] private float minY = -100f;
    [SerializeField] private float maxY = 100f;

    private Camera mainCamera;
    private Vector3 targetPosition;
    private Vector3 velocitySmoothing;
    private float targetZoom;
    private float zoomVelocity;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            mainCamera = GetComponent<Camera>();
            targetPosition = transform.position;
            targetZoom = mainCamera.orthographicSize;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        if (MobileInputManager.Instance != null)
        {
            MobileInputManager.Instance.OnTouchMove += HandleTouchMove;
            MobileInputManager.Instance.OnPinchZoom += HandlePinchZoom;
        }
    }

    private void OnDisable()
    {
        if (MobileInputManager.Instance != null)
        {
            MobileInputManager.Instance.OnTouchMove -= HandleTouchMove;
            MobileInputManager.Instance.OnPinchZoom -= HandlePinchZoom;
        }
    }

    private void HandleTouchMove(Vector2 delta)
    {
        // Convert screen movement to world space movement
        Vector3 adjustment = new Vector3(-delta.x, -delta.y, 0) * panSpeed * Time.deltaTime;
        targetPosition += adjustment;
    }

    private void HandlePinchZoom(float zoomDelta)
    {
        targetZoom = Mathf.Clamp(targetZoom - zoomDelta * zoomSpeed, minZoom, maxZoom);
    }

    private void Update()
    {
        UpdatePosition();
        UpdateZoom();
    }

    private void UpdatePosition()
    {
        // Clamp target position to boundaries
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        // Smoothly move camera
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocitySmoothing,
            smoothTime
        );
    }

    private void UpdateZoom()
    {
        mainCamera.orthographicSize = Mathf.SmoothDamp(
            mainCamera.orthographicSize,
            targetZoom,
            ref zoomVelocity,
            smoothTime
        );
    }

    public void FocusOn(Vector3 position)
    {
        targetPosition = new Vector3(position.x, position.y, transform.position.z);
    }

    public void SetZoom(float zoom)
    {
        targetZoom = Mathf.Clamp(zoom, minZoom, maxZoom);
    }

    public void SetBoundaries(float minX, float maxX, float minY, float maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
    }
}
