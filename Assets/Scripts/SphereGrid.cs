using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Grid consisting of <see cref="Node"/> used for A* Pathfinding.
/// Consists of the faces of the mesh of a sphere.
/// </summary>
public class SphereGrid
{
    public readonly float OneDegreeDistance;    // Distance of 1/360th of the circumference of the sphere.
    public Vector3 Center { get; }              // Center point of the sphere
    public Transform transform { get; }         // transform of the GameObject with the mesh.
    public int NodeCount => _nodes.Count;       // returns count of nodes in the grid.
    
    private Dictionary<int, Node> _nodes;       // nodeIndex -> Node
    private float radius;                       // radius of the sphere.

    /// <summary>
    /// Constructor
    /// </summary>
    private SphereGrid(Transform tf, float radius)
    {
        Center = tf.position;
        transform = tf;
        _nodes = new Dictionary<int, Node>();
        this.radius = radius;
        OneDegreeDistance = ((2f * radius * Mathf.PI) / 360f);
    }

    /// <summary>
    /// Generates the Grid and its Nodes from the triangles of the mesh.
    /// </summary>
    public static SphereGrid Generate(MeshFilter meshFilter, Transform tf, float radius)
    {
        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.GetTriangles(0);
        
        SphereGrid instance = new SphereGrid(tf, radius);
        
        //initialize dictionary of vertexIndex -> triangleIndex
        Dictionary<int, List<int>> vertexIndexToTriangleIndices = new Dictionary<int, List<int>>();
        // single vertex can have multiple indices
        Dictionary<Vector3, List<int>> vertexPosToIndices = new Dictionary<Vector3, List<int>>();
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            vertexIndexToTriangleIndices[i] = new List<int>();
            
            if (!vertexPosToIndices.ContainsKey(vertices[i]))
                vertexPosToIndices[vertices[i]] = new List<int>();
            vertexPosToIndices[vertices[i]].Add(i);
        }
        
        
        // for each triangle of the mesh, create a node and add to grid
        for (int i = 0; i < triangles.Length; i+=3)
        {
            int i1 = triangles[i + 0];
            int i2 = triangles[i + 1];
            int i3 = triangles[i + 2];
           
           // points in world space
           Vector3[] points = 
           {
               tf.TransformPoint(vertices[i1]),
               tf.TransformPoint(vertices[i2]),
               tf.TransformPoint(vertices[i3])
           };

           int[] indices = { i1, i2, i3 };
           int nodeIndex = Mathf.FloorToInt(i / 3f);
           
           // add to dictionary 
           foreach (int j in indices)
               vertexIndexToTriangleIndices[j].Add(nodeIndex);

           //create node and add to dictionary of nodeIndex -> Node
           Node n = new Node(nodeIndex, instance, indices, points);
           instance._nodes.Add(nodeIndex, n);
        }
        
        // FOR EACH NODE IN THE GRID : populate neighbour lists of the nodes
        foreach (Node n in instance._nodes.Values)
        {
            List<int> neighbourIndices = new List<int>();
            //FOR EACH VERTEX INDEX OF THE NODE
            foreach (int index in n.VertexIndices)
            {
                // GET POSITION OF THE VERTEX
                Vector3 key = vertices[index];
                // GET INDICES THAT SHARE THAT POSITION
                List<int> otherVertexIndices = vertexPosToIndices[key];

                // FOR EACH INDEX THAT SHARES THE POSITION
                foreach (int jndex in otherVertexIndices)
                {
                    // GET TRIANGLE INDICES THAT CONTAIN THIS VERTEX INDEX
                    neighbourIndices.AddRange(vertexIndexToTriangleIndices[jndex]);
                }
            }
            neighbourIndices = neighbourIndices.Distinct().ToList();
                
            n.Neighbours = neighbourIndices.Select(neighbourIndex => instance.GetNode(neighbourIndex)).Distinct().ToList();
        }

        return instance;
    }

    /// <summary>
    /// Tries to get a node by index.
    /// </summary>
    public Node GetNode(int index)
    {
        _nodes.TryGetValue(index, out Node n);
        return n;
    }

    /// <summary>
    /// Gets Node from the triangle index returned by a RaycastHit
    /// </summary>
    public Node GetNodeFromRayCast(RaycastHit hit)
    {
        if (hit.triangleIndex == -1) throw new Exception("Triangle Index == -1");
        return GetNode(hit.triangleIndex);
    }
}
