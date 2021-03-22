using System.Collections.Generic;

public class ThoughtsImp : Thoughts
{
    private Dictionary<string, Dictionary<Topic.Stage, HashSet<Thought>>> thoughts;
    
    public override void Receive(ThoughtRequest thoughtRequest)
    {
        Topic.Stage requestStage = thoughtRequest.MyStage;
        string topic = thoughtRequest.Topic;
        
        ThoughtResponse thoughtResponse = new ThoughtResponse(thoughts[topic][requestStage]);
        
        Send(thoughtResponse);
    }

    public override void Load(HashSet<Thought> newThoughts)
    {
        thoughts = new Dictionary<string, Dictionary<Topic.Stage, HashSet<Thought>>>();
        
        foreach (Thought thought in newThoughts)
        {
            LoadThought(thought);
        }
    }

    private void LoadThought(Thought thought)
    {
        string topic = thought.Topic;
        if (!thoughts.ContainsKey(topic))
        {
            Dictionary<Topic.Stage, HashSet<Thought>> stages = new Dictionary<Topic.Stage, HashSet<Thought>>();
            stages[Topic.Stage.None] = new HashSet<Thought>();
            stages[Topic.Stage.Hidden] = new HashSet<Thought>();
            stages[Topic.Stage.Orientation] = new HashSet<Thought>();
            stages[Topic.Stage.Complication] = new HashSet<Thought>();
            stages[Topic.Stage.Climax] = new HashSet<Thought>();
            stages[Topic.Stage.Denouement] = new HashSet<Thought>();
            stages[Topic.Stage.Coda] = new HashSet<Thought>();
            stages[Topic.Stage.Ended] = new HashSet<Thought>();
            thoughts[topic] = stages;
        }

        Topic.Stage stage = thought.Stage;
        thoughts[topic][stage].Add(thought);
    }
}