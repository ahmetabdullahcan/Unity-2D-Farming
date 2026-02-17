using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldDecorator : MonoBehaviour
{
    [SerializeField] private BaseWorldGen worldGenerator;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private TileBase[] grassTiles;
    [SerializeField] private TileBase[] decorationTiles;
    [SerializeField] private Tilemap decorationTilemap;

    private void OnEnable()
    {
        worldGenerator.OnWorldGenerated += FillGrass;
        worldGenerator.OnWorldGenerated += FillDecoration;
    }

    private void OnDisable()
    {
        worldGenerator.OnWorldGenerated -= FillGrass;
        worldGenerator.OnWorldGenerated -= FillDecoration;
    }

    private void FillGrass()
    {
        var bounds = groundTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                var pos = new Vector3Int(x, y, 0);

                if (!groundTilemap.HasTile(pos)) 
                    continue;

                if (Random.value < 0.1f)
                    groundTilemap.SetTile(pos, grassTiles[Random.Range(0, grassTiles.Length)]);
            }
        }
    }

    private void FillDecoration()
    {
        var bounds = groundTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                var pos = new Vector3Int(x, y, 0);

                if (!groundTilemap.HasTile(pos)) 
                    continue;

                if (!groundTilemap.GetTile(pos).name.Equals("Grass_Middle_0"))
                    continue;

                if (Random.value < 0.05f)
                    decorationTilemap.SetTile(pos, decorationTiles[Random.Range(0, decorationTiles.Length)]);
            }
        }
    }
}
