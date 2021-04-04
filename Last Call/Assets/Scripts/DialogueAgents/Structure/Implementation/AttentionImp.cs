using System;
using System.Collections.Generic;
using UnityEngine;

public class AttentionImp : Attention
{
    private Dictionary<string, Topic> topicsByName = new Dictionary<string, Topic>();
    private List<FocusedThought> thoughts = new List<FocusedThought>();

    private void Awake()
    {
        Events.OnUpdateTime += Decay;
    }

    private void OnDestroy()
    {
        Events.OnUpdateTime -= Decay;
    }
    
    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) DebugText();
    }

    public void DebugText()
    {
        string text = myInput.myStack.Me + ":";
        foreach (KeyValuePair<string, Topic> pair in topicsByName)
        {
            String topicName = pair.Key;
            Topic topic = pair.Value;
            text += "\n" + topic.MyStage + "("+ topic.Focus + "): " + topic.TopicName;
            foreach (FocusedThought focusedThought in thoughts)
            {
                if (focusedThought.Topic.Equals(topicName))
                {
                    text += "\n\t(" + focusedThought.Focus() + ") " + focusedThought.Text;
                }
            }
        }
        Debug.Log(text);
    }

    public override void Load(HashSet<Topic> topics)
    {
        topicsByName = new Dictionary<string, Topic>();
        foreach (Topic topic in topics)
        {
            LoadTopic(topic);
        }
    }

    private void LoadTopic(Topic topic)
    {
        string topicName = topic.TopicName;
        topicsByName[topicName] = topic;
    }
    
    private void FocusThoughts()
    {
        List<FocusedThought> focusedThoughts = new List<FocusedThought>();

        foreach (FocusedThought focusedThought in thoughts)
        {
            Appraise(focusedThought);
            Topic topic = topicsByName[focusedThought.Topic];
            Topic.Stage stage = focusedThought.Stage;

            bool add = false;
            
            // TODO: Check for climaxes and ignore other topics?
            
            switch (topic.MyStage)
            {
                case Topic.Stage.Orientation:
                    add = stage.Equals(Topic.Stage.Orientation) ||
                          stage.Equals(Topic.Stage.Complication);
                    break;
                case Topic.Stage.Complication:
                    add = stage.Equals(Topic.Stage.Orientation) ||
                          stage.Equals(Topic.Stage.Complication) ||
                          stage.Equals(Topic.Stage.Climax) ||
                          stage.Equals(Topic.Stage.Denouement);
                    break;
                case Topic.Stage.Climax:
                    add = stage.Equals(Topic.Stage.Complication) ||
                          stage.Equals(Topic.Stage.Climax) ||
                          stage.Equals(Topic.Stage.Denouement);
                    break;
                case Topic.Stage.Denouement:
                    add = stage.Equals(Topic.Stage.Complication) ||
                          stage.Equals(Topic.Stage.Denouement) ||
                          stage.Equals(Topic.Stage.Coda);
                    break;
                case Topic.Stage.Coda:
                    add = stage.Equals(Topic.Stage.Coda);
                    break;
            }
            
            if (add) focusedThoughts.Add(focusedThought);
        }
        
        if (true) ClimaxFilter(focusedThoughts);
        if (true) FocusFilter(focusedThoughts, 10f);

        focusedThoughts = focusedThoughts.GetRange(0, Math.Min(focusedThoughts.Count,10));
        List<RankedThought> rankedThoughts = new List<RankedThought>();
        foreach (FocusedThought focusedThought in focusedThoughts)
        {
            RankedThought rankedThought = new RankedThought(focusedThought);
            rankedThoughts.Add(rankedThought);
        }
        
        Send(rankedThoughts);
    }

    private void ClimaxFilter(List<FocusedThought> focusedThoughts)
    {
        Queue<FocusedThought> delete = new Queue<FocusedThought>();
        bool climaxFound = false;
        foreach (FocusedThought focusedThought in focusedThoughts)
        {
            if (focusedThought.Stage.Equals(Topic.Stage.Climax))
            {
                climaxFound = true;
                break;
            }
        }
        if (!climaxFound) return;

        foreach (FocusedThought focusedThought in focusedThoughts)
        {
            if (focusedThought.Stage.Equals(Topic.Stage.Climax) || focusedThought.Stage.Equals(Topic.Stage.Complication)) continue;
            delete.Enqueue(focusedThought);
        }
        
        while (delete.Count > 0) focusedThoughts.Remove(delete.Dequeue());
        
    }
    private void FocusFilter(List<FocusedThought> focusedThoughts, float cap)
    {
        Queue<FocusedThought> delete = new Queue<FocusedThought>();
        
        foreach (FocusedThought focusedThought in focusedThoughts)
        {
            if (focusedThought.Focus() < cap) continue;
            delete.Enqueue(focusedThought);
        }
        
        while (delete.Count > 0) focusedThoughts.Remove(delete.Dequeue());
    }

    public void Remove(Thought thought)
    {
        Queue<FocusedThought> delete = new Queue<FocusedThought>();
        foreach (FocusedThought focusedThought in thoughts)
        {
            if (focusedThought.Text.Equals(thought.Text)) delete.Enqueue(focusedThought);
        }

        while (delete.Count > 0) thoughts.Remove(delete.Dequeue());
    }
    
    public override void Receive(ThoughtFocus thoughtFocus)
    {
        Thought original = thoughtFocus.MainThought;
        if (thoughtFocus.RemoveThought && original != null) { Remove(original); }

        Topic topic = null;
        if (thoughtFocus.Topic != null) { topic = topicsByName[thoughtFocus.Topic]; }

        if (topic != null && topic.MyStage.Equals(Topic.Stage.Hidden)) ExitTo(topic, Topic.Stage.Orientation);
            
        foreach (String tangent in thoughtFocus.Tangents)
        {
            //Debug.Log("\"" + tangent + "\"");
            Topic tangentialTopic = topicsByName[tangent];
            ExitTo(tangentialTopic, Topic.Stage.Orientation);
        }
        
        TopicHeard(topic, thoughtFocus);
    }

    public override void Receive(ThoughtResponse thoughtResponse)
    {
        foreach (Thought thought in thoughtResponse.Thoughts)
        {
            thoughts.Add(FocusThought(thought));
        }
        FocusThoughts();
    }

    private void Decay(int seconds, int minutes)
    {
        ShiftAllTopics(0f, -0.15f * Time.deltaTime, 1f); // decays by 9 points per minute, intuition tells me to switch to percentage, since decay is usually exponential
        FocusThoughts();
    }
    
    private void TopicHeard(Topic topic, ThoughtFocus thoughtFocus)
    {
        if (topic != null)
        {
            switch (topic.MyStage)
            {
                case Topic.Stage.Orientation:
                    Orientation(topic, thoughtFocus);
                    break;
                case Topic.Stage.Complication:
                    Complication(topic, thoughtFocus);
                    break;
                case Topic.Stage.Climax:
                    Climax(topic, thoughtFocus);
                    break;
                case Topic.Stage.Denouement:
                    Denouement(topic, thoughtFocus);
                    break;
                case Topic.Stage.Coda:
                    Coda(topic, thoughtFocus);
                    break;
            }
            
            ShiftAllTopics(-0.02f, 0f, thoughtFocus.FinalMultiplier);
        }
        
        FocusThoughts();
    }

    private void ShiftAllTopics(float percentage, float flat, float finalMultiplier)
    {
        foreach (Topic topic in topicsByName.Values)
        {  ShiftFocusChange(topic, topic.Focus * (1f + percentage) + flat, true, finalMultiplier); }
    }

    private void ShiftFocusChange(Topic topic, float desiredFocus, bool canIncrease, float finalMultiplier)
    {
        float currentFocus = topic.Focus;
        float distance = desiredFocus - currentFocus;
        
        float low = float.NegativeInfinity;
        float high = float.PositiveInfinity;
        if (!canIncrease) high = currentFocus;

        float newFocus = Mathf.Clamp(currentFocus + distance, low, high);
        float actualChange = (newFocus - currentFocus) * finalMultiplier;
        
        topic.FocusToRoots(actualChange, 0.7f, 3);
    }

    private void ShiftFocusChangeDiminishing(Topic topic, float desiredFocus, bool canIncrease, float a, float finalMultiplier)
    {
        float currentFocus = topic.Focus;
        float distance = desiredFocus - currentFocus;
        
        float multiplier = 1f;

        if (distance < -a || distance > a)
        {
            multiplier = Math.Abs(a / distance);
        }

        float low = float.NegativeInfinity;
        float high = float.PositiveInfinity;
        if (!canIncrease) high = currentFocus;

        float newFocus = Mathf.Clamp(currentFocus + distance * multiplier, low, high);
        float actualChange = (newFocus - currentFocus) * finalMultiplier;
        
        topic.FocusToRoots(actualChange, 0.7f, 3);
    }
    
    private void Orientation(Topic topic, ThoughtFocus thoughtFocus)
    {
        if (thoughtFocus.MyStage.Equals(Topic.Stage.Complication)) ExitTo(topic, Topic.Stage.Complication);
        ShiftFocusChangeDiminishing(topic, thoughtFocus.Complexity, true, 5f, thoughtFocus.FinalMultiplier);
    }

    private void Complication(Topic topic, ThoughtFocus thoughtFocus)
    {
        if (thoughtFocus.MyStage.Equals(Topic.Stage.Climax)) ExitTo(topic, Topic.Stage.Climax);
        if (thoughtFocus.MyStage.Equals(Topic.Stage.Denouement)) ExitTo(topic, Topic.Stage.Denouement);
        ShiftFocusChangeDiminishing(topic, thoughtFocus.Complexity, true, 3f, thoughtFocus.FinalMultiplier);
    }

    private void Climax(Topic topic, ThoughtFocus thoughtFocus)
    {
        float focusBefore = topic.Focus;
        ShiftFocusChangeDiminishing(topic, thoughtFocus.Complexity, true, 1f, thoughtFocus.FinalMultiplier);
        float focusAfter = topic.Focus;
        
        if (focusAfter < focusBefore) ExitTo(topic, Topic.Stage.Denouement);
    }

    private void Denouement(Topic topic, ThoughtFocus thoughtFocus)
    {
        if (thoughtFocus.MyStage.Equals(Topic.Stage.Coda)) ExitTo(topic, Topic.Stage.Coda);
        ShiftFocusChangeDiminishing(topic, thoughtFocus.Complexity, false, 3f, thoughtFocus.FinalMultiplier);
    }

    private void Coda(Topic topic, ThoughtFocus thoughtFocus)
    {
        if (thoughtFocus.MyStage.Equals(Topic.Stage.Coda)) ExitTo(topic, Topic.Stage.Ended);
        ShiftFocusChangeDiminishing(topic, thoughtFocus.Complexity, true, 5f, thoughtFocus.FinalMultiplier);
    }

    private void ExitTo(Topic topic, Topic.Stage stage)
    {
        topic.MyStage = stage;

        Queue<ThoughtRequest> requests = new Queue<ThoughtRequest>();
        
        switch (stage)
        {
            case Topic.Stage.Orientation:
                requests.Enqueue(new ThoughtRequest(topic.TopicName, Topic.Stage.Orientation));
                requests.Enqueue(new ThoughtRequest(topic.TopicName, Topic.Stage.Complication));
                break;
            case Topic.Stage.Complication:
                requests.Enqueue(new ThoughtRequest(topic.TopicName, Topic.Stage.Orientation));
                requests.Enqueue(new ThoughtRequest(topic.TopicName, Topic.Stage.Complication));
                requests.Enqueue(new ThoughtRequest(topic.TopicName, Topic.Stage.Climax));
                requests.Enqueue(new ThoughtRequest(topic.TopicName, Topic.Stage.Denouement));
                break;
            case Topic.Stage.Climax:
                requests.Enqueue(new ThoughtRequest(topic.TopicName, Topic.Stage.Complication));
                requests.Enqueue(new ThoughtRequest(topic.TopicName, Topic.Stage.Climax));
                requests.Enqueue(new ThoughtRequest(topic.TopicName, Topic.Stage.Denouement));
                RemoveThoughts(topic, Topic.Stage.Orientation);
                break;
            case Topic.Stage.Denouement:
                requests.Enqueue(new ThoughtRequest(topic.TopicName, Topic.Stage.Complication));
                requests.Enqueue(new ThoughtRequest(topic.TopicName, Topic.Stage.Denouement));
                requests.Enqueue(new ThoughtRequest(topic.TopicName, Topic.Stage.Coda));
                RemoveThoughts(topic, Topic.Stage.Orientation);
                RemoveThoughts(topic, Topic.Stage.Climax);
                break;
            case Topic.Stage.Coda:
                requests.Enqueue(new ThoughtRequest(topic.TopicName, Topic.Stage.Coda));
                RemoveThoughts(topic, Topic.Stage.Complication);
                RemoveThoughts(topic, Topic.Stage.Denouement);
                break;
            case Topic.Stage.Ended:
                RemoveThoughts(topic, Topic.Stage.Coda);
                break;
        }

        while (requests.Count > 0) Send(requests.Dequeue());
    }

    private void RemoveThoughts(Topic topic, Topic.Stage stage)
    {
        Queue<FocusedThought> delete = new Queue<FocusedThought>();
        foreach (FocusedThought thought in thoughts)
        { if (topicsByName[thought.Topic].Equals(topic) && thought.Stage.Equals(stage)) delete.Enqueue(thought); }

        while (delete.Count > 0) { thoughts.Remove(delete.Dequeue()); }
    }

    private FocusedThought FocusThought(Thought thought)
    {
        FocusedThought focusedThought = new FocusedThought(thought, 0f, 0f);
        Appraise(focusedThought);
        return focusedThought;
    }

    private void Appraise(FocusedThought focusedThought)
    {
        focusedThought.Distance = Math.Abs(focusedThought.Complexity - topicsByName[focusedThought.Topic].Focus);
        focusedThought.Share = AttentionShare(topicsByName[focusedThought.Topic]);
    }

    private float AttentionShare(Topic soughtTopic)
    {
        List<Topic> allTopics = new List<Topic>(topicsByName.Values);

        float total = 0f;
        float sought = 0f;

        foreach (Topic topic in allTopics)
        {
            float topicFocus = topic.RootFocus(0.7f, 3);
            total += topicFocus;
            if (topic.Equals(soughtTopic)) { sought += topicFocus; }
        }

        return sought / total;
    }
}