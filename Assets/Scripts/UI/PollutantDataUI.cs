using P106.Main.Pollutant;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace P106.Main.UI
{
	public class PollutantDataUI : MonoBehaviour
	{
		[SerializeField] RectTransform imageGameObject;
		[SerializeField] RectTransform textGameObject;
		[SerializeField] PollutantData data;

		[SerializeField] Sprite pollutantIcon;
		[SerializeField] TMP_Text textMeshProText;

		PollutantType pollutantType;

		public Sprite PollutantIcon { get => pollutantIcon; set => pollutantIcon = value; }
		public TMP_Text TextMeshProText => textMeshProText;

		void OnEnable()
		{
			PollutantBase.OnPollutantCollected += UpdatePollutantUI;
		}

		void OnDisable()
		{
			PollutantBase.OnPollutantCollected -= UpdatePollutantUI;
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

		void UpdatePollutantUI(PollutantType type)
		{
			if (type != pollutantType) return;
			textMeshProText.text = data.ResourceCount.ToString();
		}
	}
}