using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PathfinderSphere pathfinder;
    [SerializeField] private LayerMask planetLayer;

    private Camera cam;

    private List<Node> movePath;
    
    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            bool start = Physics.Raycast(transform.position, -transform.up, out RaycastHit startHit,
                5f, planetLayer);

            bool target = Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit targetHit,
                Mathf.Infinity, planetLayer);

            if (start && target)
                Move(pathfinder.FindPath(startHit, targetHit));
        }
    }

    private void Move(List<Node> path)
    {
        movePath = path;
    }
}
