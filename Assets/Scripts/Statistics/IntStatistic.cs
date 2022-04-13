using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntStatistic : Statistic<int>
{
    public IntStatistic(string name, int initialValue)
    {
        this.name = name;
        this.value = initialValue;
    }

    public IntStatistic(string name, int initialValue, int total)
    {
        this.name = name;
        this.value = initialValue;
        this.total = total;
    }

    public override void AddToValue(int additionalValue)
    {
        value += additionalValue;
    }
}
