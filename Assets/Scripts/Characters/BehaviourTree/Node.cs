using System.Collections.Generic;

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
                return true;
            }
        }
        return false;
    }
}

public class Parallel : Node
{
    private List<Node> nodes;

    public Parallel(List<Node> nodes)
    {
        this.nodes = nodes;
    }

    public override bool Execute()
    {
        bool allSucceeded = true;

        foreach (var node in nodes)
        {
            if (!node.Execute())
            {
                allSucceeded = false;
            }
        }

        return allSucceeded;
    }
}