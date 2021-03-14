using System;
using System.Collections.Generic;

public class Thought
{
    public string topic;
    public Topic.Stage stage;
    public float complexity;

    public string text;

    public List<string> tangents;
    public List<Affection> affections;
    public List<Emotion> emotions;
}

public class RankedThought : Thought,  IComparable<RankedThought>
{
    public float rank;
    
    public int CompareTo(RankedThought other)
    {
        float dif = rank - other.rank;

        if (dif < -float.Epsilon) return -1;
        if (dif > float.Epsilon) return 1;
        return 0;
    }
}