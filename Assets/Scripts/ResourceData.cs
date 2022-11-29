using UnityEngine;

namespace P106.Main.Pollutant
{
	[CreateAssetMenu(fileName = "New Resource Data", menuName = "P1/Resource/New Resource", order = 0)]
	public class ResourceData : ScriptableObject
	{
		[SerializeField] GameObject resourceUIPrefab;
		[Tooltip("64x64 sprite")]
		[SerializeField] Sprite resourceIcon;

		[SerializeField] PollutantData pollutant;

		public PollutantData Pollutant => pollutant;
		public GameObject ResourceUIPrefab => resourceUIPrefab;
		public Sprite ResourceIcon => resourceIcon;
	} 
}