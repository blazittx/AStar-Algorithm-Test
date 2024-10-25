    using UnityEngine;
    using System.Collections.Generic;

    public class Pathfinding : MonoBehaviour
    {
        public Transform seeker;
        public List<Grid.Tile> path = new List<Grid.Tile>();

        public void FindPathToTarget(Vector3 targetPosition, List<Grid.Tile> dangerTiles)
        {
            FindPath(seeker.position, targetPosition, dangerTiles);
        }

        public void FindPath(Vector3 startPos, Vector3 targetPos, List<Grid.Tile> dangerTiles)
        {
            Grid.Tile startTile = Grid.Instance.GetClosest(startPos);
            Grid.Tile targetTile = Grid.Instance.GetClosest(targetPos);

            if (startTile == null || targetTile == null)
            {
                path.Clear();
                return;
            }

            List<Grid.Tile> openSet = new List<Grid.Tile>();
            HashSet<Grid.Tile> closedSet = new HashSet<Grid.Tile>();
            openSet.Add(startTile);

            Dictionary<Grid.Tile, Grid.Tile> cameFrom = new Dictionary<Grid.Tile, Grid.Tile>();
            Dictionary<Grid.Tile, int> gScore = new Dictionary<Grid.Tile, int>();
            Dictionary<Grid.Tile, int> fScore = new Dictionary<Grid.Tile, int>();

            gScore[startTile] = 0;
            fScore[startTile] = GetDistance(startTile, targetTile);

            while (openSet.Count > 0)
            {
                Grid.Tile currentTile = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (fScore[openSet[i]] < fScore[currentTile])
                    {
                        currentTile = openSet[i];
                    }
                }

                openSet.Remove(currentTile);
                closedSet.Add(currentTile);

                if (currentTile == targetTile)
                {
                    RetracePath(startTile, targetTile, cameFrom);
                    return;
                }

                foreach (Grid.Tile neighbor in GetNeighbours(currentTile))
                {
                    if (neighbor.occupied || closedSet.Contains(neighbor) || (dangerTiles != null && dangerTiles.Contains(neighbor)))
                        continue;

                    int tentativeGScore = gScore[currentTile] + GetDistance(currentTile, neighbor);
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                    else if (tentativeGScore >= gScore[neighbor])
                    {
                        continue;
                    }

                    cameFrom[neighbor] = currentTile;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + GetDistance(neighbor, targetTile);
                }
            }

            path.Clear();
        }

        void RetracePath(Grid.Tile startTile, Grid.Tile endTile, Dictionary<Grid.Tile, Grid.Tile> cameFrom)
        {
            path.Clear();
            Grid.Tile currentTile = endTile;

            while (currentTile != startTile)
            {
                path.Add(currentTile);
                currentTile = cameFrom[currentTile];
            }
            path.Add(startTile);
            path.Reverse();
        }

        int GetDistance(Grid.Tile tileA, Grid.Tile tileB)
        {
            int dstX = Mathf.Abs(tileA.x - tileB.x);
            int dstY = Mathf.Abs(tileA.y - tileB.y);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }

        List<Grid.Tile> GetNeighbours(Grid.Tile tile)
        {
            List<Grid.Tile> neighbours = new List<Grid.Tile>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    Vector2Int checkPos = new Vector2Int(tile.x + x, tile.y + y);
                    Grid.Tile neighbour = Grid.Instance.TryGetTile(checkPos);
                    if (neighbour != null && !neighbour.occupied)
                    {
                        neighbours.Add(neighbour);
                    }
                }
            }

            return neighbours;
        }

        public bool IsPathValid()
        {
            return path != null && path.Count > 0;
        }
    }
