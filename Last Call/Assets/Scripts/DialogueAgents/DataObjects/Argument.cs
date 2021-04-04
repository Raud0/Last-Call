public class Argument
{
    public enum Type
    {
        Idealism,
        Pacifism,
        Altruism,
        Fatalism,
        Futurism,
        Tribalism,
        Authoritarianism,
        Militarism,
        Utilitarianism,
        Superiority
    }
    
    public Type MyType { get; set; }
    public float Strength { get; set; }

    public Argument(Type myType, float strength)
    {
        MyType = myType;
        Strength = strength;
    }
}