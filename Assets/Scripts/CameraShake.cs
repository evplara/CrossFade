using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Coroutine currentShake;
    private Vector3 originalLocalPos;

    void Awake()
    {
        originalLocalPos = transform.localPosition;
    }

    public void StartShake(float duration, float magnitude)
    {
        // Stop any existing shake (prevents stacking bugs)
        if (currentShake != null)
        {
            StopCoroutine(currentShake);
            transform.localPosition = originalLocalPos; // hard reset
        }

        currentShake = StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;

        // Always use a stable base position
        originalLocalPos = transform.localPosition;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(
                originalLocalPos.x + x,
                originalLocalPos.y + y,
                originalLocalPos.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        // ✅ Guaranteed reset
        transform.localPosition = originalLocalPos;
        currentShake = null;
    }
}