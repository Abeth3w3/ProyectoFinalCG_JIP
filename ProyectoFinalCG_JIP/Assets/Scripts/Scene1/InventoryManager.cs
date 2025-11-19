using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [System.Serializable]
    public class ItemData
    {
        public string itemName;
        public Sprite icon;
        public int quantity;
    }

    // Solo slots rápidos - sin inventario completo
    public ItemData slot1; // Burgers
    public ItemData slot2; // Drinks

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Precargar íconos
            ItemDatabase.LoadIcons();
        }
        else Destroy(gameObject);
    }

    // Sistema ultra-simple: agregar y equipar automáticamente
    public void AddItem(string name, int amount = 1)
    {
        Debug.Log($"Recolectando: {name} x{amount}");

        // BURGER → Slot 1
        if (name.ToLower().Contains("burger"))
        {
            if (slot1 == null)
            {
                slot1 = new ItemData
                {
                    itemName = name,
                    quantity = amount,
                    icon = ItemDatabase.GetIcon(name)
                };
            }
            else
            {
                slot1.quantity += amount;
            }
            Debug.Log($"🍔 Burgers en Slot 1: {slot1.quantity}");
        }
        // DRINK → Slot 2
        else if (name.ToLower().Contains("drink") || name.ToLower().Contains("bebida"))
        {
            if (slot2 == null)
            {
                slot2 = new ItemData
                {
                    itemName = name,
                    quantity = amount,
                    icon = ItemDatabase.GetIcon(name)
                };
            }
            else
            {
                slot2.quantity += amount;
            }
            Debug.Log($"🥤 Drinks en Slot 2: {slot2.quantity}");
        }

        // Actualizar UI de slots
        UIQuickSlots.Instance?.RefreshSlots();
    }

    // Usar Burger del Slot 1
    public void UseSlot1()
    {
        if (slot1 != null && slot1.quantity > 0)
        {
            slot1.quantity--;
            Debug.Log($"🍔 Burger lanzada! Quedan: {slot1.quantity}");

            if (slot1.quantity <= 0)
                slot1 = null;

            UIQuickSlots.Instance?.RefreshSlots();
        }
        else
        {
            Debug.Log("No hay burgers en el Slot 1");
        }
    }

    // Usar Drink del Slot 2
    public void UseSlot2()
    {
        if (slot2 != null && slot2.quantity > 0)
        {
            slot2.quantity--;
            Debug.Log($"🥤 Bebida usada! Quedan: {slot2.quantity}");

            if (slot2.quantity <= 0)
                slot2 = null;

            UIQuickSlots.Instance?.RefreshSlots();
        }
        else
        {
            Debug.Log("No hay bebidas en el Slot 2");
        }
    }
}