using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager instance;

    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private Volume volume;
    [SerializeField] private GameObject ghostEffect;

    private float lethargyValue = 1f;
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

    //use this value to slow down actions: cursor speed in interview, how fast the car switches lanes in driving
    public void ActivateLethargy(float value)
    {
        lethargyValue = 1f - (value / 100f);
    }

    public void ActivateDizzy(float value)
    {
        cameraShake.StartShake(100, value/100);
    }

    public void ActivateHallucination(float value)
    {
        ghostEffect.SetActive(true);
        ghostEffect.GetComponent<GhostsEffect>().SetRates(value / 100);
    }

    public void ActivateFocus(float value)
    {
        bloom.active = true;
        bloom.intensity.value = value / 100;
    }

    public void ActiveNausea(float value)
    {
        vignette.active = true;
        vignette.intensity.value = value / 100;
    }

    public void ActiveHigh(float value)
    {
        colorAdjustments.active = true;
    }
}
