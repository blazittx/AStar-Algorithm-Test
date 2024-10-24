using UnityEngine;

public class TaskGoToFinish : Node
{
    private Kim kim;
    private bool isTaskCompleted = false;  // Flag to ensure the task runs only once

    public TaskGoToFinish(Kim kim)
    {
        this.kim = kim;
    }

    public override bool Execute()
    {
        if (isTaskCompleted)
        {
            return true;
        }

        // Get the finish tile from the grid
        Grid.Tile finishTile = Grid.Instance.GetFinishTile();
        if (finishTile != null)
        {
            Vector3 finishPosition = Grid.Instance.WorldPos(finishTile); // Get world position of the finish tile
            kim.SetPathToTarget(finishPosition); // Set Kim's path to the finish tile

            Debug.Log("Setting path to finish tile.");

            // Mark the task as completed
            isTaskCompleted = true;
            return true;  // Task is successfully running/completed
        }
        else
        {
            Debug.LogError("No finish tile found!");
            return false;  // Task failed because no finish tile was found
        }
    }
}