using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDistanceText : MonoBehaviour
{
    private MaxMovementHandler _movement;
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _movement = FindObjectOfType<MaxMovementHandler>();
    }

    private void Update()
    {
        float dist = (float)Mathf.RoundToInt(_movement.Distance * 100) / 10.0f;
        _text.text = string.Format("{0}m / {1}m", dist, _movement.MaxDistance * 10);
    }
}
