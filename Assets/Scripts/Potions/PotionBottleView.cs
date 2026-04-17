using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PotionBottleView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private PotionSceneController controller;
    [SerializeField] private bool isCup;
    [SerializeField] private SpriteRenderer bottleRenderer;
    [SerializeField] private TextMeshPro labelText;
    [SerializeField] private Color selectedColor = new(0.75f, 1f, 0.75f, 1f);
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color emptyColor = new(0.5f, 0.5f, 0.5f, 1f);

    private int slotIndex;
    private bool hasPotion;
    private string tooltipText;
    private string baseLabel;

    private void Awake()
    {
        EnsureController();
    }

    // Set the potion for the bottle view
    public void SetPotion(PotionSceneController sceneController, int inventoryIndex, CrossFade.Potions.PotionData potion, bool isSelected, string tooltip)
    {
        if (isCup)
            return;

        controller = sceneController;
        slotIndex = inventoryIndex;
        hasPotion = true;
        tooltipText = tooltip;
        baseLabel = potion.Name;

        if (labelText != null)
            labelText.text = baseLabel;

        if (bottleRenderer != null)
            bottleRenderer.color = isSelected ? selectedColor : defaultColor;
    }

    // Clear the potion for the bottle view
    public void ClearPotion(PotionSceneController sceneController, int index)
    {
        if (isCup)
            return;

        controller = sceneController;
        slotIndex = index;
        hasPotion = false;
        tooltipText = $"Bottle {index}: Empty";
        baseLabel = "Empty";

        if (labelText != null)
            labelText.text = baseLabel;

        if (bottleRenderer != null)
            bottleRenderer.color = emptyColor;
    }

    // Set the hover state for the bottle view
    public void SetHoverState(bool isHovered, string tooltip)
    {
        if (isCup)
            return;

        if (labelText == null)
            return;

        labelText.text = isHovered ? tooltip : baseLabel;
    }

    // Controls ------------------------------------------------------------
    private void OnMouseDown()
    {
        HandleClick();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        HandleClick();
    }

    private void OnMouseEnter()
    {
        EnsureController();
        if (isCup)
            return;

        if (controller == null)
            return;

        controller.OnBottleHover(slotIndex, true);
    }

    private void OnMouseExit()
    {
        EnsureController();
        if (isCup)
            return;

        if (controller == null)
            return;

        controller.OnBottleHover(slotIndex, false);
    }

    private void EnsureController()
    {
        if (controller == null)
        {
            controller = FindFirstObjectByType<PotionSceneController>();
        }
    }

    private void HandleClick()
    {
        EnsureController();
        if (controller == null)
            return;

        if (isCup)
        {
            Debug.Log("[PotionBottleView] Cup clicked -> OnCupClicked()");
            controller.OnCupClicked();
            return;
        }

        if (!hasPotion)
            return;

        Debug.Log($"[PotionBottleView] Bottle clicked -> OnBottleClicked({slotIndex})");
        controller.OnBottleClicked(slotIndex);
    }
}