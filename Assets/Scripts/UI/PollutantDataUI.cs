using P106.Main.Pollutant;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace P106.Main.UI
{
	public class PollutantDataUI : MonoBehaviour
	{
		[SerializeField] PollutantData data;
		[SerializeField] Image imageComponent;
		[SerializeField] TMP_Text textMeshProText;

		PollutantType pollutantType;

		public Sprite SpriteIcon
		{
			get
			{
				imageComponent ??= GetComponentInChildren<Image>();
				return imageComponent.sprite;
			}
			set
			{
				imageComponent ??= GetComponentInChildren<Image>();
				imageComponent.sprite = value;
			}
		}

		public int PollutantCount
		{
			get => int.TryParse(textMeshProText.text, out int count) 
				? count 
				: -1;
			set => textMeshProText.text = value.ToString("n0");
		}

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
			textMeshProText ??= GetComponentInChildren<TMP_Text>();
			imageComponent ??= GetComponentInChildren<Image>();
		}

		void Start()
		{
			if (!data) return;
			if (!imageComponent) return;
			SpriteIcon = data.Icon;
		}

		void UpdatePollutantUI(PollutantType type)
		{
			if (type != pollutantType) return;
			textMeshProText.text = data.ResourceCount.ToString();
		}
	}
}