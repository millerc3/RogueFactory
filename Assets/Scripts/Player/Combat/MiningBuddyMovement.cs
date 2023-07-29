using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningBuddyMovement : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float speed = 5f;
    
    private void Update()
    {
        float d = Mathf.Sqrt(Vector3.Distance(transform.position, targetTransform.position));
        transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, speed * Time.deltaTime * d);
    }

    public Transform GetTargetTransform()
    {
        return targetTransform;
    }

    public void SetTargetTransform(Transform newTransform)
    {
        targetTransform = newTransform;
    }
}
