using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CrossFade.Potions;

public class PotionSceneController : MonoBehaviour
{
    private enum FlowState
    {
        SelectBottleToPour,
        CupReadyForSecondPour,
        CupMixedAwaitingCupClick,
        CupArmedForDrink
    }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI inventoryText;
    [SerializeField] private TextMeshProUGUI selectedPotionText;
    [SerializeField] private Button mixButton; // Acts as "Pour button" when cup is empty, "Mix button" when cup is full.
    [SerializeField] private Button drinkButton; // Acts as "Drink button" when cup is full.

    [Header("Cup Visual")] // Visual representation of the cup's contents.
    [SerializeField] private SpriteRenderer liquidRenderer;
    [SerializeField] private Transform liquidTransform;

    [Header("Bottle Views")]
    [SerializeField] private PotionBottleView[] bottleViews;

    [Header("Inventory")]
    [SerializeField] private int maxSlots = 3;

    private Vector3 liquidStartScale;

    private PotionController potionController;
    private int selectedBottleIndex = -1;
    private int cupPotionIndex = -1;
    private FlowState flowState = FlowState.SelectBottleToPour;
    private bool cupClickedToDrink;

    private void Start()
    {
        if (liquidTransform != null)
        {
            liquidStartScale = liquidTransform.localScale;
        }

        potionController = PotionController.Instance;
        selectedBottleIndex = -1;
        cupPotionIndex = -1;
        flowState = FlowState.SelectBottleToPour;
        cupClickedToDrink = false;
        RefreshScene();
    }

    // Every frame, refresh the buttons.
    private void Update()
    {
        EnsureController();
        RefreshButtons();
    }

    public void RollPotion()
    {
        EnsureController();
        if (potionController == null || potionController.Inventory.Count >= maxSlots)
            return;

        potionController.OnRollButtonClicked();
        flowState = cupPotionIndex >= 0 ? flowState : FlowState.SelectBottleToPour;
        RefreshScene();
    }

    public void MixPotion0And1()
    {
        MixSelectedPotions();
    }

    // Pour flow:
    // 1) select bottle A -> pour (cup now has A)
    // 2) select bottle B -> pour (cup mixes A+B)
    public void MixSelectedPotions()
    {
        EnsureController();
        LogPourState("PourClicked-BeforeValidation");
        if (potionController == null)
            return;

        if (selectedBottleIndex < 0 || selectedBottleIndex >= potionController.Inventory.Count)
            return;

        if (flowState == FlowState.CupMixedAwaitingCupClick || flowState == FlowState.CupArmedForDrink)
            return;

        // First pour: move selected bottle into cup state.
        if (cupPotionIndex < 0)
        {
            cupPotionIndex = selectedBottleIndex;
            flowState = FlowState.CupReadyForSecondPour;
            cupClickedToDrink = false;
            selectedBottleIndex = -1; // Disable Pour until a new bottle is selected.
            LogPourState("PourClicked-AfterFirstPour");
            RefreshScene(); // Updates cup visual + text to poured bottle.
            return;
        }

        // Second pour: mix selected bottle with cup contents.
        if (selectedBottleIndex == cupPotionIndex)
            return;

        var left = Mathf.Min(cupPotionIndex, selectedBottleIndex);
        var right = Mathf.Max(cupPotionIndex, selectedBottleIndex);
        if (!potionController.TryMixByIndex(left, right))
            return;

        cupPotionIndex = Mathf.Min(left, potionController.Inventory.Count - 1);
        flowState = FlowState.CupMixedAwaitingCupClick;
        cupClickedToDrink = false;
        selectedBottleIndex = -1;
        LogPourState("PourClicked-AfterSecondPourMix");
        RefreshScene();
    }

    public void ClearDisplay()
    {
        selectedPotionText.text = "No potion selected.";
    }

    public void SelectPotion(int index)
    {
        EnsureController();
        if (potionController == null || index < 0 || index >= potionController.Inventory.Count)
            return;

        var potion = potionController.Inventory[index];
        selectedPotionText.text = FormatPotionDetails(potion);
    }

    public void OnBottleClicked(int index)
    {
        EnsureController();
        if (potionController == null || index < 0 || index >= potionController.Inventory.Count)
            return;

        // Once cup already has the mixed result, force cup->drink flow.
        if (flowState == FlowState.CupMixedAwaitingCupClick || flowState == FlowState.CupArmedForDrink)
            return;

        selectedBottleIndex = index;
        flowState = cupPotionIndex >= 0 ? FlowState.CupReadyForSecondPour : FlowState.SelectBottleToPour;
        cupClickedToDrink = false;
        SelectPotion(index);
        RefreshBottleViews();
        RefreshButtons();
    }

