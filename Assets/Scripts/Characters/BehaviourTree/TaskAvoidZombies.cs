using System.Collections.Generic;
using UnityEngine;

public class TaskAvoidZombies : Node
{
    private Kim kim;
    private List<GameObject> zombies;

    public TaskAvoidZombies(Kim kim)
    {
        this.kim = kim;
    }

    public override bool Execute()
    {
        // Get all zombies on the map
        zombies = new List<GameObject>(kim.GetContextByTag("Zombie"));

        // If there are no zombies, return true (no need to avoid)
        if (zombies.Count == 0)
        {
            return true;
        }

        // Mark the tiles around zombies as occupied
        foreach (GameObject zombie in zombies)
        {
            Grid.Tile zombieTile = Grid.Instance.GetClosest(zombie.transform.position);
            MarkDangerZone(zombieTile);
        }

        return true; // Task completed
    }

    private void MarkDangerZone(Grid.Tile zombieTile)
    {
        int dangerRadius = 2;  // 4x4 area means 2 tiles in each direction

        for (int x = -dangerRadius; x <= dangerRadius; x++)
        {
            for (int y = -dangerRadius; y <= dangerRadius; y++)
            {
                Vector2Int tilePos = new Vector2Int(zombieTile.x + x, zombieTile.y + y);
                Grid.Tile dangerTile = Grid.Instance.TryGetTile(tilePos);
                if (dangerTile != null)
                {
                    dangerTile.occupied = true; // Mark tile as occupied
                }
            }
        }
    }
}