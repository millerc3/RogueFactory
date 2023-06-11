using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [SerializeField] private ItemPickup itemPickup;
    [SerializeField] private bool canBePickedUp = true;

    public bool CanBePickedUp => canBePickedUp;

    private void Awake()
    {
        if (itemPickup == null) Debug.LogError($"There is no item pickup on {name}");
    }

    private void Start()
    {
        SetCanBePickedUp(CanBePickedUp);
    }

    public void SetCanBePickedUp(bool _canBePickedUp)
    {
        canBePickedUp = _canBePickedUp;

        itemPickup.enabled = canBePickedUp;
    }
}
