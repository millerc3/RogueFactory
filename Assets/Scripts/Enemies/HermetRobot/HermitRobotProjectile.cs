using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HermitRobotProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 3f;

    private void Awake()
    {
        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
