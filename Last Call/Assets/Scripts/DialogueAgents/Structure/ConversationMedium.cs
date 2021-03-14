using System;

public static class ConversationMedium
{
    public static event Action<Speech> OnSpeech;
    public static void Speak(Speech speech) => OnSpeech?.Invoke(speech);

    public static event Action<Interaction> OnInteraction;
    public static void Interact(Interaction interaction) => OnInteraction?.Invoke(interaction);
}