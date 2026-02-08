using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour
{
    #region Editor Fields
    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap interactableTilemap;
    [SerializeField] private Tilemap highlightTilemap;

    [Header("Tiles")]
    [SerializeField] private TileBase[] farmTiles;
    [SerializeField] private TileBase[] wateredFarmTiles;
    [SerializeField] private TileBase highlightTile;


    [Header("Experimental")]
    [SerializeField] private Sprite[] progressRingBar;
    [SerializeField] private SpriteRenderer progressRingRenderer;
    [SerializeField] private TextMeshProUGUI speechBubbleText;
    [SerializeField] private Image speechBubbleRenderer;
    [SerializeField] private GameObject hotbar;

    [Header("Animations")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    #endregion

    private Camera mainCamera;
    private Vector3Int lastHighlightedCell;
    private bool hasHighlight;

    #region Action Handlers
    private Farming farming;

    private Watering watering;

    #endregion

    private void Start()
    {
        farming = new Farming(farmTiles, interactableTilemap, playerAnimator);
        watering = new Watering(wateredFarmTiles, interactableTilemap, playerAnimator);
    }

    
    private void Awake()
    {
        mainCamera = Camera.main;
        speechBubbleRenderer.color = new Color(
            speechBubbleRenderer.color.r, 
            speechBubbleRenderer.color.g, 
            speechBubbleRenderer.color.b, 
            0f);
    }


    private void LateUpdate()
    {
        UpdateHighlight();
        HandleInput();
    }

    private int GetSelectedHotbarSlot()
    {
        foreach (Image slot in hotbar.GetComponentsInChildren<Image>())
        {
            if (slot.enabled)
            {
                return Array.IndexOf(hotbar.GetComponentsInChildren<Image>(), slot);
            }
        }
        return -1;
    }


    private async Task ProgressUpdate()
    {
        playerInput.actions.Disable();
        for (int i = 0; i < progressRingBar.Length; i++)
        {
            progressRingRenderer.sprite = progressRingBar[i];
            await Task.Delay(300);
        }
        playerInput.actions.Enable();
        progressRingRenderer.sprite = null;
    }

    private void UpdateHighlight()
    {
        Vector3Int cell = GetTargetedTile();

        if (!interactableTilemap.HasTile(cell))
        {
            ClearHighlight();
            return;
        }

        if (hasHighlight && cell == lastHighlightedCell)
            return;

        if (hasHighlight)
            highlightTilemap.SetTile(lastHighlightedCell, null);

        highlightTilemap.SetTile(cell, highlightTile);

        lastHighlightedCell = cell;
        hasHighlight = true;
    }


    private void ClearHighlight()
    {
        if (!hasHighlight)
            return;

        highlightTilemap.SetTile(lastHighlightedCell, null);
        hasHighlight = false;
    }

    private Vector3Int GetTargetedTile()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        worldPos.z = 0f;

        return interactableTilemap.WorldToCell(worldPos);
    }


    private void SetLookDirection()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        Vector3 direction = worldPos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle >= -45f && angle < 45f)
        {
            playerSpriteRenderer.flipX = false;
            playerAnimator.SetFloat("LookDirectionX", 1); 
            playerAnimator.SetFloat("LookDirectionY", 0);
        }

        else if (angle >= 45f && angle < 135f)
        {
            playerAnimator.SetFloat("LookDirectionX", 0); 
            playerAnimator.SetFloat("LookDirectionY", 1);
        }
        else if (angle >= -135f && angle < -45f)
        {
            playerAnimator.SetFloat("LookDirectionX", 0);
            playerAnimator.SetFloat("LookDirectionY", -1);
        }
        else
        {
            playerSpriteRenderer.flipX = true;
            playerAnimator.SetFloat("LookDirectionX", -1); 
            playerAnimator.SetFloat("LookDirectionY", 0);
        }
    }

    private async Task ShowSpeechBubble(string message)
    {
        if (speechBubbleText.text.Length > 0)
            return;
        speechBubbleRenderer.color = new Color(
        speechBubbleRenderer.color.r, 
        speechBubbleRenderer.color.g, 
        speechBubbleRenderer.color.b, 
        1f);
        while (speechBubbleText.text.Length  < message.Length)
        {
            speechBubbleText.text += message[speechBubbleText.text.Length];
            await Task.Delay(50);
        }
        await Task.Delay(1000);
        while (speechBubbleText.text.Length > 0)
        {
            speechBubbleText.text = speechBubbleText.text.Substring(0, speechBubbleText.text.Length - 1);
            await Task.Delay(50);
        }
        speechBubbleRenderer.color = new Color(
            speechBubbleRenderer.color.r, 
            speechBubbleRenderer.color.g, 
            speechBubbleRenderer.color.b, 
            0f);
        speechBubbleText.text = "";
    }

    private async void HandleInput()
    {
        if (!playerInput.actions["Action"].WasPerformedThisFrame())
            return;
        SetLookDirection();
        Vector3Int targetedCell = GetTargetedTile();
        switch (GetSelectedHotbarSlot())
        {
            case 0:
                if (farming.CanFarmAtCell(targetedCell, speechBubbleRenderer, speechBubbleText))
                {
                    playerAnimator.SetBool("isHoeing", true);
                    await ProgressUpdate();
                    farming.HandleFarming(targetedCell);
                    playerAnimator.SetBool("isHoeing", false);
                }
                break;
            case 1:
                await ShowSpeechBubble("This feature is not implemented yet!");
                break;
            case 2:
                await ShowSpeechBubble("This feature is not implemented yet!");
                break;
            case 3:
                if (watering.CanWaterTheCell(targetedCell, speechBubbleRenderer, speechBubbleText))
                {
                    playerAnimator.SetBool("isWatering", true);
                    await ProgressUpdate();
                    watering.HandleWatering(targetedCell);
                    playerAnimator.SetBool("isWatering", false);
                }
                break;
            default:
                break;
        }
    }
}
