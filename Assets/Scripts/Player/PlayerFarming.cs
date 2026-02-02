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

    [Header("Menu")]
    [SerializeField] private GameObject interactableUI;

    private Camera mainCamera;
    private Vector3Int lastHighlightedCell;
    private bool hasHighlight;

    private bool isMenuOpen = false;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        UpdateHighlight();
        HandleInput();
    }

    private void GetMenuState()
    {
        if (interactableUI == null)
            return;
        if (interactableUI.activeSelf)
            isMenuOpen = true;
        else
            isMenuOpen = false;
    }

    private void HandleInput()
    {
        GetMenuState();
        if (isMenuOpen)
            return;

        if (!playerInput.actions["Hoe"].WasPerformedThisFrame())
            return;

        if (!hasHighlight)
            return;

        ReplaceTile(lastHighlightedCell, farmTile);
    }

    private void UpdateHighlight()
    {
        GetMenuState();
        if (isMenuOpen)
        {
            ClearHighlight();
            return;
        }
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
}
