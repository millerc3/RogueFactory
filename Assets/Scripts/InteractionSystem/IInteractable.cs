using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IInteractable
{

    public void TapInteract(GameObject interactor, out bool interactSuccessful);

    public void HoldInteract(GameObject interactor, out bool interactSuccessful);

    public void StopHoldInteract(GameObject interactor);

    public void StartHover(GameObject interactor);

    public void EndHover(GameObject interactor);
}
