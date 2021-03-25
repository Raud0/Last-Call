public class Topic
{
    public enum Stage
    {
        None,
        Hidden,
        Orientation,
        Complication,
        Climax,
        Denouement,
        Coda,
        Ended
    }

    public string TopicName { get; set; }
    public Topic Parent { get; set; }
    public Stage MyStage { get; set; }
    public float Focus { get; set; }

    public float RootFocus(float multiplier, int maxDepth)
    {
        if (maxDepth <= -1)
        {
            return 0f;
        }

        if (this.Parent == null)
        {
            return this.Focus;
        }

        return this.Focus + Parent.RootFocus(multiplier, maxDepth - 1) * multiplier;
    }

    public void FocusToRoots(float amount, float multiplier, int maxDepth)
    {
        if (maxDepth <= -1)
        {
            return;
        }

        this.Focus += amount;

        if (this.Parent == null)
        {
            return;
        }

        this.Parent.FocusToRoots(amount * multiplier, multiplier, maxDepth - 1);
    }

    public Topic(string name, Topic parent, Stage stage = Stage.Hidden, float focus = 0f)
    {
        TopicName = name;
        Parent = parent;
        MyStage = stage;
        Focus = focus;
    }
}