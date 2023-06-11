using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactor : MonoBehaviour
{
    protected IInteractable prevInteractable = null;
    protected IInteractable targetInteractable = null;

    protected abstract void Scan();
    protected virtual void TapInteract(GameObject interactor)
    {
        if (targetInteractable != null)
        {
            targetInteractable.TapInteract(interactor, out bool interactSuccessful);
        }
    }

    protected virtual void HoldInteract(GameObject interactor)
    {
        if (targetInteractable != null)
        {
            targetInteractable.HoldInteract(interactor, out bool interactSuccessful);
        }
    }

    protected virtual void StartHover(GameObject interactor)
    {
        if (targetInteractable != null)
        {
            targetInteractable.StartHover(interactor);
        }
    }

    protected virtual void EndHover(GameObject interactor)
    {
        if (targetInteractable != null)
        {
            targetInteractable.EndHover(interactor);
        }
    }

    public static bool InRangeOfInteractor(Transform interactorTransform, Transform interactableTransform, float interactionRange)
    {
        if (interactorTransform == null) return true;

        return Vector3.Distance(interactableTransform.position, interactorTransform.position) <= interactionRange;
    }

    public virtual void StopHoldInteract(GameObject interactor)
    {
        if (targetInteractable != null)
        {
            targetInteractable.StopHoldInteract(interactor);
        }
    }
}
