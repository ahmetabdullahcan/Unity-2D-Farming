using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int numberOfSlots;
    public GameObject[] itemPrefabs;
    public InputActionReference toggleInventoryAction;

    public PlayerInput playerInput;

    void Start()
    {
        inventoryPanel.SetActive(false);
        for (int i = 0; i < numberOfSlots; i++)
        {
            Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();
            if (i < itemPrefabs.Length)
            {
                GameObject item = Instantiate(itemPrefabs[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = item;
            }
        }
    }

    void onToggleInventory(InputAction.CallbackContext context)
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }

    void OnEnable()
    {
        toggleInventoryAction.action.Enable();
        toggleInventoryAction.action.performed += onToggleInventory;
    }

    void OnDisable()
    {
        toggleInventoryAction.action.performed -= onToggleInventory;
        toggleInventoryAction.action.Disable();
    }
}
