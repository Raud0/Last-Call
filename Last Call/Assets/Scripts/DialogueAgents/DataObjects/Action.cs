public class Action
{
    public enum Type
    {
        Speech,
        Interaction
    };
    
    public string Actor { get; set; }
    public Type MyType { get; set; }

    public Action(Type type, string actor)
    {
        MyType = type;
        Actor = actor;
    }
}    

public class Speech : Action
{
    public float Progress { get; set; }
    public Thought Thought { get; set; }
    
    public Speech(string actor, float progress, Thought thought) : base(Type.Speech, actor)
    {
        Progress = progress;
        Thought = thought;
    }
}

public class Interaction : Action
{
    public Interaction(string actor) : base(Type.Interaction, actor)
    {
    }
}