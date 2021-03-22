public class Emotion
{
    public enum Type
    {
        Social,
        Will,
        Love
    }

    public Type MyType { get; set; }
    public float Strength { get; set; }

    public Emotion(Type myType, float strength)
    {
        MyType = myType;
        Strength = strength;
    }
}