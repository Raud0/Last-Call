using System;

public static class Events
{
    public static event Action<int,int> OnUpdateTime;
    public static void UpdateTime(int minutes,int seconds) => OnUpdateTime?.Invoke(minutes,seconds);

    public static event Action<Option> OnAddOption;
    public static event Action<Option> OnRemoveOption;
    public static event Action<Option> OnChooseOption;
    public static void AddOption(Option option) => OnAddOption?.Invoke(option);
    public static void RemoveOption(Option option) => OnRemoveOption?.Invoke(option);
    public static void ChooseOption(Option option) => OnChooseOption?.Invoke(option);
}
