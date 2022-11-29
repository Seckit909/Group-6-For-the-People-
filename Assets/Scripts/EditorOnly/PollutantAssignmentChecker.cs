#if UNITY_EDITOR

using System;
using P106.Main.Pollutant;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace P106.Main.EditorOnly
{
	public class PollutantAssignmentChecker : MonoBehaviour
	{
		[Header("Hvis feltet er tomt; vælg den ønskede pollutantType")]
		[SerializeField] PollutantType pollutantType = PollutantType.NONE;

		void OnValidate()
		{
			if (PrefabStageUtility.GetCurrentPrefabStage() != null) return;
			TryAddComponent();
		}

		void TryAddComponent()
		{
			if (!TryGetComponent(out PollutantBase pollutant))
			{
				PollutantBase newComponent = pollutantType switch
				{
					PollutantType.OilSpill => gameObject.AddComponent<OilSpillPollutant>(),
					PollutantType.Plastic => gameObject.AddComponent<PlasticPollutant>(),
					PollutantType.Ammonia => throw new NotImplementedException("Ammonia Pollutant komponent er endnu ikke implementeret."),
					PollutantType.NONE => null,
					_ => null
				};
				if (pollutantType is PollutantType.NONE)
				{
					Debug.LogError($"{pollutantType} er ikke gyldig. Please vælg en anden.", this);
					return;
				}

				if (newComponent == null)
				{
					Debug.LogError("Forsøgte at tilføje et ugyldigt PollutantBase komponent, som resulterede i en NULL-værdi.\n" +
					               $"{pollutantType} er muligvis ugyldig. Prøv at vælge en anden.", this);
					return;
				}
			}

			RemoveThisComponentFromGameObject(pollutant);
		}

		void RemoveThisComponentFromGameObject(PollutantBase pollutant)
		{
			if (pollutant == null || pollutant.PollutantType is PollutantType.NONE || pollutantType is PollutantType.NONE) return;
			Debug.Log($"Fjerner {nameof(PollutantAssignmentChecker)} fra Pollutant GameObject, {gameObject.name},\n" +
			          $"da det har et komponent af typen {pollutant.GetType()}, med en gyldig {nameof(PollutantType)}; {pollutant.PollutantType}.", gameObject);
			UnityEditor.EditorApplication.delayCall += ()=> DestroyImmediate(this);
		}
	}
}

#endif