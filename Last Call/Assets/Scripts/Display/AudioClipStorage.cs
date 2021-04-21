using System.Collections.Generic;
using UnityEngine;

public class AudioClipStorage : MonoBehaviour
{
    public List<AudioClip> clipsInput;
    private Dictionary<string, AudioClip> clips;
    
    private void Awake()
    {
        clips = new Dictionary<string, AudioClip>();
        foreach (AudioClip audioClip in clipsInput)
        {
            string clipName = audioClip.name;
            clips[clipName] = audioClip;
            Debug.Log("Loaded audio clip " + clipName + ".");
        }
    }

    public AudioClip GetClip(string clipName) => !clips.ContainsKey(clipName) ? null : clips[clipName];
}