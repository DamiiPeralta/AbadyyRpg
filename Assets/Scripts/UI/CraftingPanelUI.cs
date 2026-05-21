using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingPanelUI : MonoBehaviour
{
    [Header("Recipes")]
    public List<CraftingRecipeSO> recipes = new List<CraftingRecipeSO>();
    public CraftingStationType currentStation = CraftingStationType.Blacksmith;

    [Header("List")]
    public Transform recipeRowsRoot;
    public RecipeRowUI recipeRowPrefab;

    [Header("Details")]
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text inputsText;
    public TMP_Text outputsText;
    public List<RecipeStackSlotUI> inputSlots = new List<RecipeStackSlotUI>();
    public List<RecipeStackSlotUI> outputSlots = new List<RecipeStackSlotUI>();
    public TMP_Text quantityText;
    public TMP_InputField quantityInput;
    public Button decreaseButton;
    public Button increaseButton;
    public Button processButton;
    public TMP_Text processButtonText;
    public TMP_Text messageText;

    [Header("Station Buttons")]
    public Button blacksmithButton;
    public Button cookButton;
    public Button alchemistButton;
    public Button runicMageButton;

    private readonly List<RecipeRowUI> rowInstances = new List<RecipeRowUI>();
    private CraftingRecipeSO selectedRecipe;
    private int quantity = 1;

    private void Awake()
    {
        AutoBind();
        HookButtons();
    }

    private void OnEnable()
    {
        RefreshRecipes();
    }

    public void ShowBlacksmith()
    {
        ShowStation(CraftingStationType.Blacksmith);
    }

    public void ShowCook()
    {
        ShowStation(CraftingStationType.Cook);
    }

    public void ShowAlchemist()
    {
        ShowStation(CraftingStationType.Alchemist);
    }

    public void ShowRunicMage()
    {
        ShowStation(CraftingStationType.RunicMage);
    }

    public void ShowStation(CraftingStationType stationType)
    {
        currentStation = stationType;
        selectedRecipe = null;
        quantity = 1;
        RefreshRecipes();
    }

    public void RefreshRecipes()
    {
        ClearRows();

        List<CraftingRecipeSO> filteredRecipes = GetFilteredRecipes();

        if (selectedRecipe == null && filteredRecipes.Count > 0)
            selectedRecipe = filteredRecipes[0];

        if (recipeRowsRoot != null && recipeRowPrefab != null)
        {
            foreach (CraftingRecipeSO recipe in filteredRecipes)
            {
                if (recipe == null)
                    continue;

                RecipeRowUI row = Instantiate(recipeRowPrefab, recipeRowsRoot);
                row.gameObject.SetActive(true);
                row.Bind(recipe, SelectRecipe);
                rowInstances.Add(row);
            }
        }

        RefreshDetails();
    }

    private void SelectRecipe(CraftingRecipeSO recipe)
    {
        selectedRecipe = recipe;
        quantity = 1;
        RefreshDetails();
    }

    private void IncreaseQuantity()
    {
        SetQuantity(quantity + 1);
    }

    private void DecreaseQuantity()
    {
        SetQuantity(quantity - 1);
    }

    private void HandleQuantityInputChanged(string value)
    {
        if (!int.TryParse(value, out int parsed))
            parsed = 1;

        SetQuantity(parsed, false);
    }

    private void SetQuantity(int value, bool updateInput = true)
    {
        quantity = Mathf.Clamp(value, 1, 999);

        if (updateInput && quantityInput != null)
            quantityInput.text = quantity.ToString();

        RefreshDetails();
    }

    private void RefreshDetails()
    {
        if (quantityInput != null && quantityInput.text != quantity.ToString())
            quantityInput.text = quantity.ToString();

        if (selectedRecipe == null)
        {
            SetText(titleText, "Sin receta");
            SetText(descriptionText, string.Empty);
            SetText(inputsText, string.Empty);
            SetText(outputsText, string.Empty);
            RefreshStackSlots(null, inputSlots, true);
            RefreshStackSlots(null, outputSlots, false);
            SetText(quantityText, quantity.ToString());
            SetText(messageText, "No hay recetas para esta estacion.");
            SetProcessInteractable(false);
            return;
        }

        SetText(titleText, selectedRecipe.GetDisplayName());
        SetText(descriptionText, selectedRecipe.description);
        SetText(inputsText, BuildStacksText("Entrada", selectedRecipe.inputs, true));
        SetText(outputsText, BuildStacksText("Salida", selectedRecipe.outputs, false));
        RefreshStackSlots(selectedRecipe.inputs, inputSlots, true);
        RefreshStackSlots(selectedRecipe.outputs, outputSlots, false);
        SetText(quantityText, quantity.ToString());

        bool canProcess = CanProcess(selectedRecipe, quantity);
        SetProcessInteractable(canProcess);
        SetText(messageText, canProcess ? "Listo para procesar." : "Faltan recursos.");
    }

    private string BuildStacksText(string title, List<RecipeStack> stacks, bool showOwned)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine(title);

        if (stacks == null || stacks.Count == 0)
        {
            builder.AppendLine("-");
            return builder.ToString();
        }

        int count = Mathf.Min(6, stacks.Count);

        for (int i = 0; i < count; i++)
        {
            RecipeStack stack = stacks[i];

            if (stack == null)
                continue;

            int needed = Mathf.Max(0, stack.amount) * quantity;
            string displayName = GetStackName(stack);

            if (showOwned)
            {
                int owned = GetOwnedAmount(stack);
                string color = owned >= needed ? "#D7F8C5" : "#FF7777";
                builder.AppendLine($"<color={color}>{displayName}: {needed}/{owned}</color>");
            }
            else
            {
                builder.AppendLine($"{displayName}: x{needed}");
            }
        }

        if (stacks.Count > count)
            builder.AppendLine($"Y {stacks.Count - count} mas...");

        return builder.ToString();
    }

    private void RefreshStackSlots(List<RecipeStack> stacks, List<RecipeStackSlotUI> slots, bool showOwned)
    {
        if (slots == null || slots.Count == 0)
            return;

        int stackCount = stacks != null ? stacks.Count : 0;

        for (int i = 0; i < slots.Count; i++)
        {
            RecipeStackSlotUI slot = slots[i];

            if (slot == null)
                continue;

            if (i < stackCount && stacks[i] != null)
                slot.Show(stacks[i], quantity, showOwned);
            else
                slot.Hide();
        }
    }

    private bool CanProcess(CraftingRecipeSO recipe, int amount)
    {
        if (recipe == null || InventoryRuntimeState.Instance == null)
            return false;

        if (recipe.inputs == null || recipe.inputs.Count == 0)
            return false;

        foreach (RecipeStack input in recipe.inputs)
        {
            if (input == null)
                return false;

            int needed = Mathf.Max(0, input.amount) * amount;

            if (GetOwnedAmount(input) < needed)
                return false;
        }

        return true;
    }

    private void ProcessSelectedRecipe()
    {
        if (selectedRecipe == null || !CanProcess(selectedRecipe, quantity))
        {
            RefreshDetails();
            return;
        }

        foreach (RecipeStack input in selectedRecipe.inputs)
            SpendStack(input, quantity);

        foreach (RecipeStack output in selectedRecipe.outputs)
            AddStack(output, quantity);

        SetText(messageText, $"Procesado x{quantity}: {selectedRecipe.GetDisplayName()}");
        RefreshDetails();
    }

    private int GetOwnedAmount(RecipeStack stack)
    {
        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;

        if (stack == null || inventory == null)
            return 0;

        switch (stack.stackType)
        {
            case RecipeStackType.Item:
                return inventory.GetAmount(stack.GetItemId());
            case RecipeStackType.Gold:
                return inventory.gold;
            case RecipeStackType.Food:
                return inventory.food;
            case RecipeStackType.Wood:
                return inventory.wood;
            case RecipeStackType.Stone:
                return inventory.stone;
            case RecipeStackType.Iron:
                return inventory.iron;
            case RecipeStackType.Leather:
                return inventory.leather;
            case RecipeStackType.Crystals:
                return inventory.crystals;
            default:
                return 0;
        }
    }

    private bool SpendStack(RecipeStack stack, int amount)
    {
        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;

        if (stack == null || inventory == null)
            return false;

        int total = Mathf.Max(0, stack.amount) * amount;

        switch (stack.stackType)
        {
            case RecipeStackType.Item:
                return inventory.RemoveItem(stack.GetItemId(), total);
            case RecipeStackType.Gold:
                return inventory.SpendGold(total);
            case RecipeStackType.Food:
                return inventory.SpendFood(total);
            case RecipeStackType.Wood:
                return inventory.SpendWood(total);
            case RecipeStackType.Stone:
                return inventory.SpendStone(total);
            case RecipeStackType.Iron:
                return inventory.SpendIron(total);
            case RecipeStackType.Leather:
                return inventory.SpendLeather(total);
            case RecipeStackType.Crystals:
                return inventory.SpendCrystals(total);
            default:
                return false;
        }
    }

    private void AddStack(RecipeStack stack, int amount)
    {
        InventoryRuntimeState inventory = InventoryRuntimeState.Instance;

        if (stack == null || inventory == null)
            return;

        int total = Mathf.Max(0, stack.amount) * amount;

        switch (stack.stackType)
        {
            case RecipeStackType.Item:
                inventory.AddItem(stack.GetItemId(), total);
                break;
            case RecipeStackType.Gold:
                inventory.AddGold(total);
                break;
            case RecipeStackType.Food:
                inventory.AddFood(total);
                break;
            case RecipeStackType.Wood:
                inventory.AddWood(total);
                break;
            case RecipeStackType.Stone:
                inventory.AddStone(total);
                break;
            case RecipeStackType.Iron:
                inventory.AddIron(total);
                break;
            case RecipeStackType.Leather:
                inventory.AddLeather(total);
                break;
            case RecipeStackType.Crystals:
                inventory.AddCrystals(total);
                break;
        }
    }

    private string GetStackName(RecipeStack stack)
    {
        if (stack == null)
            return "-";

        if (stack.stackType == RecipeStackType.Item)
        {
            ItemBase item = stack.GetItem();
            return item != null && !string.IsNullOrWhiteSpace(item.itemName) ? item.itemName : stack.GetItemId();
        }

        switch (stack.stackType)
        {
            case RecipeStackType.Gold:
                return "Oro";
            case RecipeStackType.Food:
                return "Comida";
            case RecipeStackType.Wood:
                return "Madera";
            case RecipeStackType.Stone:
                return "Piedra";
            case RecipeStackType.Iron:
                return "Hierro";
            case RecipeStackType.Leather:
                return "Cuero";
            case RecipeStackType.Crystals:
                return "Cristales";
            default:
                return stack.stackType.ToString();
        }
    }

    private List<CraftingRecipeSO> GetFilteredRecipes()
    {
        List<CraftingRecipeSO> filtered = new List<CraftingRecipeSO>();

        foreach (CraftingRecipeSO recipe in recipes)
        {
            if (recipe != null && recipe.stationType == currentStation)
                filtered.Add(recipe);
        }

        return filtered;
    }

    private void ClearRows()
    {
        foreach (RecipeRowUI row in rowInstances)
        {
            if (row != null)
                Destroy(row.gameObject);
        }

        rowInstances.Clear();
    }

    private void SetProcessInteractable(bool value)
    {
        if (processButton != null)
            processButton.interactable = value;

        SetText(processButtonText, "Procesar");
    }

    private void HookButtons()
    {
        if (blacksmithButton != null)
            blacksmithButton.onClick.AddListener(ShowBlacksmith);

        if (cookButton != null)
            cookButton.onClick.AddListener(ShowCook);

        if (alchemistButton != null)
            alchemistButton.onClick.AddListener(ShowAlchemist);

        if (runicMageButton != null)
            runicMageButton.onClick.AddListener(ShowRunicMage);

        if (decreaseButton != null)
            decreaseButton.onClick.AddListener(DecreaseQuantity);

        if (increaseButton != null)
            increaseButton.onClick.AddListener(IncreaseQuantity);

        if (processButton != null)
            processButton.onClick.AddListener(ProcessSelectedRecipe);

        if (quantityInput != null)
            quantityInput.onValueChanged.AddListener(HandleQuantityInputChanged);
    }

    private void AutoBind()
    {
        if (recipeRowsRoot == null)
            recipeRowsRoot = FindChildTransform("Panel_CraftingRecipeContainer");

        if (recipeRowsRoot == null)
            recipeRowsRoot = FindChildTransform("Content");

        if (recipeRowPrefab == null)
            recipeRowPrefab = GetComponentInChildren<RecipeRowUI>(true);

        blacksmithButton = blacksmithButton != null ? blacksmithButton : FindChild<Button>("Button_Blacksmith");
        blacksmithButton = blacksmithButton != null ? blacksmithButton : FindButtonByText("Herrero");

        cookButton = cookButton != null ? cookButton : FindChild<Button>("Button_Cook");
        cookButton = cookButton != null ? cookButton : FindButtonByText("Cocinero");

        alchemistButton = alchemistButton != null ? alchemistButton : FindChild<Button>("Button_Alchemist");
        alchemistButton = alchemistButton != null ? alchemistButton : FindButtonByText("Alquimista");

        runicMageButton = runicMageButton != null ? runicMageButton : FindChild<Button>("Button_RunicMage");
        runicMageButton = runicMageButton != null ? runicMageButton : FindButtonByText("Mago runico");

        processButton = processButton != null ? processButton : FindChild<Button>("Button_Craft");
        processButton = processButton != null ? processButton : FindChild<Button>("Button_Process");

        if (processButtonText == null && processButton != null)
            processButtonText = processButton.GetComponentInChildren<TMP_Text>(true);

        quantityInput = quantityInput != null ? quantityInput : GetComponentInChildren<TMP_InputField>(true);
        decreaseButton = decreaseButton != null ? decreaseButton : FindChild<Button>("Button_QuantityDown");
        increaseButton = increaseButton != null ? increaseButton : FindChild<Button>("Button_QuantityUp");

        titleText = titleText != null ? titleText : FindChild<TMP_Text>("Text_RecipeTitle");
        titleText = titleText != null ? titleText : FindChild<TMP_Text>("Text_RecipeName");
        descriptionText = descriptionText != null ? descriptionText : FindChild<TMP_Text>("Text_RecipeDescription");
        inputsText = inputsText != null ? inputsText : FindChild<TMP_Text>("Text_RecipeInputs");
        outputsText = outputsText != null ? outputsText : FindChild<TMP_Text>("Text_RecipeOutputs");
        quantityText = quantityText != null ? quantityText : FindChild<TMP_Text>("Text_RecipeQuantity");
        quantityText = quantityText != null ? quantityText : FindChild<TMP_Text>("Text_ItemQuantToCraft");
        messageText = messageText != null ? messageText : FindChild<TMP_Text>("Text_CraftingMessage");

        AutoBindStackSlots();
    }

    private void AutoBindStackSlots()
    {
        if (inputSlots.Count == 0)
            AddSlotsFromPanel(inputSlots, "Panel_CraftingRecipeNeedItems");

        if (inputSlots.Count == 0)
            AddSlotsFromPanel(inputSlots, "Panel_CraftingRecipeInputs");

        if (outputSlots.Count == 0)
            AddSlotsFromPanel(outputSlots, "Panel_CraftingRecipeOutputItems");

        if (outputSlots.Count == 0)
            AddSlotsFromPanel(outputSlots, "Panel_CraftingRecipeOutputs");
    }

    private void AddSlotsFromPanel(List<RecipeStackSlotUI> targetSlots, string panelName)
    {
        Transform panel = FindChildTransform(panelName);

        if (panel == null)
            return;

        foreach (RecipeStackSlotUI slot in panel.GetComponentsInChildren<RecipeStackSlotUI>(true))
        {
            if (slot != null && !targetSlots.Contains(slot))
                targetSlots.Add(slot);
        }
    }

    private Button FindButtonByText(string value)
    {
        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            TMP_Text text = button.GetComponentInChildren<TMP_Text>(true);

            if (text != null && text.text.Trim().ToLowerInvariant() == value.ToLowerInvariant())
                return button;
        }

        return null;
    }

    private T FindChild<T>(string childName) where T : Component
    {
        Transform child = FindChildTransform(childName);
        return child != null ? child.GetComponent<T>() : null;
    }

    private Transform FindChildTransform(string childName)
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name.Trim() == childName)
                return child;
        }

        return null;
    }

    private void SetText(TMP_Text text, string value)
    {
        if (text != null)
            text.text = value;
    }
}
