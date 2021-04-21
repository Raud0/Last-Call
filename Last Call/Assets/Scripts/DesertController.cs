using UnityEngine;

public class DesertController : MonoBehaviour
{
    public GameObject grass;
    public GameObject cloud;
    public GameObject mountains;
    public GameObject sky;
    
    private void Update()
    {
        float x_clouds = Mathf.Sin(Time.time / 30f) * 5f;
        cloud.transform.localPosition = new Vector3(x_clouds, 0f, 0f);
        
        float x_grass = Mathf.Sin(Time.time / 10f) * 0.01f +
                        Mathf.Sin(Time.time / 1f + 0.12312f) * 0.01f;
        grass.transform.localPosition = new Vector3(x_grass, 0f, 0f);
    }
}
