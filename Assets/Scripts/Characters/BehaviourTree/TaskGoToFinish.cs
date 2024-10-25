using UnityEngine;

public class TaskGoToFinish : Node
{
    private Kim kim;
    private bool isTaskCompleted = false;

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

        Grid.Tile finishTile = Grid.Instance.GetFinishTile();
        if (finishTile != null)
        {
            Vector3 finishPosition = Grid.Instance.WorldPos(finishTile);
            kim.SetPathToTarget(finishPosition);

            Debug.Log("Setting path to finish tile at: " + finishPosition.ToString());

            isTaskCompleted = true;
            return true;
        }
        else
        {
            Debug.LogError("No finish tile found!");
            return false;
        }
    }
}