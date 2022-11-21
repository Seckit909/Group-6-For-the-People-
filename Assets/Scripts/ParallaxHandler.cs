using UnityEngine;

public class ParallaxHandler : MonoBehaviour
{
    [SerializeField] bool doLoop = true;
    [SerializeField] float parallaxEffect;
    
    float startPosition;
    float length;
    Camera mainCam;
    SpriteRenderer spriteRenderer;
    
    Vector3 Position { get => transform.position; set => transform.position = value; }
    
    void Awake()
    {
        mainCam = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        startPosition = transform.position.x;
        length = spriteRenderer.bounds.size.x;
    }
    
    void Update()
    {
        Vector3 camPos = mainCam.transform.position;
        float distance = (camPos.x * parallaxEffect);
        Position = new Vector3(startPosition + distance, Position.y, Position.z);
        if (!doLoop) return;

        float temp = camPos.x * (1 - parallaxEffect);
        
        if (temp > startPosition + length)
            startPosition += length;
        else if (temp < startPosition - length)
            startPosition -= length;
    }
}