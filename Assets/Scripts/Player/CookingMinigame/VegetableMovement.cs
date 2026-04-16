using UnityEngine;

public class VegetableMovement : MonoBehaviour
{
    [SerializeField] public float speed = 10f;

    private Rigidbody rigidBody;
    private Vector3 direction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        direction = new Vector3(1, 0, 0).normalized;

    }

    void FixedUpdate()
    {
        Vector3 newPosition = rigidBody.position + direction * (speed * Time.fixedDeltaTime);

        rigidBody.MovePosition(newPosition);
    }

    void OnSlice(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Sliced!");
        }
    }
}
