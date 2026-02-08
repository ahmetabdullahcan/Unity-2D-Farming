using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

public class Hotbar : MonoBehaviour
{
    [SerializeField] private GameObject[] hotbarSlots;
    [SerializeField] private Image[] selectionSlots;
    [SerializeField] private GameObject hotbar;
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

    private async Task HotbarGoAway()
    {
        while (hotbar.transform.localPosition.x > -100)
        {
            hotbar.transform.localPosition += new Vector3(-10, 0, 0);
            await Task.Delay(20);
        }

    }

    private async Task HotbarComeBack()
    {
        while (hotbar.transform.localPosition.x < 0)
        {
            hotbar.transform.localPosition += new Vector3(10, 0, 0);
            await Task.Delay(20);
        }
    }

    private async void MoveHotbar()
    {
        if (hotbar.transform.localPosition.x >= 0)
        {
            await HotbarGoAway();
            hotbar.SetActive(!hotbar.activeSelf);
        }
        else
        {
            hotbar.SetActive(!hotbar.activeSelf);
            await HotbarComeBack();
        }
    }

    private void OnHotbarChanged(InputAction.CallbackContext context)
    {
        if (context.control.path.Equals("/Keyboard/tab"))
        {
            MoveHotbar();
            return;
        }
        var slotIndex = int.Parse(context.control.path.Substring(context.control.path.Length - 1));

        if (slotIndex <= 0)
            return;

        selectedSlot = slotIndex - 1;
        DisableNonSelectedSlots();
    }
}