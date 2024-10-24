using System.Collections.Generic;
using UnityEngine;

public class Kim : CharacterController
{
    // === Serialized Fields ===
    [Header("Character Context Radius")]
    [SerializeField] private float contextRadius = 10f;

    // === Public Fields ===
    [Header("References")]
    public Pathfinding pathfinding;
    public Animator animator;
    [SerializeField] private GameObject pathMarkerPrefab;  // Assign a path marker prefab in the inspector
    private List<GameObject> activePathMarkers = new List<GameObject>(); // Store active path markers

    [Header("Item and Zombie Management")]
    public List<GameObject> allItems = new List<GameObject>();
    public List<GameObject> allZombies = new List<GameObject>(); // Store detected zombies
    public GameObject currentTarget;
    public bool allItemsCollected = false;

    // === Private Fields ===
    private Node rootNode;

    public override void StartCharacter()
    {
        base.StartCharacter();
        pathfinding = GetComponent<Pathfinding>();

        // Get all the items in the scene tagged as "Burger"
        allItems = new List<GameObject>(GetContextByTag("Burger"));

        // Get all zombies in the scene
        allZombies = new List<GameObject>(GetContextByTag("Zombie"));

        BuildBehaviorTree();
    }

    private void BuildBehaviorTree()
    {
        rootNode = new Selector(new List<Node>
        {
            new TaskCollectItems(this),
            new TaskAvoidZombies(this),   // New task for avoiding zombies
            new TaskGoToFinish(this)      // This task executes when all items are collected
        });
    }

    private void VisualizePath()
    {
        // Clear previous path markers
        foreach (var marker in activePathMarkers)
        {
            Destroy(marker);
        }
        activePathMarkers.Clear();

        // Place new path markers along the current path
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

        // Execute the behavior tree logic
        rootNode.Execute();
        
        // Dynamically update the path based on the current target (item or finish tile)
        UpdateCurrentPath();

        // Visualize the path
        VisualizePath();
    }

    public void UpdateCurrentPath()
    {
        if (currentTarget != null)
        {
            // If there is a current target (like an item), set the path to it
            pathfinding.FindPathToTarget(currentTarget.transform.position);
        }
        else if (allItemsCollected)
        {
            // If all items are collected and no current target, set path to the finish tile
            Grid.Tile finishTile = Grid.Instance.GetFinishTile();
            if (finishTile != null)
            {
                Vector3 finishPosition = Grid.Instance.WorldPos(finishTile);
                pathfinding.FindPathToTarget(finishPosition); // Set path to finish tile
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

    public void SetPathToTarget(Vector3 targetPosition)
    {
        Debug.Log("SetPathToTarget Called");

        // Find the path to the target and set it in the walk buffer
        pathfinding.FindPathToTarget(targetPosition);
        SetWalkBuffer(pathfinding.path);  // Use the existing walk buffer in CharacterController
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
        // Remove the collected item from the list
        allItems.Remove(item);

        if (allItems.Count == 0)
        {
            allItemsCollected = true; // All items have been collected
        }
        else
        {
            // Set the path for the next closest item
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
        // Find all objects in range with the specified tag
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
