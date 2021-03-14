﻿using System.Collections.Generic;

public abstract class Decider : OutputStackModule
{
    public abstract void Receive(Emotion emotion);
    public abstract void Receive(ContextInput contextInput);
    public abstract void Receive(List<Thought> thoughts);

    public void Send(Speech speech)
    { ConversationMedium.Speak(speech); }
    public void Send(Interaction interaction)
    { ConversationMedium.Interact(interaction);}
}