using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ParallaxHandler : MonoBehaviour
{
    float startPosition;
    float length;
    public Camera cam;

    [SerializeField] bool loop;
    [SerializeField] float parallaxEffect;
    void Start()
    {
        startPosition = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    void Update()
    {
        Vector3 camPos = cam.transform.position;
        float distance = (camPos.x * parallaxEffect);
        Vector3 pos = transform.position;
        transform.position = (new Vector3(startPosition + distance, pos.y, pos.z));

        if (!loop)
            return;

        float temp = (camPos.x * (1 - parallaxEffect));
        if (temp > startPosition + length)
            startPosition += length;
        else if (temp < startPosition - length)
            startPosition -= length;
    }
}
