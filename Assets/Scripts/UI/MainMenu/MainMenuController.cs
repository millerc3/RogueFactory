using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void LoadFactory()
    {
        SceneLoader.LoadScene(SceneLoader.Scene.FactoryTest);
    }

    public void LoadWilds()
    {
        SceneLoader.LoadScene(SceneLoader.Scene.TPSTesting);
    }
}
