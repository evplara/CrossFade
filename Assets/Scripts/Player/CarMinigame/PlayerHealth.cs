using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    // 🎥 Camera shake
    public CameraShake cameraShake;
    public float shakeDuration = 0.15f;
    public float shakeMagnitude = 0.3f;

    // 🔊 Sound (external AudioSource)
    public AudioSource damageAudioSource;
    public AudioClip damageSound;

    // 🟥 Red overlay (UI Image)
    public Image damageOverlay;
    public float flashDuration = 0.2f;
    public float maxAlpha = 0.5f;

    void Start()
    {
        currentHealth = maxHealth;

        // Ensure overlay starts invisible
        if (damageOverlay != null)
        {
            Color c = damageOverlay.color;
            c.a = 0f;
            damageOverlay.color = c;
        }
    }

    public void TakeDamage()
    {
        currentHealth -= 1;
        Debug.Log("Player took damage! Current health: " + currentHealth);


        if (cameraShake != null)
        {
            cameraShake.StartShake(shakeDuration, shakeMagnitude);
        }

        if (damageAudioSource != null && damageSound != null)
        {
            damageAudioSource.PlayOneShot(damageSound);
        }

        if (damageOverlay != null)
        {
            StopCoroutine("FlashRed");
            StartCoroutine(FlashRed());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashRed() //used copilot to help with this segment
    {
        Color c = damageOverlay.color;

        // Fade in
        float t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, maxAlpha, t / flashDuration); //copilot auto complete
            damageOverlay.color = c;
            yield return null;
        }

        // Fade out
        t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(maxAlpha, 0f, t / flashDuration);
            damageOverlay.color = c;
            yield return null;
        }

        c.a = 0f;
        damageOverlay.color = c;
    }

    void Die()
    {
        Debug.Log("Player Died");
        // probably go back to gacha area
    }
}