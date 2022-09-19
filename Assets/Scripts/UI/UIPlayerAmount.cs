using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UIPlayerAmount : MonoBehaviour
{
    [SerializeField]
    private float _spinSpeed = 2;

    [SerializeField]
    private TextMeshProUGUI _text;

    [SerializeField]
    private Color[] _colors;

    private int _amount;

    private void Start()
    {
        _amount = 2;
        GameManager.Instance.SetPlayerAmount(_amount);
    }

    public void IncreaseAmount()
    {
        CrazySpin(++_amount);
        GameManager.Instance.SetPlayerAmount(_amount);
    }

    public void ResetAmount()
    {
        _amount = 2;
        CrazySpin(_amount);
        GameManager.Instance.SetPlayerAmount(_amount);
    }

    private async void CrazySpin(int targetNum)
    {
        float t = 0;

        Quaternion startRotation = Quaternion.identity;

        bool increased = false;
        while (t <= 1f)
        {
            t += Time.deltaTime * _spinSpeed;

            float te = Mathf.SmoothStep(0.0f, 1.0f, t);

            transform.rotation = startRotation * Quaternion.Euler(0, 0, te * 360);

            if (t > 0.5f && !increased)
            {
                increased = true;

                _text.text = targetNum.ToString();
                if (targetNum < _colors.Length)
                {
                    _text.color = _colors[targetNum];
                }
                else
                {
                    _text.color = _colors[_colors.Length - 1];
                }
            }

            await Task.Yield();
        }

        transform.rotation = startRotation;
    }
}
