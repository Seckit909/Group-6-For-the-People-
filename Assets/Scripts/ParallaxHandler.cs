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
        float distance = (cam.transform.position.x * parallaxEffect);

        transform.position = (new Vector3(startPosition + distance, transform.position.y, transform.position.z));

        if (loop)
        {
            float temp = (cam.transform.position.x * (1 - parallaxEffect));
            if (temp > startPosition + length)
                startPosition += length;
            else if (temp < startPosition - length)
                startPosition -= length;
        }
    }
}
