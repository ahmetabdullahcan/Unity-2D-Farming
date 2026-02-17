using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TreeClusterGenerator : MonoBehaviour
{
    [SerializeField] private BaseWorldGen worldGenerator;
    [SerializeField] private Tilemap referenceTilemap;
    [SerializeField] private Tilemap treeTilemap;
    [SerializeField] private TileBase treeTile;
    [SerializeField] private int clusterCount = 20;
    [SerializeField] private int clusterRadius = 3;
    [SerializeField] private int treesPerCluster = 6;
    [SerializeField] private float minDistance = 1.5f;


    private void OnEnable()
    {
        worldGenerator.OnWorldGenerated += GenerateClusters;
    }

    private void OnDisable()
    {
        worldGenerator.OnWorldGenerated -= GenerateClusters;
    }

    private void GenerateClusters()
    {
        BoundsInt bounds = referenceTilemap.cellBounds;

        for (int i = 0; i < clusterCount; i++)
        {
            Vector3Int center = new Vector3Int(
                Random.Range(bounds.xMin, bounds.xMax),
                Random.Range(bounds.yMin, bounds.yMax),
                0);

            List<Vector3Int> placedTrees = new();

            for (int t = 0; t < treesPerCluster; t++)
            {
                Vector3Int randomPos = center + new Vector3Int(
                    Random.Range(-clusterRadius, clusterRadius + 1),
                    Random.Range(-clusterRadius, clusterRadius + 1),
                    0);

                if (!bounds.Contains(randomPos)) continue;

                bool tooClose = false;

                foreach (var existing in placedTrees)
                {
                    if (Vector3Int.Distance(existing, randomPos) < minDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (tooClose) continue;

                treeTilemap.SetTile(randomPos, treeTile);
                placedTrees.Add(randomPos);
            }
        }
    }
}