    public void OnBottleHover(int index, bool isHovered)
    {
        EnsureController();
        if (potionController == null)
            return;

        for (int i = 0; i < bottleViews.Length; i++)
        {
            if (bottleViews[i] == null)
                continue;

            var start = Mathf.Max(0, potionController.Inventory.Count - 3);
            var inventoryIndex = start + i;
            if (inventoryIndex != index)
                continue;

            var potion = inventoryIndex < potionController.Inventory.Count ? potionController.Inventory[inventoryIndex] : null;
            bottleViews[i].SetHoverState(isHovered, potion != null ? BuildTooltipText(i, potion) : $"Bottle {i}: Empty");
            break;
        }
    }

    public void OnCupClicked()
    {
        EnsureController();
        Debug.Log($"[PotionScene][CupClick-Before] flowState={flowState} cupPotionIndex={cupPotionIndex} cupClickedToDrink={cupClickedToDrink} controller={(potionController != null)}");
        if (potionController == null || cupPotionIndex < 0 || cupPotionIndex >= potionController.Inventory.Count)
        {
            Debug.Log("[PotionScene][CupClick-Blocked] invalid controller or cup potion index.");
            return;
        }

        if (flowState != FlowState.CupMixedAwaitingCupClick && flowState != FlowState.CupArmedForDrink)
        {
            Debug.Log($"[PotionScene][CupClick-Blocked] invalid state for arming drink: {flowState}");
            return;
        }

        flowState = FlowState.CupArmedForDrink;
        cupClickedToDrink = true;
        var cupPotion = potionController.Inventory[cupPotionIndex];
        selectedPotionText.text = FormatPotionDetails(cupPotion);
        UpdateCupVisual(cupPotion);
        RefreshButtons();
        LogPourState("CupClick-AfterArmDrink");
    }

    public void DrinkSelectedOrFirst()
    {
        DrinkFromCup();
    }

    public void DrinkFromCup()
    {
        EnsureController();
        if (potionController == null || cupPotionIndex < 0 || cupPotionIndex >= potionController.Inventory.Count)
            return;

        if (flowState != FlowState.CupArmedForDrink)
            return;

        if (!potionController.OnConsumeButtonClicked(cupPotionIndex))
            return;

        selectedBottleIndex = -1;
        cupPotionIndex = -1;
        flowState = FlowState.SelectBottleToPour;
        cupClickedToDrink = false;
        RefreshScene();
    }

    private void RefreshScene()
    {
        EnsureController();
        if (potionController == null)
        {
            ResetCupVisual();
            RefreshButtons();
            return;
        }

        if (selectedBottleIndex < 0 || selectedBottleIndex >= potionController.Inventory.Count)
        {
            selectedBottleIndex = -1;
        }

        if (cupPotionIndex < 0 || cupPotionIndex >= potionController.Inventory.Count)
        {
            cupPotionIndex = -1;
            flowState = FlowState.SelectBottleToPour;
            cupClickedToDrink = false;
        }

        RefreshInventoryText();
        RefreshBottleViews();
        RefreshButtons();

        if (cupPotionIndex >= 0 && cupPotionIndex < potionController.Inventory.Count)
        {
            var cupPotion = potionController.Inventory[cupPotionIndex];
            selectedPotionText.text = FormatPotionDetails(cupPotion);
            UpdateCupVisual(cupPotion);
        }
        else
        {
            ResetCupVisual();
        }
    }

    private void RefreshInventoryText()
    {
        if (potionController == null)
            return;

        var sb = new StringBuilder();
        sb.AppendLine("Inventory");

        if (potionController.Inventory.Count == 0)
        {
            sb.AppendLine("(empty)");
        }
        else
        {
            var start = Mathf.Max(0, potionController.Inventory.Count - 3);
            var viewIndex = 0;
            for (int i = start; i < potionController.Inventory.Count && viewIndex < 3; i++, viewIndex++)
            {
                var potion = potionController.Inventory[i];
                sb.AppendLine($"Bottle {viewIndex}: {potion.Name} [{potion.Rarity}]");
            }
        }

        inventoryText.text = sb.ToString();
    }

