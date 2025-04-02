using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderworldSafety : MonoBehaviour
{
    public float distanceToLand = 10f;

    private void OnTriggerEnter(Collider other)
    {
        Transform detected = other.gameObject.transform;

        detected.position = new Vector3(detected.position.x, transform.position.y + distanceToLand, detected.position.z);
    }
}
