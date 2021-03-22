public class ContextInput
{
    
    public string Actor { get; set; }
    public bool Acting { get; set; }

    public ContextInput(string actor, bool acting)
    {
        Actor = actor;
        Acting = acting;
    }
}