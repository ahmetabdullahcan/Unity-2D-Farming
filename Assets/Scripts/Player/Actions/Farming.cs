using System;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


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
    private bool CheckAreaTiles(Vector3Int cellPosition, TileBase hoeableTile, Image speechBubbleRenderer, TextMeshProUGUI speechBubbleText)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int checkPos = cellPosition + new Vector3Int(x, y, 0);
                if (!interactableTilemap.HasTile(checkPos) ||
                farmTileNames.Contains(interactableTilemap.GetTile(checkPos).name))
                {
                    _ = ShowMessage(speechBubbleRenderer, speechBubbleText, 1000, "This area is already hoed!");
                    return false;
                }
                if (interactableTilemap.GetTile(checkPos).name.StartsWith("Watered"))
                {
                    _ = ShowMessage(speechBubbleRenderer, speechBubbleText, 1000, "This area is already hoed and watered!");
                    return false;
                }
                if (interactableTilemap.GetTile(checkPos).name != hoeableTile.name)
                {
                    _ = ShowMessage(speechBubbleRenderer, speechBubbleText, 1000, "This area can't be hoed!");
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

    private async Task ShowMessage(Image speechBubbleRenderer, TextMeshProUGUI speechBubbleText, int delay, string message)
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
        await Task.Delay(delay);
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

    public bool CanFarmAtCell(Vector3Int cellPosition, Image speechBubbleRenderer, TextMeshProUGUI speechBubbleText, TileBase hoeableTile)
    {
        if (!interactableTilemap.HasTile(cellPosition))
            return false;

        if (!CheckAreaTiles(cellPosition, hoeableTile, speechBubbleRenderer, speechBubbleText))
            return false;

        if (!IsPlayerInRange(cellPosition))
        {
            _ = ShowMessage(speechBubbleRenderer, speechBubbleText, 1000, "Too far away!");
            return false;
        }

        return true;
    }

    public void HandleFarming(Vector3Int cellPosition)
    {
        ReplaceTiles(cellPosition);
    }
}


        