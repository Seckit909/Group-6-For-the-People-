using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PollutantDataUI : MonoBehaviour
{
	[SerializeField] RectTransform imageGameObject;
	[SerializeField] RectTransform textGameObject;
	[SerializeField] PollutantData data;

	void OnValidate()
	{
		if (!data)
		{
			if(imageGameObject)
				imageGameObject.GetComponent<Image>().sprite = null;
			if (!textGameObject) return;
			var tmpText = textGameObject.GetComponent<TMP_Text>();
			tmpText.color = Color.red;
			tmpText.text = "NO DATA";
		}
		else
		{
			if(imageGameObject)
				imageGameObject.GetComponent<Image>().sprite = data.Icon;
			if (!textGameObject) return;
			var tmpText = textGameObject.GetComponent<TMP_Text>();
			tmpText.color = Color.black;
			tmpText.text = data.ResourceCount.ToString();
		}
	}

	void OnEnable()
	{ 
		OnValidate();
	}
}