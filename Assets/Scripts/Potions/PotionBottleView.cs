using TMPro;
using UnityEngine;

public class PotionBottleView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer bottleRenderer;
    [SerializeField] private TextMeshPro labelText;

    private PotionSceneController controller;
    private int slotIndex;
    private bool hasPotion;

    public void SetPotion(PotionSceneController sceneController, int index, CrossFade.Potions.PotionData potion)
    {
        controller = sceneController;
        slotIndex = index;
        hasPotion = true;

        if (labelText != null)
            labelText.text = potion.Name;

        if (bottleRenderer != null)
            bottleRenderer.color = Color.white;
    }

    public void ClearPotion(PotionSceneController sceneController, int index)
    {
        controller = sceneController;
        slotIndex = index;
        hasPotion = false;

        if (labelText != null)
            labelText.text = "Empty";

        if (bottleRenderer != null)
            bottleRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1f);
    }

    private void OnMouseDown()
    {
        if (!hasPotion || controller == null)
            return;

        controller.SelectPotion(slotIndex);
    }
}