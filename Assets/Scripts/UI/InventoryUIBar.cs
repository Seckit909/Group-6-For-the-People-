using System.Collections.Generic;
using System.Linq;
using P106.Main.Pollutant;
using UnityEngine;

namespace P106.Main.UI
{
	public class InventoryUIBar : MonoBehaviour
	{
		[SerializeField] List<ResourceData> resources = Enumerable.Empty<ResourceData>().ToList();

		void Start()
		{
			for (int i = 0; i < resources.Count; i++)
			{
				var resourcePrefab = Instantiate(resources[i].ResourceUIPrefab, transform);
				resourcePrefab.GetComponent<PollutantDataUI>().PollutantIcon = resources[i].ResourceIcon;
				resourcePrefab.GetComponent<PollutantDataUI>().TextMeshProText.text = resources[i].Pollutant.ResourceCount.ToString();

			}   
		}
	}

}