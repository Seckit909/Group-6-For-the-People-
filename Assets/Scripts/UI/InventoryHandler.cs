using P106.Main.Pollutant;
using UnityEngine;
using TMPro;
using Color = UnityEngine.Color;

namespace P106.Main.Player
{
    public class InventoryHandler : MonoBehaviour
    {
        Camera cam;
        public TextMeshProUGUI pink;
        public TextMeshProUGUI red;
        public TextMeshProUGUI green;

        //int pinkCollected;
        //int redCollected;
        //int greenCollected;

        void Awake()
        {
            cam = Camera.main;
        }

        static void PollutionCounter(int currentCount, TextMeshProUGUI pollutantText)
        {
            currentCount += 1;
            pollutantText.color = Color.black;
            pollutantText.text = currentCount.ToString();
        }

        void OnTriggerExit2D(Collider2D point)
        {
            if (!point.gameObject.CompareTag("Pollutant")) return;
            var pollutant = point.gameObject.GetComponent<PollutantBase>();
            pollutant.PlayerInVicinity = false;
        }

        void OnTriggerEnter2D(Collider2D point)
        {
            if (!point.gameObject.CompareTag("Pollutant")) return;
            var pollutant = point.gameObject.GetComponent<PollutantBase>();
            pollutant.PlayerInVicinity = true;

            //    var tag = point.gameObject.tag;
            //    switch (tag)
            //    {
            //        case "pink":
            //            PollutionCounter(pinkCollected, pink);
            //            Debug.Log("Yes it pink");
            //            break;
            //        case "red":
            //            PollutionCounter(redCollected, red);
            //            Debug.Log("Yes it red");
            //            break;
            //        case "green":
            //            PollutionCounter(greenCollected, green);
            //            Debug.Log("Yes it green");
            //            break;

            //    }
        }
    }
}
