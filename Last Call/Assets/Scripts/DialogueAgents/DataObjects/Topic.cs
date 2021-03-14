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
        Depleted
    }
    
    Topic parent;

    public Stage stage;
    public float focus;

    public float RootFocus(float multiplier, int maxDepth)
    {
        if (maxDepth <= -1) { return 0f; }
        if (this.parent == null) { return this.focus; }
        
        return this.focus + parent.RootFocus(multiplier, maxDepth - 1) * multiplier;
    }

    public void FocusToRoots(float amount, float multiplier, int maxDepth)
    {
        if (maxDepth <= -1) { return; }
        this.focus += amount;
        
        if (this.parent == null) { return; }
        this.parent.FocusToRoots(amount * multiplier, multiplier, maxDepth - 1);
    }

    public string topicName;
}