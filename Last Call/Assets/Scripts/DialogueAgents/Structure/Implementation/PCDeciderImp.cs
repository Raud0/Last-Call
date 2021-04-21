using System.Collections.Generic;

public class PCDeciderImp : DeciderImp
{
    private bool queue = false;
    List<RankedThought> queuedThoughts;
    
    public override void ImpReceive(Emotion emotion)
    {
        return;
    }
    
    public override void ImpReceive(SocialInput socialInput)
    {
        return;
    }

    public override void ImpReceive(ActingInput actingInput)
    {
        return;
    }

    public override void ImpReceive(List<RankedThought> thoughts)
    {
        queuedThoughts = new List<RankedThought>(thoughts);
        queue = true;
    }

    private void SendUpdate()
    {
        queue = false;
        myOutput.myStack.conversation.optionDisplayer.DeciderImp = this;
        myOutput.myStack.conversation.optionDisplayer.QueueUpdateThoughts(new List<Thought>(queuedThoughts));
    }
    
    private void FixedUpdate()
    {
        if (queue) SendUpdate();
        
        DoSpeaking();
    }

    private void DoSpeaking()
    {
        ProgressThought();
    }

    public void ReleaseThought(Thought thought)
    {
        if (currentThought != null && currentThought.Equals(thought)) currentThought = null;
        currentThoughtProgress = 0f;
    }

    protected override void FinishThought()
    {
        myOutput.myStack.conversation.optionDisplayer.FinishThought(currentThought);
        ReleaseThought(currentThought);
    }
}