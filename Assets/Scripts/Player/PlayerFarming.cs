using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerFarming : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap interactableTilemap;
    [SerializeField] private Tilemap highlightTilemap;

    [Header("Tiles")]
    [SerializeField] private TileBase farmTile;
    [SerializeField] private TileBase highlightTile;


    [Header("Experimental")]
    [SerializeField] private Sprite[] progressRingBar;
    [SerializeField] private SpriteRenderer progressRingRenderer;
    [SerializeField] private float maxDistance = 1.5f;
    [SerializeField] private TextMeshProUGUI speechBubbleText;
    [SerializeField] private SpriteRenderer speechBubbleRenderer;

    [Header("Animations")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;

    private Camera mainCamera;
    private Vector3Int lastHighlightedCell;
    private bool hasHighlight;

    private bool isProgressing = false;


    private void Awake()
    {
        mainCamera = Camera.main;
        speechBubbleRenderer.enabled = false;
    }

    private void LateUpdate()
    {
        UpdateHighlight();
        HandleInput();
    }

    private float GetDistanceToCell(Vector3Int cell)
    {
        Vector3 cellWorldPos = interactableTilemap.CellToWorld(cell) + interactableTilemap.tileAnchor;
        return Vector3.Distance(transform.position, cellWorldPos);
    }


    private async Task ProgressUpdate()
    {
        GetLookDirection();
        playerAnimator.SetBool("isHoeing", true);
        isProgressing = true;
        playerInput.actions.Disable();
        for (int i = 0; i < progressRingBar.Length; i++)
        {
            progressRingRenderer.sprite = progressRingBar[i];
            await Task.Delay(300);
        }
        isProgressing = false;
        playerInput.actions.Enable();
        progressRingRenderer.sprite = null;
        playerAnimator.SetBool("isHoeing", false);
    }


    private async void HandleInput()
    {
        if (!playerInput.actions["Hoe"].WasPerformedThisFrame())
            return;

        if (!hasHighlight)
            return;
        if (GetDistanceToCell(lastHighlightedCell) > maxDistance)
        {
            speechBubbleRenderer.enabled = true;
            speechBubbleText.text = "Too far!";
            await Task.Delay(1000);
            speechBubbleText.text = "";
            speechBubbleRenderer.enabled = false;
            return;
        }
        lastHighlightedCell = GetTargetedTile();
        if (interactableTilemap.GetTile(lastHighlightedCell).name.Equals("FarmLand_Tile_4"))
            return;
        await ProgressUpdate();
        ReplaceTile(lastHighlightedCell, farmTile);
    }

    private void UpdateHighlight()
    {
        if (isProgressing)
            return;

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

    private void ReplaceTile(Vector3Int cellPosition, TileBase newTile)
    {
        interactableTilemap.SetTile(cellPosition, newTile);
    }

    private void GetLookDirection()
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
}
