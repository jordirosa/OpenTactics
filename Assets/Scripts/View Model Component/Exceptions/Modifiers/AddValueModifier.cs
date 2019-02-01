public class AddValueModifier : ValueModifier
{
    public readonly float toAdd;

    public AddValueModifier(int sortOrder, float toAdd) : base(sortOrder)
    {
        this.toAdd = toAdd;
    }

    public override float modify(float value)
    {
        return value + toAdd;
    }
}