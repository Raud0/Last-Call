using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeechDisplay : MonoBehaviour
{
    private TextMeshProUGUI ugui;
    private RectTransform rect;
    private WaveAndFade waveAndFade;
    private string text;
    private float progress;
    private float timeSinceCompletion;

    private void Awake()
    {
        ugui = GetComponent<TextMeshProUGUI>();
        rect = GetComponent<RectTransform>();
        waveAndFade = GetComponent<WaveAndFade>();
        text = "";
        progress = 0f;
        timeSinceCompletion = 0f;
    }

    private void Update()
    {
        rect.anchoredPosition += new Vector2(0f, 50f * Time.deltaTime);

        if (progress > 1f)
        {
            if (timeSinceCompletion > 5f)
            {
                Destroy(gameObject);
            }
            else
            {
                timeSinceCompletion += Time.deltaTime;
            }
        } 
    }

    public void SetText(string text)
    {
        this.text = text;
        ugui.text = "";
    }

    public void SetColor(Color color)
    {
        ugui.color = color;
    }

    public void SetAlignment(bool left)
    {
        ugui.alignment = left ? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
    }

    public bool UpdateProgress(float progress)
    {
        this.progress = progress;
        UpdateText();

        return this.progress >= 1f;
    }

    private void UpdateText()
    {
        int newLength = Mathf.Clamp(Mathf.FloorToInt(text.Length * progress), 0, text.Length);
        ugui.text = text.Substring(0, newLength);
        waveAndFade.fadeEnd = newLength;
    }
}