    private void RefreshBottleViews()
    {
        if (potionController == null)
            return;

        var inventory = potionController.Inventory;
        var start = Mathf.Max(0, inventory.Count - 3);
        for (int i = 0; i < bottleViews.Length; i++)
        {
            if (bottleViews[i] == null)
                continue;

            var inventoryIndex = start + i;
            if (inventoryIndex < inventory.Count)
            {
                var potion = inventory[inventoryIndex];
                bottleViews[i].SetPotion(this, inventoryIndex, potion, selectedBottleIndex == inventoryIndex, BuildTooltipText(i, potion));
            }
            else
            {
                bottleViews[i].ClearPotion(this, i);
            }
        }
    }

    private string BuildTooltipText(int bottleIndex, PotionData potion)
    {
        return $"Bottle {bottleIndex}: {potion.Name}\n{potion.Rarity}";
    }

    private void RefreshButtons()
    {
        EnsureController();
        var canPour = potionController != null
                      && selectedBottleIndex >= 0
                      && flowState != FlowState.CupMixedAwaitingCupClick
                      && flowState != FlowState.CupArmedForDrink
                      && (cupPotionIndex < 0 || selectedBottleIndex != cupPotionIndex);
        var canDrink = potionController != null
                       && flowState == FlowState.CupArmedForDrink
                       && cupPotionIndex >= 0
                       && cupPotionIndex < potionController.Inventory.Count
                       && cupClickedToDrink;

        if (mixButton != null)
        {
            mixButton.interactable = canPour;
        }

        if (drinkButton != null)
        {
            drinkButton.interactable = canDrink;
        }
    }

    private void EnsureController()
    {
        if (potionController == null)
        {
            potionController = PotionController.Instance;
        }
    }

    private void LogPourState(string stage)
    {
        var inventoryCount = potionController != null ? potionController.Inventory.Count : -1;
        var canPour = potionController != null
                      && selectedBottleIndex >= 0
                      && flowState != FlowState.CupMixedAwaitingCupClick
                      && flowState != FlowState.CupArmedForDrink
                      && (cupPotionIndex < 0 || selectedBottleIndex != cupPotionIndex);
        var canDrink = potionController != null
                       && flowState == FlowState.CupArmedForDrink
                       && cupPotionIndex >= 0
                       && cupPotionIndex < inventoryCount
                       && cupClickedToDrink;

        Debug.Log(
            $"[PotionScene][{stage}] " +
            $"flowState={flowState} | selectedBottleIndex={selectedBottleIndex} | cupPotionIndex={cupPotionIndex} | " +
            $"cupClickedToDrink={cupClickedToDrink} | inventoryCount={inventoryCount} | canPour={canPour} | canDrink={canDrink}");
    }

    private string FormatPotionDetails(PotionData potion)
    {
        var sb = new StringBuilder();
        sb.AppendLine(potion.Name);
        sb.AppendLine($"Rarity: {potion.Rarity}");
        sb.AppendLine();

        for (int i = 0; i < potion.Effects.Count; i++)
        {
            var effect = potion.Effects[i];
            sb.AppendLine($"{effect.EffectType}: {effect.Value}");
        }

        sb.AppendLine();
        sb.AppendLine($"Max Effect: {potion.GetMaxEffectValue()}");
        sb.AppendLine($"Greened Out: {(potion.IsGreenedOut() ? "Yes" : "No")}");

        return sb.ToString();
    }

    private void UpdateCupVisual(PotionData potion)
    {
        if (liquidRenderer != null)
            liquidRenderer.color = BuildPotionColor(potion);

        if (liquidTransform != null)
        {
            float fill = Mathf.Clamp(0.35f + (potion.GetMaxEffectValue() / 20f), 0.35f, 1.2f);
            liquidTransform.localScale = new Vector3(liquidStartScale.x, fill, liquidStartScale.z);
        }
    }

    private void ResetCupVisual()
    {
        if (liquidRenderer != null)
            liquidRenderer.color = Color.clear;

        if (liquidTransform != null)
            liquidTransform.localScale = liquidStartScale;

        selectedPotionText.text = "No potion selected.";
    }

    private Color BuildPotionColor(PotionData potion)
    {
        const float colorNormalizeMax = 20f;

        float high = potion.GetEffectValue(EffectType.High) / colorNormalizeMax;
        float dizzy = potion.GetEffectValue(EffectType.Dizziness) / colorNormalizeMax;
        float focus = potion.GetEffectValue(EffectType.Focus) / colorNormalizeMax;

        return new Color(
            Mathf.Clamp01(0.2f + high),
            Mathf.Clamp01(0.2f + focus),
            Mathf.Clamp01(0.2f + dizzy),
            1f
        );
    }
}
