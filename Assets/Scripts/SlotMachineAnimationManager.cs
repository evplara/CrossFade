using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnimationManager : MonoBehaviour
{
    [Header("UI Button (will be destroyed)")]
    public Button slotButton;

    [Header("Animators to control")]
    public Animator[] animators;

    [Header("Trigger name")]
    public string triggerName = "SlotMachinePull";

    [Header("Slot Prefabs (3 required)")]
    public GameObject[] slotPrefabs;

    [Header("Spawn settings")]
    public float spawnY = 0f;
    public float spawnZ = 0f;

    [Header("Delay before spawning")]
    public float spawnDelay = 0.5f;

    private float[] xPositions = new float[] { -0.37f, 0f, 0.37f };

    [Header("Audio")]
    public AudioSource audioSource;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySlotMachine()
    {
        // Disable button immediately
        if (slotButton != null)
        {
            slotButton.interactable = false;
        }

        if (audioSource != null)
        {
            audioSource.Play();
        }

        // Trigger animations
        foreach (Animator anim in animators)
        {
            if (anim != null)
            {
                anim.ResetTrigger(triggerName);
                anim.SetTrigger(triggerName);
            }
        }

        StartCoroutine(SpawnAndDestroyButton());
    }

    private IEnumerator SpawnAndDestroyButton()
    {
        // Wait before spawn
        yield return new WaitForSeconds(spawnDelay);

        // Safety check
        if (slotPrefabs.Length < 3)
        {
            Debug.LogWarning("Assign 3 prefabs in slotPrefabs!");
            yield break;
        }

        // Spawn all 3 slots
        for (int i = 0; i < 3; i++)
        {
            Vector3 spawnPos = new Vector3(xPositions[i], spawnY, spawnZ);
            Instantiate(slotPrefabs[i], spawnPos, Quaternion.identity);
        }

        // Wait after spawn
        yield return new WaitForSeconds(1f);

        // Destroy the button GameObject
        if (slotButton != null)
        {
            Destroy(slotButton.gameObject);
        }
    }
}