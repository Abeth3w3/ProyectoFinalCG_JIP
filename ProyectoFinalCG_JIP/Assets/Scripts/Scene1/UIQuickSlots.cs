using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIQuickSlots : MonoBehaviour
{
    public static UIQuickSlots Instance;

    [Header("Slot 1 - Burgers")]
    public Image slot1Icon;
    public TextMeshProUGUI slot1Text;

    [Header("Slot 2 - Drinks")]
    public Image slot2Icon;
    public TextMeshProUGUI slot2Text;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        RefreshSlots();
    }

    public void RefreshSlots()
    {
        // Slot 1 - Burgers
        if (InventoryManager.Instance.slot1 != null)
        {
            slot1Icon.sprite = InventoryManager.Instance.slot1.icon;
            slot1Icon.color = Color.white;
            slot1Text.text = InventoryManager.Instance.slot1.quantity.ToString();
        }
        else
        {
            slot1Icon.color = Color.clear;
            slot1Text.text = "0";
        }

        // Slot 2 - Drinks
        if (InventoryManager.Instance.slot2 != null)
        {
            slot2Icon.sprite = InventoryManager.Instance.slot2.icon;
            slot2Icon.color = Color.white;
            slot2Text.text = InventoryManager.Instance.slot2.quantity.ToString();
        }
        else
        {
            slot2Icon.color = Color.clear;
            slot2Text.text = "0";
        }
    }
}