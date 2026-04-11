using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager instance;

    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private Volume volume;
    [SerializeField] private GameObject ghostEffect;

    private float lethargyValue;
    public float LethargyValue => lethargyValue;

    private Vignette vignette;
    private Bloom bloom;
    private ColorAdjustments colorAdjustments;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out bloom);
        volume.profile.TryGet(out colorAdjustments);
    }

    //TODO: actually make the value, affect the effect


    //use this value to slow down actions: cursor speed in interview, how fast the car switches lanes in driving
    public void ActivateLethargy(float value)
    {
        lethargyValue = value;
    }

    public void ActivateDizzy(float value)
    {
        cameraShake.StartShake(100, 0.6f);
    }

    public void ActivateHallucination(float value)
    {
        ghostEffect.SetActive(true);
    }

    public void ActivateFocus(float value)
    {
        bloom.active = true;
    }

    public void ActiveNausea(float value)
    {
        vignette.active = true;
    }

    public void ActiveHigh(float value)
    {
        colorAdjustments.active = true;
    }
}
