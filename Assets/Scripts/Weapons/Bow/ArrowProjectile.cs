using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class ArrowProjectile : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void Launch(Vector3 targetPosition, float launchForce)
    {
        transform.LookAt(targetPosition);
        rb.AddForce(transform.forward * launchForce, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
