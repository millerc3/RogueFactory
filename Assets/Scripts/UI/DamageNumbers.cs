using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class DamageNumbers : MonoBehaviour
{
    private TMP_Text numbersText;

    [SerializeField] private float riseSpeed = 1.0f;
    [SerializeField] private float swayAmount = 1.0f;
    private float timeAlive = 0f;

    private void Awake()
    {
        numbersText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;
        transform.position += (Vector3.up * riseSpeed + Vector3.right * Mathf.Sin(Time.time) * swayAmount * timeAlive) * Time.deltaTime;
    }

    public void SetValue(int value)
    {
        numbersText.text = value.ToString();
    }
}
