using UnityEngine;

namespace P106.Main.Player
{
	[CreateAssetMenu(menuName = "P106/Data/Player")]
	public class PlayerData : ScriptableObject
	{
		Vector2 playerPosition;

		public Vector2 PlayerPosition { get => playerPosition; set => playerPosition = value; }

		public void ResetPlayerPositionData() => playerPosition = Vector2.zero;
	}
}