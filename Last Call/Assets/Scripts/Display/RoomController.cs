using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomController : MonoBehaviour
{
    public Ticker ticker;
    
    public GameObject wall;
    public GameObject geometry;
    public GameObject table;
    public GameObject chair;
    public GameObject cushion;
    public GameObject sign;
    public GameObject clock;
    public GameObject cover;

    public int stage = 0;
    
    private void Awake()
    {
        StartCoroutine(OpenCovers(1f));
    }

    private void Update()
    {
        int secondsLeft = ticker.GetSecondsLeft();

        stage = stage switch
        {
            0 when secondsLeft <= 900 => 1,
            1 when secondsLeft <= 600 => 2,
            2 when secondsLeft <= 300 => 3,
            3 when secondsLeft <= 5 => 4,
            _ => stage
        };

        switch (stage)
        {
            case 0: StageZero(); break;
            case 1: StageOne(); break;
            case 2: StageTwo(); break;
            case 3: StageThree(); break;
            case 4: StageFour(); break;
        }   
    }

    private bool Test(float N, float T)
    {
        return Mathf.Pow(Mathf.Pow(Mathf.PerlinNoise(Time.time * 0.11f + 0.1905f, Time.time * 1.17f + 0.123f), 2) + 0.5f, 1f/N) > T;
    }
    
    private void StageZero()
    {
        //NOTHING HAPPENS :P
        chair.SetActive(true);
        cushion.SetActive(true);
        geometry.SetActive(true);
        wall.SetActive(true);
        sign.SetActive(true);
        table.SetActive(true);
    }
    
    private void StageOne()
    {
        StageZero();
        
    }

    private void StageTwo()
    {
        StageOne();
        geometry.SetActive(false);
    }

    private void StageThree()
    {
        StageTwo();
        wall.SetActive(false);
        sign.SetActive(false);
        chair.SetActive(false);
        cushion.SetActive(false);
        table.SetActive(Random.Range(0f, 1000f) > 5f);
    }

    private void StageFour()
    {
        StageThree();
        table.SetActive(Random.Range(0f, 1000f) > 300f);
        clock.transform.localPosition = new Vector3(100f, 100f, 0f);
    }

    private IEnumerator OpenCovers(float wait)
    {
        yield return new WaitForSeconds(wait);
        cover.SetActive(false);
    }

    public void GoBlank()
    {
        cover.SetActive(true);
    }
}
