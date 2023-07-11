using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeathScreenController : MonoBehaviour
{
    [SerializeField] private FadeImageOpacity backgroundFadeComponent;
    [SerializeField] private GameObject deathScreenUI;
    [SerializeField] private CombatPlayerCollectionManager playerCollectionManager;


    private void Start()
    {
        deathScreenUI.SetActive(false);
    }

    private void OnEnable()
    {
        if (backgroundFadeComponent.FadeFinishEvent == null)
        {
            backgroundFadeComponent.FadeFinishEvent = new UnityEvent();
        }
        backgroundFadeComponent.FadeFinishEvent.AddListener(OnFadeCompeted);
    }

    private void OnDisable()
    {
        backgroundFadeComponent.FadeFinishEvent.RemoveListener(OnFadeCompeted);
    }

    private void OnFadeCompeted()
    {
        deathScreenUI.SetActive(true);

        StoreItemsInCollection();
    }

    private void StoreItemsInCollection()
    {
        playerCollectionManager = FindObjectOfType<CombatPlayerCollectionManager>();
        if (playerCollectionManager != null)
        {
            playerCollectionManager.AddToSavedCollection();
        }
    }

    public void LoadMainMenuScene()
    {
        SceneLoader.LoadScene(SceneLoader.Scene.MainMenu);
    }

    public void LoadFactoryScene()
    {
        SceneLoader.LoadScene(SceneLoader.Scene.FactoryTest);
    }
}
