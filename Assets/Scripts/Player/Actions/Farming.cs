using System;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;


public class Farming
{
    private readonly string[] farmTileNames;
    private readonly TileBase[] farmTiles;
    private readonly Tilemap interactableTilemap;
    private readonly Animator playerAnimator;


    public Farming(TileBase[] farmTiles, Tilemap interactableTilemap, Animator playerAnimator)
    {
        this.farmTiles = farmTiles;
        farmTileNames = new string[farmTiles.Length];
        for (int i = 0; i < farmTiles.Length; i++)
        {
            farmTileNames[i] = farmTiles[i].name;
        }
        this.interactableTilemap = interactableTilemap;
        this.playerAnimator = playerAnimator;
    }

    private void ReplaceTiles(Vector3Int cellPosition)
    {
        interactableTilemap.SetTile(cellPosition + new Vector3Int(-1, 1, 0), farmTiles[0]);
        interactableTilemap.SetTile(cellPosition + new Vector3Int(0, 1, 0), farmTiles[1]);
        interactableTilemap.SetTile(cellPosition + new Vector3Int(1, 1, 0), farmTiles[2]);
        interactableTilemap.SetTile(cellPosition + new Vector3Int(-1, 0, 0), farmTiles[3]);
        interactableTilemap.SetTile(cellPosition, farmTiles[4]);
        interactableTilemap.SetTile(cellPosition + new Vector3Int(1, 0, 0), farmTiles[5]);
        interactableTilemap.SetTile(cellPosition + new Vector3Int(-1, -1, 0), farmTiles[6]);
        interactableTilemap.SetTile(cellPosition + new Vector3Int(0, -1, 0), farmTiles[7]);
        interactableTilemap.SetTile(cellPosition + new Vector3Int(1, -1, 0), farmTiles[8]);
    }
    private bool CheckAreaTiles(Vector3Int cellPosition)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int checkPos = cellPosition + new Vector3Int(x, y, 0);
                if (!interactableTilemap.HasTile(checkPos) ||
                farmTileNames.Contains(interactableTilemap.GetTile(checkPos).name))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool IsPlayerInRange(Vector3Int cellPosition, float maxDistance = 1.0f)
    {
        Vector3 playerPos = playerAnimator.transform.position;
        Vector3 tileWorldPos = interactableTilemap.CellToWorld(cellPosition) + interactableTilemap.tileAnchor;
        float distance = Vector3.Distance(playerPos, tileWorldPos);
        return distance <= maxDistance;
    }

    private async Task ShowTooFarMessage(SpriteRenderer speechBubbleRenderer, TextMeshProUGUI speechBubbleText, int delay = 1000)
    {
        speechBubbleRenderer.enabled = true;
        speechBubbleText.text = "Too far!";
        await Task.Delay(delay);
        speechBubbleRenderer.enabled = false;
        speechBubbleText.text = "";
    }

    public bool CanFarmAtCell(Vector3Int cellPosition, SpriteRenderer speechBubbleRenderer, TextMeshProUGUI speechBubbleText)
    {
        if (!interactableTilemap.HasTile(cellPosition))
            return false;

        if (!CheckAreaTiles(cellPosition))
            return false;

        if (!IsPlayerInRange(cellPosition))
        {
            _ = ShowTooFarMessage(speechBubbleRenderer, speechBubbleText);
            return false;
        }

        return true;
    }

    public void HandleFarming(Vector3Int cellPosition)
    {
        ReplaceTiles(cellPosition);
    }
}


        