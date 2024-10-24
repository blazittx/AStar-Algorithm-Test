using System.Collections.Generic;
using UnityEngine;

public abstract class Node
{
    public abstract bool Execute();
}

public class Selector : Node
{
    private List<Node> nodes;

    public Selector(List<Node> nodes)
    {
        this.nodes = nodes;
    }

    public override bool Execute()
    {
        foreach (var node in nodes)
        {
            if (node.Execute())
            {
                return true; // If any node succeeds, return true.
            }
        }
        return false; // All nodes failed.
    }
}