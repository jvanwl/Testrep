using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class MobileInputManager : MonoBehaviour
{
    private static MobileInputManager _instance;
    public static MobileInputManager Instance => _instance;

    public event Action<Vector2> OnTouchStart;
    public event Action<Vector2> OnTouchMove;
    public event Action<Vector2> OnTouchEnd;
    public event Action<float> OnPinchZoom;
    
    private Vector2 touchStart;
    private Vector2 previousTouchPosition;
    private bool isDragging;
    private float previousPinchDistance;

    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 10f;
    [SerializeField] private float dragThreshold = 10f;
    [SerializeField] private float doubleTapTime = 0.3f;
    
    private float lastTapTime;
    private Vector2 lastTapPosition;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        // Handle single touch for selection and dragging
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    HandleTouchBegan(touch);
                    break;

                case TouchPhase.Moved:
                    HandleTouchMoved(touch);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    HandleTouchEnded(touch);
                    break;
            }
        }
        // Handle pinch to zoom with two fingers
        else if (Input.touchCount == 2)
        {
            HandlePinchZoom();
        }
    }

    private void HandleTouchBegan(Touch touch)
    {
        // Ignore touches that started on UI elements
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            return;

        touchStart = touch.position;
        previousTouchPosition = touch.position;
        isDragging = false;

        // Check for double tap
        if (Time.time - lastTapTime < doubleTapTime &&
            Vector2.Distance(lastTapPosition, touch.position) < dragThreshold)
        {
            HandleDoubleTap(touch.position);
        }

        lastTapTime = Time.time;
        lastTapPosition = touch.position;

        OnTouchStart?.Invoke(touch.position);
    }

    private void HandleTouchMoved(Touch touch)
    {
        if (!isDragging)
        {
            if (Vector2.Distance(touch.position, touchStart) > dragThreshold)
            {
                isDragging = true;
            }
        }

        if (isDragging)
        {
            Vector2 delta = touch.position - previousTouchPosition;
            OnTouchMove?.Invoke(delta);
        }

        previousTouchPosition = touch.position;
    }

    private void HandleTouchEnded(Touch touch)
    {
        if (!isDragging && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            // Handle tap selection
            HandleTapSelection(touch.position);
        }

        OnTouchEnd?.Invoke(touch.position);
    }

    private void HandlePinchZoom()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
        {
            previousPinchDistance = Vector2.Distance(touch1.position, touch2.position);
        }
        else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
        {
            float currentPinchDistance = Vector2.Distance(touch1.position, touch2.position);
            float pinchDelta = currentPinchDistance - previousPinchDistance;
            
            OnPinchZoom?.Invoke(pinchDelta * 0.01f);
            previousPinchDistance = currentPinchDistance;
        }
    }

    private void HandleDoubleTap(Vector2 position)
    {
        // Convert screen position to world position
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            // Focus on the tapped object
            CameraController.Instance?.FocusOn(hit.collider.transform.position);
        }
    }

    private void HandleTapSelection(Vector2 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            // Handle selection of units, buildings, or UI elements
            ISelectable selectable = hit.collider.GetComponent<ISelectable>();
            selectable?.OnSelect();
        }
    }

    public bool IsPointerOverUI()
    {
        if (Input.touchCount > 0)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }
        return false;
    }
}
