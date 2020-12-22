using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    private void Awake()
    {
        Events.OnChooseOption += ChooseOption;
    }

    private void OnDestroy()
    {
        Events.OnChooseOption -= ChooseOption;
    }

    private void Start()
    {
        DefaultOptions();
    }

    private void DefaultOptions()
    {
        List<string> texts = new List<string>
        {
            "Option 1",
            "Option 2",
            "Option 3"
        };

        foreach (string text in texts)
        {
            Option option = new Option();
            option.text = text;
            
            Events.AddOption(option);
        }
    }

    private void ChooseOption(Option option)
    {
        
    }
}
