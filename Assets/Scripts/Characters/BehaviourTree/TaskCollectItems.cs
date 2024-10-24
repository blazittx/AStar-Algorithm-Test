using UnityEngine;

public class TaskCollectItems : Node
{
    private Kim kim;

    public TaskCollectItems(Kim kim)
    {
        this.kim = kim;
    }

    public override bool Execute()
    {
        if (kim.allItemsCollected)
        {
            return false;
        }

        // Check if there's a current target or if the current target is invalid
        if (kim.currentTarget == null || !kim.pathfinding.IsPathValid())
        {
            GameObject nextItem = GetNextItem();
            if (nextItem != null)
            {
                kim.SetPathToTarget(nextItem.transform.position);
                kim.currentTarget = nextItem;
                Debug.Log("Going For Target: " + nextItem.name);
                return true; // Task is running
            }
            else
            {
                return false; // No more items to collect
            }
        }

        // Move towards the item and check if it's collected
        if (Vector3.Distance(kim.transform.position, kim.currentTarget.transform.position) < 1)
        {
            kim.CollectItem(kim.currentTarget);
            kim.currentTarget = null; // Clear the current target
            return true;
        }

        return true; // Continue executing until the target is reached
    }

    private GameObject GetNextItem()
    {
        // Logic to get the closest item or the next target.
        if (kim.allItems.Count > 0)
        {
            return kim.GetClosestItem();
        }
        return null;
    }
}