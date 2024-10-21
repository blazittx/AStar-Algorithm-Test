using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kim : CharacterController
{
    [SerializeField] float ContextRadius;
    [SerializeField] float speed = 3f;

    private Pathfinding pathfinding;
    private int currentPathIndex = 0;
    public Animator animator;

    public override void StartCharacter()
    {
        base.StartCharacter();
        pathfinding = FindObjectOfType<Pathfinding>(); // Find the Pathfinding component in the scene
    }

    public override void UpdateCharacter()
    {
        base.UpdateCharacter();

        // Check if there is a path to follow
        if (pathfinding.path != null && pathfinding.path.Count > 0)
        {
            FollowPath();
        }
        else
        {
            animator.SetBool("Walk", false); // Stop walking if no path
        }

        Zombie closest = GetClosest(GetContextByTag("Zombie"))?.GetComponent<Zombie>();
    }

    void FollowPath()
    {
        // Check if we are at the last point in the path
        if (currentPathIndex < pathfinding.path.Count)
        {
            Grid.Tile targetTile = pathfinding.path[currentPathIndex];
            Vector3 targetPosition = Grid.Instance.WorldPos(targetTile);

            // Calculate the direction to face
            Vector3 direction = (targetPosition - transform.position).normalized;

            // Move towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Rotate Kim to face the direction she is moving
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
            }

            // Check if we have reached the target tile
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentPathIndex++; // Move to the next tile in the path
            }

            // Set the "Walk" bool to true while moving
            animator.SetBool("Walk", true);
        }
        else
        {
            // No more tiles to follow, stop walking
            animator.SetBool("Walk", false);
        }
    }

    Vector3 GetEndPoint()
    {
        return Grid.Instance.WorldPos(Grid.Instance.GetFinishTile());
    }

    GameObject[] GetContextByTag(string aTag)
    {
        Collider[] context = Physics.OverlapSphere(transform.position, ContextRadius);
        List<GameObject> returnContext = new List<GameObject>();
        foreach (Collider c in context)
        {
            if (c.transform.CompareTag(aTag))
            {
                returnContext.Add(c.gameObject);
            }
        }
        return returnContext.ToArray();
    }

    GameObject GetClosest(GameObject[] aContext)
    {
        float dist = float.MaxValue;
        GameObject Closest = null;
        foreach (GameObject z in aContext)
        {
            float curDist = Vector3.Distance(transform.position, z.transform.position);
            if (curDist < dist)
            {
                dist = curDist;
                Closest = z;
            }
        }
        return Closest;
    }
}
