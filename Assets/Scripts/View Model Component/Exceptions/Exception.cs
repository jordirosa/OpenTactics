public class Exception
{
    public bool toggle { get; private set; }
    public readonly bool defaultToggle;

    public Exception(bool defaultToggle)
    {
        this.defaultToggle = defaultToggle;

        toggle = defaultToggle;
    }

    public void FlipToggle()
    {
        toggle = !defaultToggle;
    }
}