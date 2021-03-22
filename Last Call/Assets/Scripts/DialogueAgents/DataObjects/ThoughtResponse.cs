using System.Collections.Generic;

public class ThoughtResponse
{
    public HashSet<Thought> Thoughts { get; set; }

    public ThoughtResponse(HashSet<Thought> thoughts)
    {
        Thoughts = thoughts;
    }
}