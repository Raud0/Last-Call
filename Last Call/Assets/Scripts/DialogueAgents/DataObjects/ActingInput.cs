public class ActingInput
{
    public string Actor { get; set; }
    public bool Acting { get; set; }

    public ActingInput(string actor, bool acting)
    {
        Actor = actor;
        Acting = acting;
    }
}