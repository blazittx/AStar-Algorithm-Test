using System.Collections.Generic;

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