using System;
using UnityEngine;
using UnityEngine.UI;

public class BarScript : MonoBehaviour
{
    [SerializeField]
    private Image content;

    [SerializeField]
    private Text valueText;

    [SerializeField]
    private float lerpSpeed;

    private float fillAmount;

    [SerializeField]
    private bool lerpColors;

    [SerializeField]
    private Color fullColor;

    [SerializeField]
    private Color lowColor;

    public float MaxValue { get; set; }

    private float currentTime;

    public float Value
    {
        set
        {
            fillAmount = Map(value, 0, MaxValue, 0, 1);
        }
    }

    void Start()
    {
        if (lerpColors) //Sets the standard color
        {
            content.color = fullColor;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        
        HandleBar();

    }

    private void HandleBar()
    {
        if (fillAmount != content.fillAmount) 
        {
            content.fillAmount = Mathf.Lerp(content.fillAmount, fillAmount, Time.deltaTime * lerpSpeed);

            if (lerpColors) 
            {
                content.color = Color.Lerp(lowColor, fullColor, fillAmount);
            }

        }

    }

    public void Reset()
    {
        Value = MaxValue;
        content.fillAmount = 1;
    }

    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
}
