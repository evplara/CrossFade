using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*
 * PlayerHealth.cs — Car minigame damage bridge + VFX
 *
 * No longer owns HP. Forwards damage to HealthManager (global singleton).
 * Still handles the car-specific feedback: camera shake, damage sound, red flash.
 * CarMovement.cs still calls player.TakeDamage() on collision — no changes needed there.
 */

public class PlayerHealth : MonoBehaviour
{
    // how much HP each car hit takes from the global pool
    public int damagePerHit = 1;

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
        // Ensure overlay starts invisible
        if (damageOverlay != null)
        {
            Color c = damageOverlay.color;
            c.a = 0f;
            damageOverlay.color = c;
        }

        // listen for death so this scene can react
        if (HealthManager.Instance != null)
        {
            HealthManager.Instance.PlayerDied += OnPlayerDied;
        }
    }

    private void OnDestroy()
    {
        if (HealthManager.Instance != null)
        {
            HealthManager.Instance.PlayerDied -= OnPlayerDied;
        }
    }

    // CarMovement still calls this on collision — we just forward it now
    public void TakeDamage()
    {
        if (HealthManager.Instance != null)
        {
            HealthManager.Instance.TakeDamage(damagePerHit);
        }

        // scene-local VFX
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

    void OnPlayerDied()
    {
        Debug.Log("[PlayerHealth] Global health hit 0.");
        // probably go back to gacha area or game over screen
    }
}
