using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Hotbar : MonoBehaviour
{
    [SerializeField] private GameObject[] hotbarSlots;
    [SerializeField] private Image[] selectionSlots;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputActionReference hotbarAction;

    private int selectedSlot = 0;

    void DisableNonSelectedSlots()
    {
        for (int i = 0; i < selectionSlots.Length; i++)
        {
            if (i == selectedSlot)
            {
                selectionSlots[i].color = new Color(
                    selectionSlots[i].color.r, 
                    selectionSlots[i].color.g, 
                    selectionSlots[i].color.b, 
                    1f);
                selectionSlots[i].enabled = true;
            }
            else
            {
                selectionSlots[i].color = new Color(
                    selectionSlots[i].color.r, 
                    selectionSlots[i].color.g, 
                    selectionSlots[i].color.b, 
                    0f);
                selectionSlots[i].enabled = false;
            }
        }
    }

    void Start()
    {
        DisableNonSelectedSlots();
    }

    private void OnEnable()
    {
        hotbarAction.action.Enable();
        hotbarAction.action.performed += OnHotbarChanged;
    }

    private void OnDisable()
    {
        hotbarAction.action.performed -= OnHotbarChanged;
        hotbarAction.action.Disable();
    }

    public int GetSelectedSlot()
    {
        return selectedSlot;
    }

    private void OnHotbarChanged(InputAction.CallbackContext context)
    {
        var slotIndex = int.Parse(context.control.path.Substring(context.control.path.Length - 1));

        if (slotIndex <= 0)
            return;

        selectedSlot = slotIndex - 1;
        DisableNonSelectedSlots();
    }
}