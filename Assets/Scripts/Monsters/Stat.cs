using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
class Stat
{
    [SerializeField]
    private BarScript bar;

    [SerializeField]
    private float maxVal;

    [SerializeField]
    private float currentVal;

    public float CurrentValue
    {
        get
        {
            return currentVal;
        }
        set
        {
            this.currentVal = Mathf.Clamp(value, 0, MaxVal);

            Bar.Value = currentVal;
        }
    }

    public float MaxVal
    {
        get
        {
            return maxVal;
        }
        set
        {
            Bar.MaxValue = value;

            this.maxVal = value;
        }
    }

    public BarScript Bar { get => bar; }

    public void Initialize()
    {
        this.MaxVal = maxVal;
        this.CurrentValue = currentVal;
    }
}

