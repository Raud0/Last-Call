public class ThoughtRequest
{
    public string Topic { get; set; }
    public Topic.Stage MyStage { get; set; }

    public ThoughtRequest(string topic, Topic.Stage stage)
    {
        Topic = topic;
        MyStage = stage;
    }
}