using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FadeImageOpacity : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField][Range(0f, 1.0f)] private float startOpacity = 0.0f;
    [SerializeField][Range(0f, 1.0f)] private float endOpacity = 1.0f;
    [Tooltip("Should the image start to transition on awake")]
    [SerializeField] private bool beginFadeOnAwake = true;
    //[Tooltip("How long does it take to fade from the start opacity to the end opacity")]
    //[SerializeField] private float timeToFade = 1.0f;

    public UnityEvent FadeStartEvent;
    private bool isFading = false;
    private float fadeTimer = 0f;
    private float currOpacity;

    public UnityEvent FadeFinishEvent;
    private bool canInvokeFadeFinishEvent = true;

    private void Awake()
    {
        ResetFade();

        if (beginFadeOnAwake)
        {
            StartFade();
        }

        if (FadeStartEvent == null)
        {
            FadeStartEvent = new UnityEvent();
        }

        if (FadeFinishEvent == null)
        {
            FadeFinishEvent = new UnityEvent();
        }
    }

    private void Update()
    {
        if (isFading)
        {
            UpdateImageOpacity();
        }
    }

    private void OnEnable()
    {
        FadeStartEvent?.AddListener(StartFade);
    }

    private void OnDisable()
    {
        FadeStartEvent?.RemoveAllListeners();
        FadeStartEvent = null;
    }


    public void StartFade()
    {
        isFading = true;
    }

    public void ResetFade()
    {
        currOpacity = startOpacity;
        canInvokeFadeFinishEvent = true;
        UpdateImageOpacity();
    }

    private void UpdateImageOpacity()
    {
        currOpacity = Mathf.Lerp(startOpacity, endOpacity, fadeTimer);
        currOpacity = Mathf.Clamp(currOpacity, startOpacity, endOpacity); ;

        Color imageColor = targetImage.color;
        imageColor.a = currOpacity;
        targetImage.color = imageColor;

        if (canInvokeFadeFinishEvent && Mathf.Approximately(currOpacity, endOpacity))
        {
            FadeFinishEvent?.Invoke();
            canInvokeFadeFinishEvent = false;
        }

        fadeTimer += Time.deltaTime;
    }


}
