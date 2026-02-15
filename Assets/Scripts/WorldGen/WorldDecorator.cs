using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldDecorator : MonoBehaviour
{
    [SerializeField] private BaseWorldGen worldGenerator;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private TileBase[] grassTiles;

    private void OnEnable()
    {
        worldGenerator.OnWorldGenerated += FillGrass;
    }

    private void OnDisable()
    {
        worldGenerator.OnWorldGenerated -= FillGrass;
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
}
