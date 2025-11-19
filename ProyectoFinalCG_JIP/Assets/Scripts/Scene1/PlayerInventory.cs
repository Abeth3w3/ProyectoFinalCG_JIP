using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class InventoryItem
    {
        public string itemName;
        public int quantity;
    }

    [Header("Inventario")]
    public List<InventoryItem> inventory = new List<InventoryItem>();

    [Header("UI Referencias (Opcional)")]
    public TMP_Text inventoryText;

    public void AddItem(string itemName, int quantity = 1)
    {
        InventoryItem existingItem = inventory.Find(item => item.itemName == itemName);

        if (existingItem != null)
            existingItem.quantity += quantity;
        else
            inventory.Add(new InventoryItem { itemName = itemName, quantity = quantity });

        Debug.Log($"Añadido: {itemName} x{quantity}");
        UpdateInventoryUI();
    }

    public bool RemoveItem(string itemName, int quantity = 1)
    {
        InventoryItem existingItem = inventory.Find(item => item.itemName == itemName);

        if (existingItem != null && existingItem.quantity >= quantity)
        {
            existingItem.quantity -= quantity;

            if (existingItem.quantity <= 0)
                inventory.Remove(existingItem);

            UpdateInventoryUI();
            return true;
        }

        Debug.LogWarning($"No se pudo remover: {itemName} x{quantity}");
        return false;
    }

    public bool HasItem(string itemName, int minQuantity = 1)
    {
        InventoryItem existingItem = inventory.Find(item => item.itemName == itemName);
        return existingItem != null && existingItem.quantity >= minQuantity;
    }

    public int GetItemQuantity(string itemName)
    {
        InventoryItem existingItem = inventory.Find(item => item.itemName == itemName);
        return existingItem != null ? existingItem.quantity : 0;
    }

    void UpdateInventoryUI()
    {
        if (inventoryText == null) return;

        string inventoryString = "Inventario:\n";
        foreach (var item in inventory)
            inventoryString += $"{item.itemName}: {item.quantity}\n";

        inventoryText.text = inventoryString;
    }
}
