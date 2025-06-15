using UnityEngine;
using UnityEngine.InputSystem;

public class PaddleController : MonoBehaviour
{
    public static PaddleController Instance;

    [Header("Paddle Settings")]
    public Transform paddleParent;
    private const float moveSpeed = 10f;
    private float paddleWidth = 1.0f;

    private Camera mainCamera;
    private InputAction pointAction;
    private Vector3 targetPosition;

    void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;

        var inputActions = InputManager.Instance.inputActions;
        pointAction = inputActions.FindAction("Game/Point");
    }

    void OnEnable()
    {
        pointAction.Enable();
    }

    void OnDisable()
    {
        pointAction.Disable();
    }

    void Update()
    {
        // Get pointer position in world space
        Vector2 screenPos = pointAction.ReadValue<Vector2>();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
        worldPos.z = 0;

        // Only use the X position for paddle movement
        targetPosition = new Vector3(worldPos.x, paddleParent.position.y, 0);
        paddleParent.position = Vector3.Lerp(paddleParent.position, targetPosition, Time.deltaTime * moveSpeed);
    }
}
