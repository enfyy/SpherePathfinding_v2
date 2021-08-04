using System.Collections.Generic;
using Pahtfinding;
using UnityEngine;

public class PathfinderSphere : MonoBehaviour
{
    [SerializeField, Tooltip("The MeshFilter of the sphere")]
    private MeshFilter _meshFilter;
    
    [SerializeField, Tooltip("The radius of the sphere")]
    public float Radius;
    
    [SerializeField, Tooltip("Layer of the planet")] 
    private LayerMask _planetLayer;
    
    [SerializeField, Tooltip("Node neighbours share: [Vertices || Edges]")] 
    private NeighbourMode _neighbourMode;
    
    [SerializeField, Tooltip("enables/disables drawing the gizmos for the path")]
    private bool drawDebug;
    
    private List<Node> _highlightedNodes;
    private Path<Node> _highlightedPath;
    private List<Node> _unwalkableNodes;

    private Camera cam;
    
    public SphereGrid Grid { get; private set; }

    private void Awake()
    {
        Grid = SphereGrid.Generate(_meshFilter, transform, Radius, _neighbourMode);
        _highlightedNodes = new List<Node>();
        _unwalkableNodes = new List<Node>();
        cam = Camera.main;
    }

    private void Update()
    {
        //Setting Nodes as unwalkable
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            bool hit = Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit targetHit,
                Mathf.Infinity, _planetLayer);
            if (!hit) return;
            
            Node hitNode = Grid.GetNodeFromRayCast(targetHit);
            if (_unwalkableNodes.Contains(hitNode))
            {
                _unwalkableNodes.Remove(hitNode);
                hitNode.Walkable = true;
            }
            else
            {
                _unwalkableNodes.Add(hitNode);
                hitNode.Walkable = false;
            }
        }
    }

    public Path<Node> FindPath(RaycastHit start, RaycastHit target)
    {
        Node s = Grid.GetNodeFromRayCast(start);
        Node t = Grid.GetNodeFromRayCast(target);
        _highlightedNodes.Clear();
        _highlightedNodes.Add(s);
        _highlightedNodes.Add(t);
        return FindPath(s, t);
    }

    /// <summary>
    /// Finds the shortest path between the two nodes and returns a list of nodes that make up that path.
    /// First node in the list is the furthest distance from the target & closest to the start.
    /// </summary>
    public Path<Node> FindPath(Node start, Node target)
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
                _highlightedPath?.Clear();
                _highlightedPath = RetracePath(start, target);
                Debug.Log($"Path found with: {_highlightedPath.Count} Nodes");
                return _highlightedPath;
            }

            foreach (Node neighbour in current.Neighbours)
            {
                if (!neighbour.Walkable || closed.Contains(neighbour)) continue;

                float newMovementCostToNeighbour = current.GCost + current.Distance(neighbour);
                if (newMovementCostToNeighbour < neighbour.GCost || !open.Contains(neighbour))
                {
                    neighbour.GCost = newMovementCostToNeighbour;
                    neighbour.HCost = neighbour.Distance(target);
                    neighbour.parent = current;

                    if (!open.Contains(neighbour))
                        open.Add(neighbour);
                }
            }
        }

        return new Path<Node>();
    }
    
    /// <summary>
    ///  Retraces the path of traversed nodes from the start node to the end node.
    /// </summary>
    private Path<Node> RetracePath(Node startNode, Node endNode)
    {
        Path<Node> path = new Path<Node>();
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
        if (!drawDebug) return;
        
        if (_highlightedPath != null)
        {
            foreach (Node n in _highlightedPath)
                HighlightNode(n, Color.green);
        }

        if (_highlightedNodes != null)
        {
             foreach (Node n in _highlightedNodes)
                 HighlightNode(n, Color.blue);
        }

        if (_unwalkableNodes != null)
        {
            foreach (Node n in _unwalkableNodes)
                HighlightNode(n, Color.red);
        }

    }
}
