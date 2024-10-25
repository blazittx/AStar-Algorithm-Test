using UnityEngine;
using System.Collections.Generic;

public class TaskAvoidZombies : Node
{
    private Kim kim;
    private GameObject dangerMarkerPrefab;
    private List<Grid.Tile> dangerTiles = new List<Grid.Tile>();
    private List<GameObject> activeDangerMarkers = new List<GameObject>();

    public TaskAvoidZombies(Kim kim, GameObject dangerMarkerPrefab)
    {
        this.kim = kim;
        this.dangerMarkerPrefab = dangerMarkerPrefab;
    }

    public override bool Execute()
    {
        ResetDangerMarkers();

        var zombies = new List<GameObject>(kim.GetContextByTag("Zombie"));

        if (zombies.Count == 0) return true;

        dangerTiles.Clear();

        foreach (GameObject zombie in zombies)
        {
            Grid.Tile zombieTile = Grid.Instance.GetClosest(zombie.transform.position);

            MarkDangerZone(zombieTile);
        }

        kim.UpdateDangerTiles(dangerTiles);

        return true;
    }

    private void ResetDangerMarkers()
    {
        foreach (var marker in activeDangerMarkers)
        {
            GameObject.Destroy(marker);
        }
        activeDangerMarkers.Clear();
    }

    private void MarkDangerZone(Grid.Tile zombieTile)
    {
        int dangerRadius = 2;

        for (int x = -dangerRadius; x <= dangerRadius; x++)
        {
            for (int y = -dangerRadius; y <= dangerRadius; y++)
            {
                Vector2Int tilePos = new Vector2Int(zombieTile.x + x, zombieTile.y + y);
                Grid.Tile dangerTile = Grid.Instance.TryGetTile(tilePos);

                if (dangerTile != null)
                {
                    dangerTiles.Add(dangerTile);

                    Vector3 tileWorldPos = Grid.Instance.WorldPos(dangerTile);
                    GameObject marker = GameObject.Instantiate(dangerMarkerPrefab, tileWorldPos, Quaternion.identity);
                    activeDangerMarkers.Add(marker);
                }
            }
        }
    }
}
