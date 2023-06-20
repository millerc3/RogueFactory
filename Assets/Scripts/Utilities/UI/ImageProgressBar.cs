using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageProgressBar : MonoBehaviour
{
    private Image progressBarFill;

    private void Awake()
    {
        progressBarFill = GetComponent<Image>();
    }

    public void SetProgress(float progress)
    {
        progressBarFill.fillAmount = Mathf.Clamp(progress, 0f, 1f);
    }
}
