using System;
using System.Collections.Generic;
using UnityEngine;

public static class Loader
{
    private static Dictionary<string, Topic.Stage> stageMap = new Dictionary<string, Topic.Stage>()
    {
        {"none", Topic.Stage.None},
        {"hidden", Topic.Stage.Hidden},
        {"orientation", Topic.Stage.Orientation},
        {"complication", Topic.Stage.Complication},
        {"climax", Topic.Stage.Climax},
        {"denouement", Topic.Stage.Denouement},
        {"coda", Topic.Stage.Coda},
        {"ended", Topic.Stage.Ended},
    };

    private static Dictionary<string, Thought.Interrupt> interruptMap = new Dictionary<string, Thought.Interrupt>()
    {
        {"want", Thought.Interrupt.Want},
        {"none", Thought.Interrupt.None}
    };

    private static Dictionary<string, Thought.Turn> turnMap = new Dictionary<string, Thought.Turn>()
    {
        {"give", Thought.Turn.Give},
        {"none", Thought.Turn.None},
        {"keep", Thought.Turn.Keep}
    };
    
    private static Dictionary<string, Thought.Affinity> affinityMap = new Dictionary<string, Thought.Affinity>()
    {
        {"love", Thought.Affinity.Love},
        {"none", Thought.Affinity.None},
        {"hate", Thought.Affinity.Hate}
    };

private static Dictionary<int, Emotion.Type> emotionIndexMap = new Dictionary<int, Emotion.Type>()
    {
        {10, Emotion.Type.Anger},
        {11, Emotion.Type.Fear},
        {12, Emotion.Type.Ego},
        {13, Emotion.Type.Respect},
    };
    
    private static Dictionary<int, Argument.Type> argumentIndexMap = new Dictionary<int, Argument.Type>()
    {
        {14, Argument.Type.Humanism},
        {15, Argument.Type.Idealism},
        {16, Argument.Type.Pacifism},
        {17, Argument.Type.Altruism},
        {18, Argument.Type.Fatalism},
        {19, Argument.Type.Futurism},
        {20, Argument.Type.Tribalism},
        {21, Argument.Type.Authoritarianism},
        {22, Argument.Type.Militarism},
        {23, Argument.Type.Utilitarianism},
        {24, Argument.Type.Superiority}
    };

    public static Dictionary<string,HashSet<Topic>> LoadTopics(TextAsset textAsset, HashSet<ActorStack> actors)
    {
        Dictionary<string,HashSet<Topic>> actorTopics = new Dictionary<string, HashSet<Topic>>();

        foreach (ActorStack actorStack in actors)
        {
            char[] nextLineChars = {'\n'};
            char[] nextValueChars = {'\t'};
        
            string text = textAsset.text;
            string[] lines = text.Split(nextLineChars);

            Dictionary<string, Topic> identities = new Dictionary<string, Topic>();
            Dictionary<string, HashSet<Topic>> branches = new Dictionary<string, HashSet<Topic>>();
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] values = line.Split(nextValueChars, 2);
            
                try
                {
                    string topicName = values[0].Trim();
                    if (topicName.Equals("")) continue;

                    Topic topic = new Topic(topicName, null);
                    identities[topicName] = topic;
                
                    if (values.Length < 2) continue;
                
                    string parentName = values[1].Trim();
                    if (parentName.Equals("")) continue;
                    if (!branches.ContainsKey(parentName)) branches[parentName] = new HashSet<Topic>();
                    branches[parentName].Add(topic);
                }
                catch (Exception e)
                {
                    Debug.Log("Skipped reading: \"" + line + "\". (" + e.Message + ")");
                }
            
            }

            foreach (KeyValuePair<string,HashSet<Topic>> pair in branches)
            {
                string parentName = pair.Key;
                HashSet<Topic> children = pair.Value;

                try
                {
                    Topic parent = identities[parentName];
                    foreach (Topic child in children) child.Parent = parent;
                }
                catch (Exception e)
                {
                    Debug.Log("Didn't find parent topic: \"" + parentName + "\"");
                }
                
            }

            HashSet<Topic> newTopics = new HashSet<Topic>();
            foreach (Topic topic in identities.Values)
            {
                //Debug.Log("Loaded topic: " + topic.TopicName);
                newTopics.Add(topic);
            }

