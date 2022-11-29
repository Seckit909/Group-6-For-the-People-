using System;
using UnityEngine;

namespace P106.Main.Pollutant
{
	[CreateAssetMenu(fileName = "New Resource Data", menuName = "P1/Resource/New Resource", order = 0)]
	public class ResourceData : ScriptableObject
	{
		[SerializeField] Sprite resourceIcon;
		[SerializeField] PollutantData pollutant;

		public Sprite ResourceIcon => resourceIcon;
		public PollutantData Pollutant => pollutant;

		public int ResourceCount => pollutant != null ? pollutant.ResourceCount : 00;
	} 
}