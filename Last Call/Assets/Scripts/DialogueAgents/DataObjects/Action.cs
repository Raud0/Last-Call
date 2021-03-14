using UnityEditor;

public class Action
{
    public string actor;

    public enum Type
    {
        Speech,
        Interaction
    };
    
    public Type type { get; set; }
}

public class Speech : Action
{
    public float progress;
    public Thought thought;
    
    public Speech(float progress, Thought thought)
    {
        this.type = Type.Speech;
        this.progress = progress;
        this.thought = thought;
    }
}

public class Interaction : Action
{
    
}