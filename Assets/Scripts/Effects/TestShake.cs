using UnityEngine;

public class TestShake : MonoBehaviour
{
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private float magnitude;

    private void Start()
    {
        cameraShake.StartShake(100, magnitude);
    }
}
