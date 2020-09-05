using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    public Manager manager;
    public NavNode[,] nodes;

    void Start() {
        //Get all nodes in scene
        //nodes = manager.GetNodes();
    }

    /// <summary>
    /// Find the shortest path between two nodes
    /// </summary>
    /// <param name="start">Starting Node</param>
    /// <param name="end">End node</param>
    /// <returns> List of nodes in order of shortest path</returns>
    public List<NavNode> FindPath(NavNode start, NavNode end) {

        //the end node in the path
        NavNode endNode = DijkstraPathfinding(start, end);

        //Create path by traversing parents from end node
        List<NavNode> result = new List<NavNode>();
        while (endNode.GetParentSet()) {
            result.Add(endNode);
            endNode = endNode.GetParentNode();
        }

        //Reverse the path
        result.Reverse();
        return result;
    }

    /// <summary>
    /// Uses Dijkstra's shortest path algorithm to create a path
    /// </summary>
    /// <param name="start">Starting Node</param>
    /// <param name="end">End node</param>
    /// <returns>The end node</returns>
    private NavNode DijkstraPathfinding(NavNode start, NavNode end) {

        //Nodes we need to check
        List<NavNode> unexplored = new List<NavNode>();
        foreach (NavNode node in nodes) {
            unexplored.Add(node);
        }

        //Starting node has no weighting
        start.SetWeight(0);

        //While we have nodes to check
        while (unexplored.Count > 0) {
            unexplored.Sort((x, y) => x.GetWeight().CompareTo(y.GetWeight()));
            NavNode current = unexplored[0];

            //Node is being explored
            unexplored.Remove(current);

            List<NavNode> neighbours = current.GetNeighbourNode();

            foreach (NavNode node in neighbours) {

                // Check if explored
                if (unexplored.Contains(node)) {
                    // Get the distance and height difference of the node
                    float weight = Vector3.Distance(node.GetPosition(), current.GetPosition());
                    float heightDiff = (current.GetPosition().y - node.GetPosition().y) * 100;
                    weight = current.GetWeight() + weight + heightDiff;

                    // If the new weight is less than the current weight.
                    if (weight < node.GetWeight()) {
                        // Set weight and update path by adding parent
                        node.SetWeight(weight);
                        node.SetParentNode(current);
                    }
                }

            }
        }
        //Return final node
        return end;
    }
}
