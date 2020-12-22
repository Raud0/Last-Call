using System.Collections.Generic;
using UnityEngine;

public class OptionPresenter : MonoBehaviour
{

    public GameObject DisplayerPrefab;
    public List<TextDisplayer> Displayers;

    private void Awake()
    {
        Events.OnAddOption += LoadOption;
        Events.OnRemoveOption += RemoveOption;
    }

    private void OnDestroy()
    {
        Events.OnAddOption -= LoadOption;
        Events.OnRemoveOption -= RemoveOption;
    }

    private void LoadOption(Option option)
    {
        TextDisplayer textDisplayer = Instantiate(DisplayerPrefab,transform).GetComponent<TextDisplayer>();
        textDisplayer.Init(option);
        
        Displayers.Add(textDisplayer);   
    }

    private void RemoveOption(Option option)
    {
        TextDisplayer textDisplayer = null;
        foreach (TextDisplayer displayer in Displayers)
        {
            if (displayer.option.Equals(option))
            {
                textDisplayer = displayer;
                break;
            }
        }

        if (textDisplayer != null)
        {
            Displayers.Remove(textDisplayer);
            
            Destroy(textDisplayer.gameObject);
        }
    }
}
