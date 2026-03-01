using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


public class Watering
{
    private readonly TileBase wateredFarmTile;
    private readonly Tilemap interactableTilemap;
    private readonly Animator playerAnimator;
    private readonly TileBase waterableTile;


    public Watering(TileBase wateredFarmTile, TileBase waterableTile, Tilemap interactableTilemap, Animator playerAnimator)
    {
        this.wateredFarmTile = wateredFarmTile;
        this.interactableTilemap = interactableTilemap;
        this.playerAnimator = playerAnimator;
        this.waterableTile = waterableTile;
    }

    private void ReplaceTiles(Vector3Int cellPosition)
    {
        interactableTilemap.SetTile(cellPosition, wateredFarmTile);
    }
    private bool CheckAreaTiles(Vector3Int cellPosition)
    {
        Debug.Log(interactableTilemap.GetTile(cellPosition).name);
        if (interactableTilemap.GetTile(cellPosition).name != waterableTile.name)
            return false;
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

    public bool CanWaterTheCell(Vector3Int cellPosition, Image speechBubbleRenderer, TextMeshProUGUI speechBubbleText)
    {
        if (!interactableTilemap.HasTile(cellPosition))
            return false;

        if (!CheckAreaTiles(cellPosition))
        {
            _ = ShowMessage(speechBubbleRenderer, speechBubbleText, 1000, "You cannot water this area!");
            return false;
        }

        if (!IsPlayerInRange(cellPosition))
        {
            _ = ShowMessage(speechBubbleRenderer, speechBubbleText, 1000, "You are too far away to water the crops!");
            return false;
        }

        return true;
    }

    public void HandleWatering(Vector3Int cellPosition)
    {
        ReplaceTiles(cellPosition);
    }
}


        