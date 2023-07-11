using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupMagnet : MonoBehaviour
{
    [SerializeField] private float magnetRadius = 8f;
    [SerializeField] private LayerMask pickupItemLayerMask;

    private int framesSkippedPerMagnetCheck = 15;
    private int frameCounter = 0;
    [SerializeField] private float magnetStrength = 5f;

    private void Update()
    {
        if (frameCounter >= framesSkippedPerMagnetCheck)
        {
            CheckForItem();
            frameCounter = 0;
        }

        frameCounter++;
    }

    private void CheckForItem()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, magnetRadius, pickupItemLayerMask);
        foreach (Collider col in cols)
        {
            ItemPickup item = col.GetComponent<ItemPickup>();
            if (item == null) continue;

            StartCoroutine(MoveItemTowarsPlayer(item.transform));
        }
    }

    private IEnumerator MoveItemTowarsPlayer(Transform itemTransform)
    {
        while (itemTransform != null && Vector3.Distance(itemTransform.position, transform.position) > .25f)
        {
            itemTransform.position = Vector3.MoveTowards(itemTransform.position, transform.position, magnetStrength * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
