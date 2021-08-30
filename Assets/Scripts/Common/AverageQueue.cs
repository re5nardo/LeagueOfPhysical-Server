using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AverageQueue
{
    private Queue<float> values = new Queue<float>();
    private bool isDirty = true;
    private int maxCount;

    private float average;
    public float Average
    {
        get
        {
            if (isDirty)
            {
                average = values.Average();
                isDirty = false;
            }

            return average;
        }
    }

    public AverageQueue(int maxCount = 10)
    {
        this.maxCount = maxCount;
    }

    public AverageQueue Add(float value)
    {
        values.Enqueue(value);
        if (values.Count > maxCount)
        {
            values.Dequeue();
        }

        isDirty = true;

        return this;
    }
}
