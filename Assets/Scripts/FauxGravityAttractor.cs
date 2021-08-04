using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FauxGravityAttractor : MonoBehaviour
{
    /// <summary>
    /// Attracts the FauxGravityBody and adjusts its rotation
    /// </summary>
    /// <param name="body"> Transform of the body</param>
    public void Attract(Transform body)
    {
        Vector3 centerPosition = transform.position;
        Vector3 gravityUp = (body.position - centerPosition).normalized;
        Vector3 bodyUp = body.up;

        Quaternion rotation = body.rotation;
        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * rotation;
        rotation = Quaternion.Slerp(rotation, targetRotation, 50 * Time.deltaTime);
        body.rotation = rotation;
    }
}
