using System;
using UnityEngine;

public abstract class InputStackModule : MonoBehaviour
{
    public InputStack myInput { get; set; }

    public void Initialize(InputStack outputStack)
    {
        myInput = outputStack;
    }
}