using UnityEngine;

public class bgmusicpitch : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Interval Settings")]
public float minInterval = 8f;
public float maxInterval = 14f;

private float timer;

    [Header("Pitch Ranges")]
    public float lowMin = 0.75f;
    public float lowMax = 0.9f;
    public float highMin = 1.1f;
    public float highMax = 1.25f;

    [Header("Lerp Settings")]
    public float pitchLerpSpeed = 1.5f;

    private bool lastWasHigh = false;
    private float targetPitch;

    void Start()
{
    audioSource = GetComponent<AudioSource>();
    ResetTimer();

    targetPitch = audioSource.pitch;
    lastWasHigh = targetPitch > 1f;
}

    void Update()
{
    timer -= Time.deltaTime;

    if (timer <= 0f)
    {
        PickNewTargetPitch();
        ResetTimer();
    }

    // Smooth pitch movement
    audioSource.pitch = Mathf.MoveTowards(
        audioSource.pitch,
        targetPitch,
        pitchLerpSpeed * Time.deltaTime
    );
}

    void PickNewTargetPitch()
    {
        if (lastWasHigh)
        {
            targetPitch = Random.Range(lowMin, lowMax);
        }
        else
        {
            targetPitch = Random.Range(highMin, highMax);
        }

        lastWasHigh = targetPitch > 1f;
    }
    void ResetTimer()
{
    timer = Random.Range(minInterval, maxInterval);
}
}