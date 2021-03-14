using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSystem : MonoBehaviour
{
    public string me;
    
    public InputStack inputStack;
    public OutputStack outputStack;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        inputStack.Initialize(this);
        outputStack.Initialize(this);
    }
}
