public class Attack
{
    public enum Type
    {
        Shame,
        Praise,
        Scare,
        Encourage,
        Mistrust,
        Trust,
        Silence
    }
    
    public Type MyType { get; set; }
    public float Strength { get; set; }

    public Attack(Type myType, float strength)
    {
        MyType = myType;
        Strength = strength;
    }
}