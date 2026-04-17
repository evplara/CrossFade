using CrossFade.Potions;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager instance;

    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private Volume volume;
    [SerializeField] private GameObject ghostEffect;

    [Header("Vignette (nausea + green-out)")]
    [SerializeField] private float nauseaVignetteMaxIntensity = 0.45f;
    [SerializeField] private float greenVignetteMaxIntensity = 0.58f;
    [SerializeField] private Color greenVignetteColor = new Color(0.04f, 0.22f, 0.07f, 1f);

    private float lethargyValue = 1f;
    public float LethargyValue => lethargyValue;

    private float _nausea01;
    private float _greenMeter01;

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

    public void SyncVignetteFromStats(PlayerPotionStats stats)
    {
        if (stats == null)
        {
            _nausea01 = 0f;
            _greenMeter01 = 0f;
            RefreshVignette();
            return;
        }

        _nausea01 = Mathf.Clamp01(stats.GetTotal(EffectType.Nausea) / 100f);

        var maxStat = 0f;
        foreach (var t in PotionRules.CoreEffects)
        {
            maxStat = Mathf.Max(maxStat, stats.GetTotal(t));
        }

        _greenMeter01 = GreenOutVisual.EvaluateMeter01(maxStat);
        RefreshVignette();
    }

    private void RefreshVignette()
    {
        if (vignette == null)
        {
            return;
        }

        var nauseaPart = _nausea01 * nauseaVignetteMaxIntensity;
        var greenPart = _greenMeter01 * greenVignetteMaxIntensity;
        var intensity = Mathf.Clamp01(Mathf.Max(nauseaPart, greenPart));
        var active = intensity > 0.001f;

        vignette.active = active;
        if (!active)
        {
            return;
        }

        vignette.intensity.value = intensity;
        var baseDark = Color.black;
        vignette.color.value = Color.Lerp(baseDark, greenVignetteColor, Mathf.Clamp01(_greenMeter01));
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

    public void ActiveHigh(float value)
    {
        colorAdjustments.active = true;
    }
}
