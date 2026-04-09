using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [Header("Speed Settings")]
    public float minStartSpeed = 80f;
    public float maxStartSpeed = 140f;

    public float maxSpeed = 200f;
    public float acceleration = 50f;

    private float currentSpeed;
    private bool hasHit = false;

    void Start()
    {
        currentSpeed = Random.Range(minStartSpeed, maxStartSpeed);
    }

    void Update()
    {
        // Gradually increase speed
        currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);

        // Move toward player (negative Z direction)
        transform.Translate(Vector3.back * currentSpeed * Time.deltaTime);

        // Destroy when past player
        if (transform.position.z < -10f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player != null)
        {
            player.TakeDamage();
            hasHit = true;
        }
    }
}