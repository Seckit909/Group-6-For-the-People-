using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

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

    private void Awake()
    {
        cam = Camera.main;
    }

    public void pinkCounter()
    {
        pinkCollected += 1;
        pink.text = "" + pinkCollected;
    }
    public void redCounter()
    {
        redCollected += 1;
        red.text = "" + redCollected;
    }
    public void greenCounter()
    {
        greenCollected += 1;
        green.text = "" + greenCollected;
    }

    private void OnTriggerEnter2D(Collider2D point)
    {
        var mousePos = InputReader.GetMousePosition();
        Debug.Log($"{mousePos}");

        if (point.gameObject.CompareTag("pink"))
        {
            pinkCounter();
            Debug.Log("Yes it pink");
        }

        if (point.gameObject.CompareTag("red"))
        {
            redCounter();
            Debug.Log("Yes it red");
        }
        
        if (point.gameObject.CompareTag("green"))
        { 
            greenCounter();
            Debug.Log("Yes it blue");
        }

    }
}
