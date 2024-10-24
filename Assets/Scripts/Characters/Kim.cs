using System.Collections.Generic;
using UnityEngine;

public class Kim : CharacterController
{
    [Header("Character Context Radius")]
    [SerializeField] private float contextRadius = 10f;

    [Header("References")]
    public Pathfinding pathfinding;
    public Animator animator;
    [SerializeField] private GameObject pathMarkerPrefab;
    [SerializeField] private GameObject dangerMarkerPrefab;

    private List<GameObject> activePathMarkers = new List<GameObject>();

    [Header("Item and Zombie Management")]
    public List<GameObject> allItems = new List<GameObject>();
    public GameObject currentTarget;
    public bool allItemsCollected = false;
    
    private List<Grid.Tile> dangerTiles = new List<Grid.Tile>();

    // Store the previous path for comparison
    private List<Grid.Tile> previousPath = new List<Grid.Tile>();

    private Node rootNode;

    public override void StartCharacter()
    {
        base.StartCharacter();
        pathfinding = GetComponent<Pathfinding>();

        allItems = new List<GameObject>(GetContextByTag("Burger"));

        BuildBehaviorTree();
    }

    private void BuildBehaviorTree()
    {
        rootNode = new Parallel(new List<Node>
        {
            new TaskAvoidZombies(this, dangerMarkerPrefab),
            new Selector(new List<Node>
            {
                new TaskCollectItems(this),
                new TaskGoToFinish(this)
            })
        });
    }

    private void VisualizePath()
    {
        foreach (var marker in activePathMarkers)
        {
            Destroy(marker);
        }
        activePathMarkers.Clear();

        foreach (Grid.Tile tile in pathfinding.path)
        {
            Vector3 markerPosition = Grid.Instance.WorldPos(tile);
            GameObject marker = Instantiate(pathMarkerPrefab, markerPosition, Quaternion.identity);
            activePathMarkers.Add(marker);
        }
    }

    public override void UpdateCharacter()
    {
        base.UpdateCharacter();

        // Execute the behavior tree
        rootNode.Execute();

        // Check if the path needs updating
        UpdateCurrentPath();

        // Visualize the path on the grid
        VisualizePath();
    }

    public void UpdateCurrentPath()
    {
        if (currentTarget != null)
        {
            pathfinding.FindPathToTarget(currentTarget.transform.position, dangerTiles);

            // Compare the new path with the previous path
            if (!PathsAreEqual(pathfinding.path, previousPath))
            {
                // If the path changed, update the walk buffer
                SetWalkBuffer(pathfinding.path);
                // Store the current path as the previous path for the next comparison
                previousPath = new List<Grid.Tile>(pathfinding.path);
            }
        }
        else if (allItemsCollected)
        {
            Grid.Tile finishTile = Grid.Instance.GetFinishTile();
            if (finishTile != null)
            {
                Vector3 finishPosition = Grid.Instance.WorldPos(finishTile);
                pathfinding.FindPathToTarget(finishPosition, dangerTiles);

                // Compare the new path with the previous path
                if (!PathsAreEqual(pathfinding.path, previousPath))
                {
                    // If the path changed, update the walk buffer
                    SetWalkBuffer(pathfinding.path);
                    // Store the current path as the previous path for the next comparison
                    previousPath = new List<Grid.Tile>(pathfinding.path);
                }
            }
            else
            {
                Debug.LogError("Finish tile not found!");
            }
        }
        else
        {
            Debug.Log("No valid target to update path.");
        }
    }

    // Helper method to compare paths
    private bool PathsAreEqual(List<Grid.Tile> pathA, List<Grid.Tile> pathB)
    {
        if (pathA.Count != pathB.Count)
        {
            return false;
        }

        for (int i = 0; i < pathA.Count; i++)
        {
            if (pathA[i] != pathB[i])
            {
                return false;
            }
        }

        return true;
    }

    public void UpdateDangerTiles(List<Grid.Tile> dangerTiles)
    {
        this.dangerTiles = new List<Grid.Tile>(dangerTiles);
    }

    public void SetPathToTarget(Vector3 targetPosition)
    {
        Debug.Log("SetPathToTarget Called");

        pathfinding.FindPathToTarget(targetPosition, dangerTiles);
        SetWalkBuffer(pathfinding.path);
        previousPath = new List<Grid.Tile>(pathfinding.path);  // Store the path as the previous path
    }

    public GameObject GetClosestItem()
    {
        float closestDistance = float.MaxValue;
        GameObject closestItem = null;

        // Find the closest item to Kim
        foreach (GameObject item in allItems)
        {
            float distance = Vector3.Distance(transform.position, item.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestItem = item;
            }
        }

        return closestItem;
    }

    public void CollectItem(GameObject item)
    {
        allItems.Remove(item);

        if (allItems.Count == 0)
        {
            allItemsCollected = true;
        }
        else
        {
            GameObject nextItem = GetClosestItem();
            if (nextItem != null)
            {
                SetPathToTarget(nextItem.transform.position);
                currentTarget = nextItem;
            }
        }
    }

    public GameObject[] GetContextByTag(string tag)
    {
        Collider[] context = Physics.OverlapSphere(transform.position, contextRadius);
        List<GameObject> result = new List<GameObject>();

        foreach (Collider collider in context)
        {
            if (collider.CompareTag(tag))
            {
                result.Add(collider.gameObject);
            }
        }

        return result.ToArray();
    }
}
