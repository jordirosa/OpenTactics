public abstract class ValueModifier : Modifier
{
    public ValueModifier(int sortOrder) : base(sortOrder) { }

    public abstract float modify(float value);
}