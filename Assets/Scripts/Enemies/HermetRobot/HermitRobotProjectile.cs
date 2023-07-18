using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HermitRobotProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private int damage = 10;
    [SerializeField] private EntityTeam_t hittableTeams;
    private Rigidbody rb;

    private void Awake()
    {
        Destroy(gameObject, 3f);
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        EntityHealthController healthController = collision.gameObject.GetComponent<EntityHealthController>();
        Entity colEntity = collision.gameObject.GetComponent<Entity>();
        
        if (healthController == null || colEntity == null || (colEntity.EntityData.Team & hittableTeams) == 0)
        {
            Impact();
            return;
        }

        healthController.DamageAt(damage, transform.position);
        Impact();
    }

    private void Impact()
    {
        Destroy(gameObject);
    }
}
