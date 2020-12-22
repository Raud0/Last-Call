using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI clockText;
    public TextMeshProUGUI choseText;
    
    private void Awake()
    {
        Events.OnUpdateTime += UpdateTime;
        Events.OnChooseOption += DisplayChosenOption;
    }
    private void OnDestroy()
    {
        Events.OnUpdateTime -= UpdateTime;
        Events.OnChooseOption -= DisplayChosenOption;
    }


    private string intToString(int val)
    {
        string valS = val.ToString();
        while (valS.Length < 2)
        {
            valS = "0" + valS;
        }

        if (valS.Length > 2)
        {
            valS = "##";
        }

        return valS;
    }
    
    private void UpdateTime(int minutes, int seconds)
    {
        string minutesS = intToString(minutes);
        string secondsS = intToString(seconds);    
        
        
        
        clockText.text = minutesS + ":" + secondsS;
    }
    
    private void DisplayChosenOption(Option option)
    {
        choseText.text = option.text;
    }
}
