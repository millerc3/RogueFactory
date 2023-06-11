using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicIcon : MonoBehaviour
{
    [SerializeField] protected Image image;
    public Button button;

    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }
}
