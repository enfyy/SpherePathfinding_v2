using System;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public int HeapIndex { get; set; }
    public int NodeIndex { get; }                   // index of the Node aka. index of the triangle.
    public int[] VertexIndices { get; }             // indices of vertices that make up the triangle that is the node.
    public float gCost { get; set; }                // G-Cost of the node -> distance from starting node
    public float hCost { get; set; }                // H-Cost of the node -> distance from the goal node
    public float fCost => gCost + hCost;            // F-Cost of the node -> G-Cost + H-Cost
    public Node parent { get; set; }                // used to build a path of nodes.
    public Vector3[] VertexPositions { get; }       // World positions of the vertices that make up the node triangle.
    public Vector3 WorldPosition { get; }           // World position of the node 
    public List<Node> Neighbours { get; set; }      // The neighbouring nodes of this node
    public bool Walkable { get; set; }              // true when node can be parth of a walkable path

    private readonly SphereGrid sphereGrid;                  // Grid that contains the node
    


    public Node(int index, SphereGrid sphereGrid, int[] vertexIndices , Vector3[] points)
    {
        NodeIndex = index;
        VertexIndices = vertexIndices;
        this.sphereGrid = sphereGrid;
        Walkable = true;
        
        // set vertex positions
        VertexPositions = new Vector3[3];
        VertexPositions[0] = sphereGrid.transform.TransformPoint(points[0]);
        VertexPositions[1] = sphereGrid.transform.TransformPoint(points[1]);
        VertexPositions[2] = sphereGrid.transform.TransformPoint(points[2]);

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
        if (sphereGrid != otherNode.sphereGrid) throw new Exception("Nodes are not on the same Grid.");

        Vector3 nodeToCenter      = sphereGrid.Center - WorldPosition;
        Vector3 otherNodeToCenter = sphereGrid.Center - otherNode.WorldPosition;
        return Vector3.Angle(nodeToCenter.normalized, otherNodeToCenter.normalized) * sphereGrid.OneDegreeDistance;
    }

    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0)
            compare = hCost.CompareTo(other.hCost);
        return -compare;
    }

}
