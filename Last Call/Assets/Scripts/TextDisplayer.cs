using TMPro;
using UnityEngine;

public class TextDisplayer : MonoBehaviour
{
    private TextJitterer jitterer;
    private BoxCollider2D collider;
    private TextMeshProUGUI textDiplay;

    private string text;
    private bool triggered;

    public Option option;

    public void Init(Option option)
    {
        this.option = option;
        this.text = option.text;

        UpdateText();
    }

    void UpdateText()
    {
        textDiplay.text = text;
    }
    
    void Awake()
    {
        textDiplay = gameObject.GetComponent<TextMeshProUGUI>();
        
        triggered = false;
        
        jitterer = gameObject.AddComponent<TextJitterer>();
        jitterer.activated = triggered;

        collider = gameObject.AddComponent<BoxCollider2D>();
        collider.size = GetComponent<RectTransform>().sizeDelta;
        collider.isTrigger = true;
    }

    private void FixedUpdate()
    {
        jitterer.activated = triggered;
        triggered = false;
    }

    public void Hover()
    {
        triggered = true;
    }

    public void Select()
    {
        Events.ChooseOption(option);
        Events.RemoveOption(option);
    }
}
