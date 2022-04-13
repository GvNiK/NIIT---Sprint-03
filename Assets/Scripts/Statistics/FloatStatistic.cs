using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatStatistic : Statistic<float>
{
    public FloatStatistic(string name, float initialValue)
    {
        this.name = name;
        this.value = initialValue;
    }

    public FloatStatistic(string name, float initialValue, float total)
    {
        this.name = name;
        this.value = initialValue;
        this.total = total;
    }
    public override void AddToValue(float additionalValue)
    {
        value += additionalValue;
    }
}
