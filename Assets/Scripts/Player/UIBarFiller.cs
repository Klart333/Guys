using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBarFiller : MonoBehaviour
{
    [SerializeField]
    private float speed = 2;

    [SerializeField]
    private Color normalColor;

    [SerializeField]
    private Color drainColor;

    private Image image;

    private float targetFillAmount = 1;
    private float currentfillAmount = 1;

    private void Start()
    {
        image = GetComponent<Image>();
        image.color = normalColor;
    }

    public void SetTargetFillAmount(float target)
    {
        targetFillAmount = target;
    }

    private void Update()
    {
        if (currentfillAmount != targetFillAmount)
        {
            float diff = targetFillAmount - currentfillAmount;
            currentfillAmount += diff * speed * Time.deltaTime;

            image.fillAmount = currentfillAmount;
            if (diff < 0 && image.color != drainColor)
            {
                image.color = drainColor;
            }
            else if (diff >= 0 && image.color != normalColor)
            {
                image.color = normalColor;
            }
        }
    }
}
