using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class PollutantData : ScriptableObject
{
	[SerializeField] int resourceCount;
	[Tooltip("64x64 sprite, som skal vises i Resource-bar'en.")]
	[SerializeField] Sprite resourceIcon;

	public int ResourceCount { get => resourceCount; set => resourceCount = value; }
	public Sprite Icon => resourceIcon;
}
