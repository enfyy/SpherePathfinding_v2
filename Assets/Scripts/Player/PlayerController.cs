using Pahtfinding;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PathfinderSphere _pathfinder;
    [SerializeField] private LayerMask _planetLayer;
    [SerializeField] private float _movementSpeed = 10f;
    [SerializeField] private float _playerHeight;
    [SerializeField] private Transform planetTransform;

    private Camera _camera;
    private Transform _transform;
    private Rigidbody _rigidbody;
    
    private bool _isWalking;            // true when player is on the way to the target location.
    private Node _currentNode;          // the node that the player is standing on.
    private Path<Node> _movePath;
    private Vector3 _positionOnGrid;

    private void Start()
    {
        _camera = Camera.main;
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();

        Assert.IsNotNull(planetTransform);
        Assert.IsNotNull(_pathfinder);
    }

    private void Update()
    {
        CheckActions();
    }

    private void FixedUpdate()
    {
        GetPositionOnGrid();
        Move(); //TODO: INVALID PATHS THROW ERRORS
    }

    private void GetPositionOnGrid()
    {
        if (!Physics.Raycast(_transform.position, -_transform.up, out RaycastHit hitInfo,
            5f, _planetLayer)) return;
        _positionOnGrid = hitInfo.point;
        _currentNode = _pathfinder.Grid.GetNodeFromRayCast(hitInfo);
    }

    /// <summary>
    /// Check for actions
    /// </summary>
    private void CheckActions()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            bool start = Physics.Raycast(_transform.position, -_transform.up, out RaycastHit startHit,
                5f, _planetLayer);

            bool target = Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit targetHit,
                Mathf.Infinity, _planetLayer);

            if (start && target)
                StartPath(_pathfinder.FindPath(startHit, targetHit));
        }
    }
    
    /// <summary>
    /// Start walking on a new path.
    /// </summary>
    /// <param name="path">List of nodes ordered by distance to the player (first => closest)</param>
    private void StartPath(Path<Node> path)
    {
        _movePath = path;
        _isWalking = true;
    }

    /// <summary>
    /// Move player along the path
    /// </summary>
    private void Move()
    {
        if (!_isWalking) return;
        float velocity = _movementSpeed * Time.deltaTime;

        if (Vector3.Distance(_movePath.First.WorldPosition, _positionOnGrid) < velocity)
        {
            _currentNode = _movePath.First;
            if (_currentNode == _movePath.Destination)
            {
                _isWalking = false;
            }
            else
            {
                _movePath.Next();
            }
        }

        Vector3 nextPosition = Vector3.MoveTowards(_positionOnGrid, _movePath.First.WorldPosition, velocity);
        _rigidbody.MovePosition(nextPosition + _transform.up * _playerHeight);
    }
}