using System;
using UnityEngine;
using System.Collections;
using TMPro;

// Class based on TMP Examples
public class ColourEffect : MonoBehaviour
{
    private TMP_Text m_TextComponent;
    private Coroutine coroutine;
    private bool active;

    void Awake()
    {
        m_TextComponent = GetComponent<TMP_Text>();
        active = false;
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

    public void Active(bool active)
    {
        this.active = active;
    }
    
    IEnumerator AnimateVertexColors()
    {
        // Force the text object to update right away so we can have geometry to modify right from the start.
        m_TextComponent.ForceMeshUpdate();

        TMP_TextInfo textInfo = m_TextComponent.textInfo;

        Color32[] newVertexColors;
        Color32 c0 = m_TextComponent.color;

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
                int materialIndex = textInfo.characterInfo[currentCharacter].materialReferenceIndex;
                newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                int vertexIndex = textInfo.characterInfo[currentCharacter].vertexIndex;

                if (!textInfo.characterInfo[currentCharacter].isVisible) continue;
                
                
                int r = 255;
                int g = 255;
                int b = 0;
                if (!active) b = 255;
                int a = 255;
                
                
                
                c0 = new Color32((byte)r, (byte)g, (byte)b, (byte)a);
                

                newVertexColors[vertexIndex + 0] = c0;
                newVertexColors[vertexIndex + 1] = c0;
                newVertexColors[vertexIndex + 2] = c0;
                newVertexColors[vertexIndex + 3] = c0;
                m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                
            }

            yield return new WaitForSeconds(0.005f);
        }
    }
}





