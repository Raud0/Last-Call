﻿using System;

 public static class Events
{
    public static event Action<int,int> OnUpdateTime;
    public static void UpdateTime(int minutes,int seconds) => OnUpdateTime?.Invoke(minutes,seconds);
}
