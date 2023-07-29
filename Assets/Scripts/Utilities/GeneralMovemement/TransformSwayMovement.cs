using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformSwayMovement : MonoBehaviour
{
    [SerializeField] private Vector3 swayAmount;
    [SerializeField] private Vector3 swaySpeed;

    private Vector3 halfSwayAmount;
    private Vector3 origin;

    private void Start()
    {
        halfSwayAmount = swayAmount / 2f;
        origin = transform.localPosition;
    }

    private void Update()
    {
        float x = 0, y = 0, z = 0;
        float st = Mathf.Sin(Time.time / 2f)/2f + .5f;
        //float xMovement = swayAmount.x * swaySpeed.x * Time.deltaTime * st;
        //float yMovement = swayAmount.y * swaySpeed.y * Time.deltaTime * st;
        //float zMovement = swayAmount.z * swaySpeed.z * Time.deltaTime * st;
        //Vector3 movement = new Vector3(xMovement, yMovement, zMovement);

        if (halfSwayAmount.x > 0f)
        {
            x = Mathf.PingPong(Time.time * swaySpeed.x, halfSwayAmount.x);// * Mathf.Sign(st);
        }
        if (halfSwayAmount.y > 0f)
        {
            y = Mathf.PingPong(Time.time * swaySpeed.y, halfSwayAmount.y);// * Mathf.Sign(st);
        }
        if (halfSwayAmount.z > 0f)
        {
            z = Mathf.PingPong(Time.time * swaySpeed.z, halfSwayAmount.z);// * Mathf.Sign(st);
        }

        Vector3 movement = new Vector3(x, y, z);

        transform.localPosition = origin + movement;
    }
}
