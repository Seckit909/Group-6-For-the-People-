using UnityEngine;

namespace P106.Main.Pollutant
{
	[CreateAssetMenu(fileName = "New Pollutant Data", menuName = "P106/Data/Pollutant")]
	public class PollutantData : ScriptableObject
	{
		[SerializeField] int resourceCount;
		[SerializeField] Sprite resourceIcon;
		[SerializeField] PollutantType pollutantType;

		public int ResourceCount { get => resourceCount; set => resourceCount = value; }
		public Sprite Icon => resourceIcon;
		public PollutantType PollutantType => pollutantType;

		void OnValidate()
		{
			if (pollutantType is not PollutantType.NONE) return;
			Debug.LogError($"PollutantData {name} har ikke en gyldig pollutantType. Lige nu er den NONE. Det går sgu ikke.", this);
		}

		void OnEnable()
		{
			PollutantBase.OnPollutantCollected += CollectPollutant;
			ResetData();
		}

		void OnDisable()
		{
			PollutantBase.OnPollutantCollected -= CollectPollutant;
		}

		void ResetData()
		{
			resourceCount = 0;
		}

		void CollectPollutant(PollutantType type)
		{
			if (type != pollutantType) return;
			resourceCount += 1;
		}
	}
}