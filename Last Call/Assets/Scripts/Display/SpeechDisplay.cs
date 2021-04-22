using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpeechDisplay : MonoBehaviour
{
    private TextMeshProUGUI ugui;
    private RectTransform rect;
    private WaveAndFade waveAndFade;
    private AudioSource audio;
    private ConversationMedium cm;
    private string text;
    private float progress;
    private float timeSinceCompletion;
    private bool amLead;

    private void Awake()
    {
        ugui = GetComponent<TextMeshProUGUI>();
        rect = GetComponent<RectTransform>();
        waveAndFade = GetComponent<WaveAndFade>();
        audio = GetComponent<AudioSource>();
        audio.loop = false;
        text = null;
        progress = 0f;
        timeSinceCompletion = 0f;
    }

    private void FixedUpdate()
    {
        rect.anchoredPosition += new Vector2(0f, 50f * Time.fixedDeltaTime);

        if (progress > 1f)
        {
            if (timeSinceCompletion > 5f)
            {
                Destroy(gameObject);
            }
            else
            {
                timeSinceCompletion += Time.fixedDeltaTime * 0.5f;
            }
        } 
    }

    public void SetConversation(ConversationMedium conversationMedium)
    {
        cm = conversationMedium;
    }
    
    public void SetText(string text)
    {
        this.text = text;
        ugui.text = null;
    }

    public void SetColor(Color color)
    {
        ugui.color = color;
    }

    public void SetLead(bool lead)
    {
        amLead = lead;
        if (lead)
        {
            audio.pitch = 1.2f;
        }
        else
        {
            audio.panStereo = 0.5f;
        }

        audio.pitch += Random.Range(-0.05f, 0.05f);
        SetAlignment(lead);
    }
    
    public void SetAlignment(bool left)
    {
        ugui.alignment = left ? TextAlignmentOptions.TopLeft : TextAlignmentOptions.TopRight;
    }

    public bool UpdateProgress(float progress)
    {
        int oldLength = Mathf.Clamp(Mathf.FloorToInt(text.Length * this.progress), 0, text.Length);
        this.progress = progress;
        int newLength = Mathf.Clamp(Mathf.FloorToInt(text.Length * this.progress), 0, text.Length);
        
        if (newLength > oldLength)
        {
            UpdateText(newLength);

            string letter = "";
            string lastLetter = "";
            for (int i = oldLength; i < newLength; i++)
            {
                string checkLetter = text[i].ToString().ToUpper();
                lastLetter = checkLetter;
                letter = checkLetter switch
                {
                    "A" => checkLetter,
                    "E" => checkLetter,
                    "I" => checkLetter,
                    "O" => checkLetter,
                    "U" => checkLetter,
                    _ => letter
                };
            }

            if (!amLead)
            {
                if (cm.animationController != null) {cm.animationController.MouthShape(lastLetter);}
            }

            AudioClip clip = cm.audioClipStorage.GetClip(letter);
            if (clip != null)
            {
                audio.clip = clip;
                audio.Play();
            }
        }

        if (this.progress >= 1f)
        {
            audio.Stop();
            return true;
        }
        return false;
    }

    private void UpdateText(int n)
    {
        ugui.maxVisibleCharacters = n;
        ugui.text = text;
        waveAndFade.fadeEnd = n;
    }
}
