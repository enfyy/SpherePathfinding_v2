using System;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public int HeapIndex { get; set; }
    public int NodeIndex { get; }                   // index of the Node aka. index of the triangle.
    public int[] VertexIndices { get; }             // indices of vertices that make up the triangle that is the node.
    public float GCost { get; set; }                // G-Cost of the node -> distance from starting node
    public float HCost { get; set; }                // H-Cost of the node -> distance from the goal node
    public float FCost => GCost + HCost;            // F-Cost of the node -> G-Cost + H-Cost
    public Node parent { get; set; }                // used to build a path of nodes.
    public Vector3[] VertexPositions { get; }       // World positions of the vertices that make up the node triangle.
    public Vector3 WorldPosition { get; }           // World position of the node 
    public List<Node> Neighbours { get; set; }      // The neighbouring nodes of this node
    public bool Walkable { get; set; }              // true when node can be parth of a walkable path

    private readonly SphereGrid _sphereGrid;         // Grid that contains the node
    
    public Node(int index, SphereGrid sphereGrid, int[] vertexIndices ,Vector3[] points)
    {
        NodeIndex = index;
        VertexIndices = vertexIndices;
        this._sphereGrid = sphereGrid;
        Walkable = true;
        
        // set vertex positions
        VertexPositions = new Vector3[3];
        VertexPositions[0] = sphereGrid.Transform.TransformPoint(points[0]);
        VertexPositions[1] = sphereGrid.Transform.TransformPoint(points[1]);
        VertexPositions[2] = sphereGrid.Transform.TransformPoint(points[2]);

        //calc world Pos
        WorldPosition = Vector3.zero;
        foreach (Vector3 p in points)
            WorldPosition += p;
        WorldPosition /= points.Length;
    }

    /// <summary>
    /// Calculates the distance between two Nodes
    /// </summary>
    public float Distance(Node otherNode)
    {
        if (_sphereGrid != otherNode._sphereGrid) throw new Exception("Nodes are not on the same Grid.");

        Vector3 nodeToCenter      = _sphereGrid.Center - WorldPosition;
        Vector3 otherNodeToCenter = _sphereGrid.Center - otherNode.WorldPosition;
        return Vector3.Angle(nodeToCenter.normalized, otherNodeToCenter.normalized) * _sphereGrid.OneDegreeDistance;
    }

    public int CompareTo(Node other)
    {
        int compare = FCost.CompareTo(other.FCost);
        if (compare == 0)
            compare = HCost.CompareTo(other.HCost);
        return -compare;
    }

}
