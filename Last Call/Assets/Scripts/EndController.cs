using System.Collections;
using UnityEngine;

public class EndController : MonoBehaviour
{
    public AudioSource song;
    public Manager manager;
    public GameObject cover;

    private void Awake()
    {
        StartEndingSequence();
        cover.SetActive(true);
    }

    private void StartEndingSequence()
    {
        song.Play();
        StartCoroutine(EndSequence(song.clip.length + 35f));
    }

    private IEnumerator EndSequence(float wait)
    {
        yield return new WaitForSeconds(8f);
        cover.SetActive(false);
        yield return new WaitForSeconds(wait - 8f);
        manager.CallEvent(9);
    }
}
