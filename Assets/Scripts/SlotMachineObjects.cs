using UnityEngine;

public class ManualSlowdown : MonoBehaviour
{
    public float startSpeed = 20f;
    public float deceleration = 40f;

    public float targetY = -5f;
    public float finalTargetY = -8f;
    public float resetY = 10f;

    public int maxLoops = 3;

    [Header("Sprites to cycle through")]
    public Sprite[] sprites;
    public float spriteChangeInterval = 0.1f;

    private float currentSpeed;
    private int loopCount = 0;
    private bool finalDrop = false;

    private SpriteRenderer sr;
    private float spriteTimer = 0f;
    private int spriteIndex = 0;

    void Start()
    {
        currentSpeed = startSpeed;
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Move downward
        currentSpeed -= deceleration * Time.deltaTime;
        currentSpeed = Mathf.Max(currentSpeed, 0f);

        transform.position += Vector3.down * currentSpeed * Time.deltaTime;

        // --- SPRITE SHUFFLE (only during looping phase) ---
        if (!finalDrop && sprites.Length > 0)
        {
            spriteTimer += Time.deltaTime;

            if (spriteTimer >= spriteChangeInterval)
            {
                spriteTimer = 0f;
                spriteIndex = (spriteIndex + 1) % sprites.Length;

                sr.sprite = sprites[Random.Range(0, sprites.Length)];
            }
        }

        // Decide target
        float currentTarget = finalDrop ? finalTargetY : targetY;

        if (transform.position.y <= currentTarget)
        {
            transform.position = new Vector3(
                transform.position.x,
                currentTarget,
                transform.position.z
            );

            if (!finalDrop)
            {
                loopCount++;

                if (loopCount >= maxLoops)
                {
                    finalDrop = true;

                    // LOCK FINAL SPRITE (stop shuffling permanently)
                    if (sprites.Length > 0)
                    {
                        sr.sprite = sprites[Random.Range(0, sprites.Length)];
                    }
                }

                transform.position = new Vector3(
                    transform.position.x,
                    resetY,
                    transform.position.z
                );

                currentSpeed = startSpeed;
            }
            else
            {
                currentSpeed = 0f;
                enabled = false;
            }
        }
    }
}