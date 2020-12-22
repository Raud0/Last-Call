using UnityEngine;

public class Control : MonoBehaviour
{
    
    Camera cam;
    private GameObject pointer;
    
    private void Start()
    {
        cam = Camera.main;
        pointer = new GameObject();
        pointer.AddComponent<CircleCollider2D>();
        pointer.AddComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        CheckMouse();
    }

    void CheckMouse()
    {
        Vector3 mousePosition = Input.mousePosition;

        Ray ray = cam.ScreenPointToRay(mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        
        if (hit.collider != null)
        {
            TextDisplayer textDisplayer = hit.collider.gameObject.GetComponent<TextDisplayer>();
            if (textDisplayer != null)
            {
                textDisplayer.Hover();
                if (Input.GetMouseButton(0))
                {
                    textDisplayer.Select();
                }
            }
        }
    }
}
