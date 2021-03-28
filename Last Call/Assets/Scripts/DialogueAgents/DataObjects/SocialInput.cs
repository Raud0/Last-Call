public class SocialInput
{
    public string Actor { get; set; }
    public Thought.Interrupt InterruptExpectation { get; set; }
    public Thought.Turn TurnExpectations { get; set; }

    public SocialInput(string actor, Thought.Interrupt interruptExpectation, Thought.Turn turnExpectations)
    {
        Actor = actor;
        InterruptExpectation = interruptExpectation;
        TurnExpectations = turnExpectations;
    }
}