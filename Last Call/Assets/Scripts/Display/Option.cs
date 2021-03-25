using System;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class Option : MonoBehaviour
{
    public ColourEffect colourEffect;
    public ColourWaveEffect colourWaveEffect;
    public RectTransform rectTransform;
    private TextMeshProUGUI ugui;
    public Thought thought;

    private bool selected;

    private void Awake()
    {
        colourEffect = GetComponent<ColourEffect>();
        colourWaveEffect = GetComponent<ColourWaveEffect>();
        rectTransform = GetComponent<RectTransform>();
        ugui = GetComponent<TextMeshProUGUI>();

        Selected(false);
        Hovered(false);
    }

    public void SetThought(Thought thought)
    {
        this.thought = thought;
        ugui.text = thought.Text;
    }

    public void Hovered(bool status)
    {
        if (selected)
        {
            status = false;
        }
        colourEffect.Active(status);
    }
    
    public void Selected(bool status)
    {
        selected = status;
        colourEffect.enabled = !status;
        colourWaveEffect.enabled = status;
        colourWaveEffect.Active(status);
    }

    public void Destroy()
    {
        Destroy(gameObject);
        enabled = false;
    }
}