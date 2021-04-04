public class SocialInput
{
    public string Actor { get; set; }
    public Thought Thought { get; set; }
    public int Stage { get; set; }

    public SocialInput(string actor, Thought thought, int stage)
    {
        Actor = actor;
        Thought = thought;
        Stage = stage;
    }
}