using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PaddleController : MonoBehaviour
{
    public static PaddleController Instance;

    private bool isLocked = false;
    private float lockTimer = 0f;

    [Header("Combo System")]
    [SerializeField] private GameObject[] comboTexts; // Assign in Inspector: [combox1, combox2, ..., combox5]
    private int currentCombo = 0;
    [SerializeField] private ParticleSystem explosionEffect;

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

        if (isLocked)
        {
            lockTimer -= Time.deltaTime;
            if (lockTimer <= 0f)
            {
                isLocked = false;
            }
            return; // Skip paddle movement
        }
        // Get pointer position in world space
        Vector2 screenPos = pointAction.ReadValue<Vector2>();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
        worldPos.z = 0;

        // Only use the X position for paddle movement
        targetPosition = new Vector3(worldPos.x, paddleParent.position.y, 0);
        paddleParent.position = Vector3.Lerp(paddleParent.position, targetPosition, Time.deltaTime * moveSpeed);
    }

    public void RegisterHit()
    {
        currentCombo++;


        if (currentCombo % 2 == 0)
        {
            int pairCount = currentCombo / 2;

            if (pairCount >= 2)
            {
                int index = Mathf.Clamp(pairCount - 2, 0, comboTexts.Length - 1);

                for (int i = 0; i < comboTexts.Length; i++)
                {
                    bool isActive = i == index;
                    comboTexts[i].SetActive(isActive);

                    if (isActive)
                    {
                        // Start the pulse effect on this combo text
                        StartCoroutine(PulseTextCoroutine(comboTexts[i].transform));
                    }
                }

                explosionEffect.Play();
            }
        }
    }

    public void RegisterMiss()
    {
        ResetCombo();
    }

    private void ResetCombo()
    {
        currentCombo = 0;

        // Hide all combo texts
        foreach (var text in comboTexts)
        {
            text.SetActive(false);
        }
    }

    private IEnumerator PulseTextCoroutine(Transform textTransform, float duration = 0.3f, float maxScale = 1.5f)
    {
        Vector3 originalScale = textTransform.localScale;
        float timer = 0f;

        while (timer < duration)
        {
            float t = timer / duration;
            float scale = Mathf.Lerp(maxScale, 1f, t); // shrink back to original
            textTransform.localScale = originalScale * scale;

            timer += Time.deltaTime;
            yield return null;
        }

        textTransform.localScale = originalScale; // Ensure it ends at normal size
    }

    public void SetPaddleWidth(float width)
    {
        paddleWidth = width;

        Vector3 scale = paddleParent.localScale;
        scale.x = width;
        paddleParent.localScale = scale;
    }

    public void LockPaddle(float duration)
    {
        isLocked = true;
        lockTimer = duration;
    }

}
