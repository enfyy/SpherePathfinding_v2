using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathfinderSphere : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask planetLayer;
    
    private List<Node> highlightedNodes;
    private List<Node> highlightedPath;
    private List<Node> unwalkableNodes;

    private Camera cam;
    
    private SphereGrid Grid { get; set; }

    private void Awake()
    {
        Grid = SphereGrid.Generate(meshFilter, transform, radius);
        highlightedNodes = new List<Node>();
        unwalkableNodes = new List<Node>();
        cam = Camera.main;
    }

    private void Update()
    {
        //Setting Nodes as unwalkable
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            bool hit = Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit targetHit,
                Mathf.Infinity, planetLayer);
            if (!hit) return;
            
            Node hitNode = Grid.GetNodeFromRayCast(targetHit);
            if (unwalkableNodes.Contains(hitNode))
            {
                unwalkableNodes.Remove(hitNode);
                hitNode.Walkable = true;
            }
            else
            {
                unwalkableNodes.Add(hitNode);
                hitNode.Walkable = false;
            }
        }
    }

    public List<Node> FindPath(RaycastHit start, RaycastHit target)
    {
        Node s = Grid.GetNodeFromRayCast(start);
        Node t = Grid.GetNodeFromRayCast(target);
        highlightedNodes.Clear();
        highlightedNodes.Add(s);
        highlightedNodes.Add(t);
        return FindPath(s, t);
    }

    /// <summary>
    /// Finds the shortest path between the two nodes and returns a list of nodes that make up that path.
    /// First node in the list is the furthest distance from the target & closest to the start.
    /// </summary>
    public List<Node> FindPath(Node start, Node target)
    {
        Heap<Node> open = new Heap<Node>(Grid.NodeCount);
        HashSet<Node> closed = new HashSet<Node>();
        open.Add(start);
        
        while (open.Count > 0)
        {
            Node current = open.RemoveFirst();
            closed.Add(current);

            if (current == target)
            {
                highlightedPath?.Clear();
                highlightedPath = RetracePath(start, target);
                Debug.Log($"Path found with: {highlightedPath.Count} Nodes");
                return highlightedPath;
            }

            foreach (Node neighbour in current.Neighbours)
            {
                if (!neighbour.Walkable || closed.Contains(neighbour)) continue;

                float newMovementCostToNeighbour = current.gCost + current.Distance(neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !open.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = neighbour.Distance(target);
                    neighbour.parent = current;

                    if (!open.Contains(neighbour))
                        open.Add(neighbour);
                }
            }
        }

        return new List<Node>();
    }
    
    /// <summary>
    ///  Retraces the path of traversed nodes from the start node to the end node.
    /// </summary>
    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node current = endNode;

        while (current != startNode)
        {
            path.Add(current);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    private void HighlightNode(Node node, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(node.VertexPositions[0], node.VertexPositions[1]);
        Gizmos.DrawLine(node.VertexPositions[1], node.VertexPositions[2]);
        Gizmos.DrawLine(node.VertexPositions[2], node.VertexPositions[0]);
    }

    private void OnDrawGizmos()
    {
        if (highlightedPath != null)
        {
            foreach (Node n in highlightedPath)
                HighlightNode(n, Color.green);
        }

        if (highlightedNodes != null)
        {
             foreach (Node n in highlightedNodes)
                 HighlightNode(n, Color.blue);
        }

        if (unwalkableNodes != null)
        {
            foreach (Node n in unwalkableNodes)
                HighlightNode(n, Color.red);
        }

    }
}
