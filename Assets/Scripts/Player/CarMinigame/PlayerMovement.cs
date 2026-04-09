using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float laneDistance = 5f;

    private Rigidbody rb;

    public GameObject turningObject;
    private Animator turningAnimator;
    private SpriteRenderer turningSprite;

    public CameraShake cameraShake;

    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.2f;

    [Header("Tilt Settings")]
    public float tiltAngle = 10f;       
    public float tiltSpeed = 10f;       
    private float targetZRotation = 0f;

    private int currentLane = 0;
    private int targetLane = 0;

    public AudioSource audioSource;
    public AudioClip turnSound;

    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    private bool isMoving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (turningObject != null)
        {
            turningAnimator = turningObject.GetComponent<Animator>();
            turningSprite = turningObject.GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (isMoving) return;

        float input = Input.GetAxisRaw("Horizontal");

        if (input != 0)
        {
            int direction = (int)Mathf.Sign(input);
            int newLane = Mathf.Clamp(currentLane + direction, -1, 1);

            if (newLane != currentLane)
            {
                targetLane = newLane;
                StartTurn(direction);
                isMoving = true;
            }
        }
    }

    void FixedUpdate()
{
    // --- MOVEMENT ---
    if (isMoving)
    {
        float targetX = targetLane * laneDistance;

        Vector3 currentPos = rb.position;
        float newX = Mathf.MoveTowards(currentPos.x, targetX, moveSpeed * Time.fixedDeltaTime);

        rb.MovePosition(new Vector3(newX, currentPos.y, currentPos.z));

        // Reached lane
        if (Mathf.Abs(newX - targetX) < 0.01f)
        {
            rb.MovePosition(new Vector3(targetX, currentPos.y, currentPos.z));
            currentLane = targetLane;
            isMoving = false;

            EndTurn();
        }
    }

    // --- ROTATION (ALWAYS RUNS) ---
    float currentZ = transform.eulerAngles.z;

    // Convert to -180 to 180
    if (currentZ > 180f) currentZ -= 360f;

    float target = isMoving ? targetZRotation : 0f; // force return to 0 when not moving

    float newZ = Mathf.Lerp(currentZ, target, tiltSpeed * Time.fixedDeltaTime);

    // Snap to 0 when very close (prevents tiny drift forever)
    if (!isMoving && Mathf.Abs(newZ) < 0.1f)
        newZ = 0f;

    transform.rotation = Quaternion.Euler(0f, 0f, newZ);
}

    void StartTurn(int direction)
{
    if (turningAnimator != null)
    {
        turningAnimator.SetBool("Turning", true);
    }

    if (turningSprite != null)
    {
        turningSprite.flipX = (direction == 1);
    }

    // Tilt opposite direction for natural feel
    targetZRotation = -direction * tiltAngle;

    if (cameraShake != null)
    {
        cameraShake.StartShake(shakeDuration, shakeMagnitude);
    }

    if (audioSource != null && turnSound != null)
    {
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(turnSound);
    }
}

    void EndTurn()
{
    if (turningAnimator != null)
    {
        turningAnimator.SetBool("Turning", false);
    }

    // Reset tilt
    targetZRotation = 0f;
}
}