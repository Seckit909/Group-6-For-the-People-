using System.Collections.Generic;
using System.Linq;
using P106.Main.Pollutant;
using UnityEngine;

namespace P106.Main.UI
{
	public class InventoryUIBar : MonoBehaviour
	{
		[SerializeField] GameObject resourceViewPrefab;
		[SerializeField] List<ResourceData> resources = Enumerable.Empty<ResourceData>().ToList();

		readonly Dictionary<ResourceData, GameObject> resourceViewDict = new();

		void Start()
		{
			PopulateResourceViewDictionary();
			InitResourceViews();
		}
		
		void PopulateResourceViewDictionary()
		{
			foreach (var resource in resources)
			{
				RemoveDictionaryEntry(resource);
				var resourceView = Instantiate(resourceViewPrefab, transform);
				resourceViewDict.Add(resource, resourceView);
			}
		}

		void InitResourceViews()
		{
			for (int i = 0; i < resources.Count; i++)
			{
				if (!resourceViewDict.TryGetValue(resources[i], out GameObject resource)) return;
				SetResourceViewIcon(resource, i);
				UpdateResourceViewCounter();
			}
		}
		
		void UpdateResourceViewCounter()
		{
			foreach (var (resource, view) in resourceViewDict)
				view.GetComponent<PollutantDataUI>().PollutantCount = resource.ResourceCount;
		}
		
		// TODO Lav custom script med direkte reference til PollutantDataUI
		void SetResourceViewIcon(GameObject resource, int i) => resource.GetComponent<PollutantDataUI>().SpriteIcon = resources[i].ResourceIcon;

		void RemoveDictionaryEntry(ResourceData resource)
		{
			if (!resourceViewDict.TryGetValue(resource, out GameObject view)) return;
			resourceViewDict.Remove(resource); // TODO GENNEMGÅ 0(1) OPERATION
			Destroy(view);
		}
	}
}