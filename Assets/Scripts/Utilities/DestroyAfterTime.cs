using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float timeUntilDestroy = 3f;
    private float timeLeft;

    [SerializeField] private bool shrinkToDestroy = true;
    [SerializeField] [Range(.001f, .5f)] private float shrinkAmountPerFrame = 0.1f;

    private bool destroying = false;

    private void Start()
    {
        timeLeft = timeUntilDestroy;
    }

    private void Update()
    {
        if (timeLeft <= 0f && !destroying)
        {
            destroying = true;
            if (shrinkToDestroy)
            {
                StartCoroutine(ShrinkAndDestroy());
            }
            else
            {
                Destroy(gameObject);
            }
            
        }

        timeLeft -= Time.deltaTime;
    }

    IEnumerator ShrinkAndDestroy()
    {
        while (transform.localScale.x > .04f)
        {
            transform.localScale *= (1f - shrinkAmountPerFrame);
            yield return null;
        }
        Destroy(gameObject);
    }
}
