using UnityEngine;
public class FullScreenSprite : MonoBehaviour {
    
    // Kyle Banks. Create a Fullscreen Background Image in Unity2D with a SpriteRenderer. 24.12.2016
    // https://kylewbanks.com/blog/create-fullscreen-background-image-in-unity2d-with-spriterenderer

    void Awake() {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        
        float cameraHeight = Camera.main.orthographicSize * 2;
        Vector2 cameraSize = new Vector2(Camera.main.aspect * cameraHeight, cameraHeight);
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        
        Vector2 scale = transform.localScale;
        if (false) { // Landscape (or equal)
            scale *= cameraSize.x / spriteSize.x;
        } else { // Portrait
            scale *= cameraSize.y / spriteSize.y;
        }
        
        transform.position = Vector2.zero; // Optional
        transform.localScale = scale;
    }
}
