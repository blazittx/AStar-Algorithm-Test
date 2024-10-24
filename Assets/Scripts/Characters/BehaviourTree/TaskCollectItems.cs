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

        if (kim.currentTarget == null || !kim.pathfinding.IsPathValid())
        {
            GameObject nextItem = GetNextItem();
            if (nextItem != null)
            {
                kim.SetPathToTarget(nextItem.transform.position);
                kim.currentTarget = nextItem;
                Debug.Log("Going For Target: " + nextItem.name);
                return true;
            }
            else
            {
                return false;
            }
        }

        if (Vector3.Distance(kim.transform.position, kim.currentTarget.transform.position) < 1)
        {
            kim.CollectItem(kim.currentTarget);
            kim.currentTarget = null;
            return true;
        }

        return true;
    }

    private GameObject GetNextItem()
    {
        if (kim.allItems.Count > 0)
        {
            return kim.GetClosestItem();
        }
        return null;
    }
}