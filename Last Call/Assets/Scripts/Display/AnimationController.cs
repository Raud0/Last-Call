using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public bool exist = false;
    public bool dead = false;
    
    public GameObject all;

    public GameObject smoke;
    public GameObject body;
    
    public GameObject hands;
    public GameObject handsUp;
    public GameObject handsDown;
    public GameObject head;
    public GameObject headHalf;
    public GameObject eyebrows;
    public GameObject glasses;
    public GameObject mouth;
    
    public GameObject mouthClosed;
    public GameObject mouthLax;
    public GameObject mouthTense;
    public GameObject mouthOpen;

    private float mouthTime = 0f;
    private float mouthTimeCap = 0.5f;

    private bool smoking = false;
    private float cigaretteTime = 0f;
    private float cigaretteTimeCap = 30f;

    private float fear = 0f;
    private float ego = 0f;
    private float anger = 0f;

    public void MouthShape(string letter)
    {
        if (!exist) return;
        
        int mode = 0;
        switch (letter)
        {
            case "A": case "E": case "L": case "N":
            case "X":
                mode = 3; break;
            case "O": case "T": case "S": case "C":
            case "D": case "R":
                mode = 1; break;
            case "W": case "U": case "Q": case "F":
            case "V": case "Y": case "Z": case "K":
                mode = 2; break;
        }   
        
        mouthClosed.SetActive(mode == 0);
        mouthLax.SetActive(mode == 1);
        mouthTense.SetActive(mode == 2);
        mouthOpen.SetActive(mode == 3);

        if (mode != 0) mouthTime = mouthTimeCap;
    }

    public void Cigarette()
    {
        if (!smoking)
        {
            cigaretteTime = cigaretteTimeCap * (0.2f + fear * 0.8f);
        }
        else
        {
            cigaretteTime = cigaretteTimeCap * (0.2f + (1 - fear) * 0.8f);
        }

        smoking = !smoking;
        handsUp.SetActive(smoking);
        handsDown.SetActive(!smoking);
    }

    private void Update()
    {
        if (!exist) return;
        
        head.SetActive(!dead);
        headHalf.SetActive(dead);

        if (!dead)
        {
            if (mouthTime > 0f)
            {
                mouthTime -= Time.deltaTime;
            }
            else
            {
                MouthShape("");
            }

            if (cigaretteTime > 0f)
            {
                cigaretteTime -= Time.deltaTime;
            }
            else
            {
                Cigarette();
            }
        }

        float y_body = (Mathf.Sin(Time.time) - 1f) * 0.01f;
        float x_body = 0f;
        if (dead)
        {
            y_body = 0f;
            x_body = (Mathf.Cos(Time.time)) * 0.04f;
        }
        body.transform.localPosition = new Vector3(x_body, y_body, 0f);
        
        float y_head = (Mathf.Sin(-Time.time) - 1f) * 0.01f;
        float x_head = 0f;
        if (dead)
        {
            y_head = 0f;
            x_head = (Mathf.Cos(-Time.time)) * 0.04f;
        }
        head.transform.localPosition = new Vector3(x_head, y_head, 0f);
        headHalf.transform.localPosition = new Vector3(x_head, y_head, 0f);

        if (dead)
        {
            float x_hands = (Mathf.Cos(Time.time)) * 0.02f;;
            hands.transform.localPosition = new Vector3(x_hands, 0f, 0f);
        }
        
        float y_brows = (1f - anger) * -0.35f;
        eyebrows.transform.localPosition = new Vector3(0f, y_brows, 0f);

        float y_glasses = ego * -1.5f + 0.5f;
        glasses.transform.localPosition = new Vector3(0f, y_glasses, 0f);

        float x_smoke = (Mathf.Sin(Time.time + 0.31232f)) * 0.02f;
        float y_smoke = (Mathf.Sin(Time.time + 1.1231f) - 1f) * 0.01f;
        smoke.transform.localPosition = new Vector3(x_smoke, y_smoke, 0f);
    }

    public void UpdateFear(float fear)
    {
        this.fear = fear;
    }

    public void UpdateAnger(float anger)
    {
        this.anger = anger;
    }

    public void UpdateEgo(float ego)
    {
        this.ego = ego;
    }

    public void IsDead(bool dead)
    {
        this.dead = dead;
    }
}
