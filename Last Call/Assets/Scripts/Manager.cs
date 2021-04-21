using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public AudioSource phone;
    public AudioSource gun;
    public GameObject jackObject;
    public AnimationController animationController;
    public RoomController roomController;

    public bool exitStarted = false;

    private void Update()
    {
        if (Input.GetKey(KeyCode.G))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) CallEvent(1);
            if (Input.GetKeyDown(KeyCode.Alpha2)) CallEvent(2);
            if (Input.GetKeyDown(KeyCode.Alpha3)) CallEvent(3);
            if (Input.GetKeyDown(KeyCode.Alpha4)) CallEvent(4);
            if (Input.GetKeyDown(KeyCode.Alpha5)) CallEvent(5);
            if (Input.GetKeyDown(KeyCode.Alpha6)) CallEvent(6);
            if (Input.GetKeyDown(KeyCode.Alpha7)) CallEvent(7);
            if (Input.GetKeyDown(KeyCode.Alpha8)) CallEvent(8);
            if (Input.GetKeyDown(KeyCode.Alpha9)) CallEvent(9);
        }
    }

    public void CallEvent(int number)
    {
        if (number == -1 || number == 0) return;
        Debug.Log("Event " + number + " called.");
        switch (number)
        {
            case 1: StartGame(); break;
            case 2: TurnOnPhone(); break;
            case 6: MoveToEndEarly(); break;
            case 7: KillJack(); break;
            case 8: MoveToEnd(); break;
            case 9: ExitEarly(); break;
        }
    }

    private void MoveToEndEarly()
    {
        StartCoroutine(MoveToScene(2, 5f));
    }

    private void MoveToEnd()
    {
        StartCoroutine(MoveToScene(2, 0f));
    }

    private void ExitEarly()
    {
        if (exitStarted) return;
        exitStarted = true;
        StartCoroutine(EndWithABang(3f));
    }

    private void KillJack()
    {
        StartCoroutine(Shoot(2f));
    }

    private void TurnOnPhone()
    {
        StartCoroutine(LoudPhone(300, 0.5f, 0.01f));
    }

    private void StartGame()
    {
        StartCoroutine(MoveToScene(1, 5f));
    }
    
    private IEnumerator MoveToScene(int scene, float wait)
    {
        yield return new WaitForSeconds(wait);
        SceneManager.LoadScene(scene);
    }
    
    private IEnumerator LoudPhone(int times, float wait, float mod)
    {
        phone.Play();
        for (int i = 0; i < times; i++)
        {
            yield return new WaitForSeconds(wait);
            phone.volume += mod;
        }
    }

    private IEnumerator Shoot(float wait)
    {
        yield return new WaitForSeconds(wait);
        gun.Play();
        if (jackObject != null)
        {
            jackObject.SetActive(false);
        }
        if (animationController != null)
        {
            animationController.IsDead(true);
        }
    }
    
    private IEnumerator EndWithABang(float wait)
    {
        yield return new WaitForSeconds(wait * 1f/3f);
        if (roomController != null) roomController.GoBlank();
        yield return new WaitForSeconds(wait * 1f/3f);
        if (gun != null) gun.Play();
        yield return new WaitForSeconds(wait * 1f/3f);
        QuitGame();
    }

    private void QuitGame()
    {
        Time.timeScale = 0f;
        Application.Quit();
    }
}
