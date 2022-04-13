using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Statistic<T>
{
    protected string name;
    protected T value;
    protected T total;
    public abstract void AddToValue(T additionalValue);

    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }

    public T Value
    {
        get
        {
            return value;
        }
        set
        {
            this.value = value;
        }
    }

    public T Total
    {
        get
        {
            return total;
        }
        set
        {
            total = value;
        }
    }
}
