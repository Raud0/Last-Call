using UnityEngine;

public abstract class OutputStackModule : MonoBehaviour
{
    public OutputStack myOutput { get; set; }

    public void Initialize(OutputStack outputStack)
    {
        myOutput = outputStack;
    }
}