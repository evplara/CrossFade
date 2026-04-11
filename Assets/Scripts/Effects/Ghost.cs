using System.IO;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    [System.Serializable]
    public struct GhostSpeed
    {
        public float minSpeed;
        public float maxSpeed;
    }

    [SerializeField] private GhostSpeed ghostSpeed;
    private float currentSpeed;

    private Vector2 direction = Vector2.right;

    public void Setup(bool isLeft)
    {
        currentSpeed = Random.Range(ghostSpeed.minSpeed, ghostSpeed.maxSpeed);
        if (isLeft) direction = Vector2.left;
    }

    private void Update()
    {
        transform.Translate(currentSpeed * direction * Time.deltaTime);
    }
}
