using UnityEngine;

public class FauxGravityBody : MonoBehaviour
{
    [SerializeField] private FauxGravityAttractor _attractor;
    
    private Transform _transform;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _rigidbody.useGravity = false;
    }
    
    private void FixedUpdate() => _attractor.Attract(_transform);
}