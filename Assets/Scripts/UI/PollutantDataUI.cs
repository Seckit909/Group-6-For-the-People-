using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PollutantDataUI : MonoBehaviour
{
	[SerializeField] RectTransform imageGameObject;
	[SerializeField] RectTransform textGameObject;
	[SerializeField] PollutantData data;

	[SerializeField] Sprite pollutantIcon;
	[SerializeField] TMP_Text textMeshProText;
	
	PollutantType pollutantType;

	void OnValidate()
	{
		if (!data)
		{
			pollutantType = PollutantType.NONE;
			if(imageGameObject)
				imageGameObject.GetComponent<Image>().sprite = null;
			if (!textGameObject) return;
			var tmpText = textGameObject.GetComponent<TMP_Text>();
			tmpText.color = Color.red;
			tmpText.text = "NO DATA";
		}
		else
		{
			pollutantType = data.PollutantType;
			if(imageGameObject)
				imageGameObject.GetComponent<Image>().sprite = data.Icon;
			if (!textGameObject) return;
			var tmpText = textGameObject.GetComponent<TMP_Text>();
			tmpText.color = Color.black;
			tmpText.text = data.ResourceCount.ToString();
		}
	}

	void Awake()
	{
		textMeshProText = GetComponentInChildren<TMP_Text>();
		pollutantIcon = GetComponentInChildren<Image>().sprite;
	}

	void Start()
	{
		if (!data) return;
		if (!pollutantIcon) return;
		pollutantIcon = data.Icon;
	}

	void OnEnable()
	{
		PollutantBase.OnPollutantCollected += UpdatePollutantUI;
		//OnValidate();
	}

	void OnDisable()
	{
		PollutantBase.OnPollutantCollected -= UpdatePollutantUI;
	}

	void UpdatePollutantUI(PollutantType type)
	{
		if (type != pollutantType) return;
		textMeshProText.text = data.ResourceCount.ToString();
	}
}