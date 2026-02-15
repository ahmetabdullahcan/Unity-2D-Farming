using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseWorldGen : MonoBehaviour
{
    public TileBase[] cornerTiles;
    public TileBase[] borderTiles;
    public Tilemap borderTilemap;
    public TileBase centerTile;
    public Tilemap groundTilemap;
    public int mapWidth;
    public int mapHeight;
    public BoxCollider2D worldCollider;
    public CinemachineConfiner2D cinemachineConfiner;

    private void setCamBorders()
    {
        worldCollider.size = new Vector2(mapWidth, mapHeight);
        worldCollider.offset = Vector2.zero;
        cinemachineConfiner.InvalidateBoundingShapeCache();
        worldCollider.enabled = false;
    }

    private void fillCorners()
    {
        borderTilemap.SetTile(new Vector3Int(-mapWidth / 2, mapHeight / 2, 0), cornerTiles[0]);
        borderTilemap.SetTile(new Vector3Int(mapWidth / 2, mapHeight / 2, 0), cornerTiles[1]);
        borderTilemap.SetTile(new Vector3Int(mapWidth / 2, -mapHeight / 2, 0), cornerTiles[2]);
        borderTilemap.SetTile(new Vector3Int(-mapWidth / 2, -mapHeight / 2, 0), cornerTiles[3]);
    }

    private void fillBorders()
    {
        for (int x = -mapWidth / 2 + 1; x < mapWidth / 2; x++)
        {
            borderTilemap.SetTile(new Vector3Int(x, mapHeight / 2, 0), borderTiles[0]);
            borderTilemap.SetTile(new Vector3Int(x, -mapHeight / 2, 0), borderTiles[1]);
        }
        for (int y = -mapHeight / 2 + 1; y < mapHeight / 2; y++)
        {
            borderTilemap.SetTile(new Vector3Int(-mapWidth / 2, y, 0), borderTiles[2]);
            borderTilemap.SetTile(new Vector3Int(mapWidth / 2, y, 0), borderTiles[3]);
        }
    }

    private void fillCenter()
    {
        for (int x = -mapWidth / 2 + 1; x < mapWidth / 2; x++)
        {
            for (int y = -mapHeight / 2 + 1; y < mapHeight / 2; y++)
            {
                groundTilemap.SetTile(new Vector3Int(x, y, 0), centerTile);
            }
        }
    }

    void Start()
    {
        fillCorners();
        fillBorders();
        fillCenter();
        setCamBorders();
    }
}
