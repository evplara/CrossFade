using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class KnifeMovement : MonoBehaviour
{

    // General variables

    [SerializeField] public float speed = 10f;
    [SerializeField] public float ascceletation = 5f; 
    private Vector3 resetPosition;

    private bool isColliding = false;
    
    public float sliceDelay = 1;
    private bool sliceIsExecuting = false;
    private bool knifeIsResetting = false;

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
        resetPosition = transform.position;

        direction = new Vector3(0, 0, 1).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        Mouse mouse = Mouse.current;

        if (mouse.leftButton.wasPressedThisFrame && !sliceIsExecuting && !knifeIsResetting)
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
        else if (knifeIsResetting)
        {
            Vector3 newPosition = rigidBody.position - direction * (speed * Time.fixedDeltaTime);
            rigidBody.MovePosition(newPosition);
        }

    }

    IEnumerator Slice()
    {
        sliceIsExecuting = true;

        yield return new WaitUntil(IsColliding);

        Debug.Log("Slice!");

        sliceIsExecuting = false;
        isColliding = false;

        StartCoroutine(ResetKnife());
    }

    IEnumerator ResetKnife()
    {
        knifeIsResetting = true;

        yield return new WaitUntil(KnifeReset);

        Debug.Log("Knife reset!");

        knifeIsResetting = false;
    }

    bool IsColliding()
    {
        return isColliding;
    }

    bool KnifeReset()
    {
        return transform.position == resetPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plane"))
        {
            isColliding = true;
            Debug.Log("Slice missed!");
        }

        if (other.CompareTag("Enemy"))
        {
            isColliding = true;
            Debug.Log("Slice hit!");
        }
    }
}
