using System;
using UnityEngine;
using System.Collections;
using TMPro;

// Class based on TMP Examples
public class WaveAndFade : MonoBehaviour
{
    private TMP_Text m_TextComponent;
    private Coroutine coroutine;
    public int fadeEnd;
    public int waveEnd;

    void Awake()
    {
        m_TextComponent = GetComponent<TMP_Text>();
        fadeEnd = 0;
        waveEnd = 0;
    }

    private void OnEnable()
    {
        coroutine = StartCoroutine(AnimateVertexColors());
    }

    private void OnDisable()
    {
        if (coroutine == null) return;
        StopCoroutine(coroutine);
    }
    
    IEnumerator AnimateVertexColors()
    {
        // Force the text object to update right away so we can have geometry to modify right from the start.
        m_TextComponent.ForceMeshUpdate();

        TMP_TextInfo textInfo = m_TextComponent.textInfo;

        Color32[] newVertexColors;

        while (true)
        {
            int characterCount = textInfo.characterCount;

            // If No Characters then just yield and wait for some text to be added
            if (characterCount == 0)
            {
                yield return new WaitForSeconds(0.25f);
                continue;
            }

            for (int currentCharacter = 0; currentCharacter < characterCount; currentCharacter++)
            {
                if (currentCharacter < waveEnd)
                {
                    // Might take some time lol
                }

                if (currentCharacter < fadeEnd)
                {
                    int materialIndex = textInfo.characterInfo[currentCharacter].materialReferenceIndex;
                    newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                    int vertexIndex = textInfo.characterInfo[currentCharacter].vertexIndex;
                    
                    for (int i = 0; i < 4; i++)
                    {
                        newVertexColors[vertexIndex + i].a = (byte) Math.Max(0, newVertexColors[vertexIndex + i].a - 1);
                    }
                    m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32); 
                }
            }
            
            yield return new WaitForSeconds(0.02f);
        }
    }
}





