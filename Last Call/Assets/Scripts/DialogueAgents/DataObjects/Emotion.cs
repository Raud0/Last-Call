public class Emotion
{
    public enum Type
    {
        Anger,
        Fear,
        Ego,
        Respect
    }

    public Type MyType { get; set; }
    public float Strength { get; set; }

    public Emotion(Type myType, float strength)
    {
        MyType = myType;
        Strength = strength;
    }
}