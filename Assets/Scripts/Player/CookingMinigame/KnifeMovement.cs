using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class KnifeMovement : MonoBehaviour
{

    // General variables

    [SerializeField] public float speed = 10f;
    [SerializeField] public float acceletation = 5f; 
    
    public float sliceDelay = 1;
    private bool sliceIsExecuting = false;

    // Knife variables

    public GameObject blade;
    private Rigidbody rigidBody;
    private Vector3 direction;

    // Blade variables

    private BoxCollider boxCollider;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        boxCollider = blade.GetComponent<BoxCollider>();

        direction = new Vector3(0, 0, 1).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        Mouse mouse = Mouse.current;

        if (mouse.leftButton.wasPressedThisFrame && !sliceIsExecuting)
        {
            StartCoroutine(Slice());
        }
    }

    void FixedUpdate()
    {
        if (sliceIsExecuting)
        {
            Vector3 newPosition = rigidBody.position + direction * (speed * Time.fixedDeltaTime);
            rigidBody.MovePosition(newPosition);
        }
    }

    IEnumerator Slice()
    {
        sliceIsExecuting = true;

        yield return new WaitUntil(IsColliding);

        Debug.Log("Yes!");

        sliceIsExecuting = false;
    }

    void IsColliding()
    {
        if ()
    }

    void OnCollisionEnter(Collider other)
    {
        if (other.CompareTag("Plane"))
        {
            Debug.Log("Slice missed!");
        }
    }
}