            actorTopics[actorStack.Me] = newTopics;
        }
        
        return actorTopics;
    }

    public static Dictionary<string,HashSet<Thought>> LoadThoughts(TextAsset textAsset)
    {
        char[] nextLineChars = {'\n'};
        char[] nextValueChars = {'\t'};
        char[] nextSubValueChars = {';'};
        
        string text = textAsset.text;
        string[] lines = text.Split(nextLineChars);
        
        Dictionary<string, HashSet<Thought>> newThoughts = new Dictionary<string, HashSet<Thought>>();
        string topic = null;
        Topic.Stage stage = Topic.Stage.None;
        string actor = null;

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] values = line.Split(nextValueChars, 25);
            
            try
            {
                // Update running values: Topic, Stage, Actor
                if (!values[0].Equals("")) topic = values[0].Trim();
                string stageText = values[1].Trim().ToLower();
                if (stageMap.ContainsKey(stageText)) stage = stageMap[stageText];
                string actorText = values[2].Trim();
                if (!actorText.Equals(""))
                {
                    actor = actorText;
                    if (!newThoughts.ContainsKey(actor)) newThoughts[actor] = new HashSet<Thought>();
                }

                // Read Complexity
                float complexity = 0f;
                if (!float.TryParse(values[3].Trim(), out complexity))
                {
                    Debug.Log("Failed to read: \"" + line + "\".");
                    continue;
                }

                // Read Text
                string thoughtText = values[4].Trim();
                if (values[4].Equals("")) continue;
                
                // bruh moment (I get it, but still)
                if (thoughtText.Contains(",") || thoughtText.Contains("\"")
                    && thoughtText[0] == '\"'
                    && thoughtText[thoughtText.Length - 1] == '\"')
                {
                    thoughtText = thoughtText.Substring(1, thoughtText.Length - 2);
                }
                
                while (thoughtText.Contains("\"\""))
                {
                    thoughtText = thoughtText.Replace("\"\"", "\"");
                }
                
                // Read Social Expectations
                Thought.Interrupt interruptStrategy = Thought.Interrupt.None;
                string interruptText = values[5].Trim().ToLower();
                if (interruptMap.ContainsKey(interruptText)) interruptStrategy = interruptMap[interruptText];

                Thought.Turn turnStrategy = Thought.Turn.None;
                string turnText = values[6].Trim().ToLower();
                if (turnMap.ContainsKey(turnText)) turnStrategy = turnMap[turnText];

                Thought.Affinity affinity = Thought.Affinity.None;
                string affinityText = values[7].Trim().ToLower();
                if (affinityMap.ContainsKey(affinityText)) affinity = affinityMap[affinityText];

                // Read Tangental Topics
                string[] tangentStrings = values[8].Trim().Split(nextSubValueChars);
                HashSet<string> tangents = new HashSet<string>();
                foreach (var tangent in tangentStrings)
                {
                    if (!tangent.Equals("")) tangents.Add(tangent.Trim());
                }

                // Read Special Event Codes 
                int eventCode = -1;
                int.TryParse(values[9].Trim(), out eventCode);

                // Read Emotional Weight Values
                HashSet<Emotion> emotions = new HashSet<Emotion>();
                foreach (KeyValuePair<int, Emotion.Type> pair in emotionIndexMap)
                {
                    int index = pair.Key;
                    if (index > values.Length - 1) continue;

                    Emotion.Type type = pair.Value;

                    float strength = 0f;
                    if (float.TryParse(values[index].Trim(), out strength))
                    {
                        Emotion emotion = new Emotion(type, strength);
                        emotions.Add(emotion);
                    }
                } 

                // Read Emotional Attack Values
                HashSet<Argument> arguments = new HashSet<Argument>();
                foreach (KeyValuePair<int, Argument.Type> pair in argumentIndexMap)
                {
                    int index = pair.Key;
                    if (index > values.Length - 1) continue;
                    
                    Argument.Type type = pair.Value;

                    float strength = 0f;
                    if (float.TryParse(values[index].Trim(), out strength))
                    {
                        Argument argument = new Argument(type, strength);
                        arguments.Add(argument);
                    }
                }
                
                Thought thought = new Thought(topic, stage, actor, complexity, thoughtText, interruptStrategy, turnStrategy, affinity, tangents, eventCode, arguments, emotions);
                newThoughts[actor].Add(thought);
                
            } catch (Exception e)
            {
                Debug.Log("Skipped reading: \"" + line + "\" with value count " + values.Length + ". (" + e.Message + ")");
            }
        }

        foreach (KeyValuePair<string, HashSet<Thought>> pair in newThoughts)
        {
            string actorName = pair.Key;
            int length = pair.Value.Count;

            Debug.Log("Read in " + length + " thoughts for " + actorName + ".");
        }

        return newThoughts;
    }
}