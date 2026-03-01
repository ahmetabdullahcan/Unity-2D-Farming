using System;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


public class Farming
{
    private readonly string farmTileName;
    private readonly TileBase farmTile;
    private readonly Tilemap interactableTilemap;
    private readonly Animator playerAnimator;
    private readonly Tilemap decorationTilemap;


    public Farming(TileBase farmTile, Tilemap interactableTilemap, Animator playerAnimator, Tilemap decorationTilemap)
    {
        this.farmTile = farmTile;
        this.decorationTilemap = decorationTilemap;
        this.farmTileName = farmTile.name;
        this.interactableTilemap = interactableTilemap;
        this.playerAnimator = playerAnimator;
    }

    private void ReplaceTiles(Vector3Int cellPosition, Tilemap tilemap)
    {

        tilemap.SetTile(cellPosition, farmTile);

    }

    private void ClearDecorationTiles(Vector3Int cellPosition)
    {

        decorationTilemap.SetTile(cellPosition, null);
    }
    
    private bool CheckAreaTiles(Vector3Int cellPosition, TileBase[] hoeableTiles, Image speechBubbleRenderer, TextMeshProUGUI speechBubbleText)
    {
        if (!interactableTilemap.HasTile(cellPosition))
        {
            _ = ShowMessage(speechBubbleRenderer, speechBubbleText, 1000, "This area is already hoed!");
            return false;
        }

        TileBase currentTile = interactableTilemap.GetTile(cellPosition);

        if (currentTile == null)
        {
            _ = ShowMessage(speechBubbleRenderer, speechBubbleText, 1000, "This area can't be hoed!");
            return false;
        }

        if (currentTile.name.Equals(farmTileName))
        {
            _ = ShowMessage(speechBubbleRenderer, speechBubbleText, 1000, "This area is already hoed!");
            return false;
        }

        if (currentTile.name.StartsWith("Watered"))
        {
            _ = ShowMessage(speechBubbleRenderer, speechBubbleText, 1000, "This area is already hoed and watered!");
            return false;
        }

        if (!hoeableTiles.Any(tile => tile.name == currentTile.name))
        {
            _ = ShowMessage(speechBubbleRenderer, speechBubbleText, 1000, "This area can't be hoed!");
            return false;
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

    public bool CanFarmAtCell(Vector3Int cellPosition, Image speechBubbleRenderer, TextMeshProUGUI speechBubbleText, TileBase[] hoeableTiles)
    {
        if (!interactableTilemap.HasTile(cellPosition))
            return false;

        if (!CheckAreaTiles(cellPosition, hoeableTiles, speechBubbleRenderer, speechBubbleText))
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
        ClearDecorationTiles(cellPosition);
        ReplaceTiles(cellPosition, interactableTilemap);
    }
}


        