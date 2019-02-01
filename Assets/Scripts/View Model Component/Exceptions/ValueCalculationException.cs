using System.Collections.Generic;

public class ValueCalculationException : Exception
{
    public float baseValue;

    List<ValueModifier> modifiers;

    public ValueCalculationException(float baseValue) : base(true)
    {
        this.baseValue = baseValue;
    }

    public void addModifier(ValueModifier m)
    {
        if (modifiers == null)
        {
            modifiers = new List<ValueModifier>();
        }
        modifiers.Add(m);
    }

    public float getModifiedValue()
    {
        if (modifiers == null)
        {
            return baseValue;
        }

        float value = baseValue;
        modifiers.Sort(compare);
        for (int i = 0; i < modifiers.Count; ++i)
        {
            value = modifiers[i].modify(value);
        }

        return value;
    }

    int compare(ValueModifier x, ValueModifier y)
    {
        return x.sortOrder.CompareTo(y.sortOrder);
    }
}
