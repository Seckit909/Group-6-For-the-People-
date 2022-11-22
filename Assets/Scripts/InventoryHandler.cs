using UnityEngine;
using TMPro;

public class InventoryHandler : MonoBehaviour
{
    Camera cam;
    public TextMeshProUGUI pink;
    public TextMeshProUGUI red;
    public TextMeshProUGUI green;

    public GameObject pinkPlastic;

    int pinkCollected = 0;
    int redCollected = 0;
    int greenCollected = 0;

    void Awake()
    {
        cam = Camera.main;
    }

    void PinkCounter()
    {
        pinkCollected += 1;
        pink.text = "" + pinkCollected;
    }
    
    void RedCounter()
    {
        redCollected += 1;
        red.text = "" + redCollected;
    }
    
    void GreenCounter()
    {
        greenCollected += 1;
        green.text = "" + greenCollected;
    }

    void OnTriggerEnter2D(Collider2D point)
    {
        if (point.gameObject.CompareTag("pink"))
        {
            PinkCounter();
            Debug.Log("Yes it pink");
        }

        if (point.gameObject.CompareTag("red"))
        {
            RedCounter();
            Debug.Log("Yes it red");
        }
        
        if (point.gameObject.CompareTag("green"))
        { 
            GreenCounter();
            Debug.Log("Yes it blue");
        }
    }
}
