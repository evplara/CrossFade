using UnityEngine;

public class BlitToScreen : MonoBehaviour
{
    public RenderTexture source;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(source, dest);
    }
}
