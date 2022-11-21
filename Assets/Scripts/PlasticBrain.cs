using UnityEngine;

public class PlasticBrain : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D point)
    {
        if (point.gameObject.CompareTag("Player"))
            Destroy(gameObject);
    }
}
