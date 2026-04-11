using UnityEngine;
using UnityEngine.UI;

public class DoubleVisionEffect : MonoBehaviour
{
    [Header("Assign these in Inspector")]
    [SerializeField] private RawImage mainView;  
    [SerializeField] private RawImage ghostOverlay;  

    [SerializeField] private float offsetX = 25f;
    [SerializeField] private float offsetY = -15f;
    [SerializeField] private float alpha = 0.45f;
    [SerializeField] private float wobbleSpeed = 1.5f;
    [SerializeField] private float wobbleAmount = 4f;

    private RenderTexture rt;
    private float time;

    void Start()
    {
        Camera cam = GetComponent<Camera>();

        rt = new RenderTexture(Screen.width, Screen.height, 24);
        cam.targetTexture = rt; // main camera renders into RT

        // Full screen view - just shows the RT normally
        mainView.texture = rt;
        mainView.color = Color.white;
        mainView.raycastTarget = false;

        // Ghost overlay - same RT, offset + transparent
        ghostOverlay.texture = rt;
        ghostOverlay.color = new Color(1f, 1f, 1f, alpha);
        ghostOverlay.raycastTarget = false;
    }

    void Update()
    {
        time += Time.deltaTime;
        float wobble = Mathf.Sin(time * wobbleSpeed) * wobbleAmount;
        ghostOverlay.rectTransform.anchoredPosition = new Vector2(offsetX + wobble, offsetY);
    }

    public void SetIntensity(float t)
    {
        ghostOverlay.color = new Color(1f, 1f, 1f, Mathf.Lerp(0f, 0.5f, t));
        offsetX = Mathf.Lerp(0f, 35f, t);
    }

    void OnDestroy()
    {
        if (rt != null) rt.Release();
    }
}
