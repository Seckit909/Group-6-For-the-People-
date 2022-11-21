using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
    Vector2 playerPosition;

    public Vector2 PlayerPosition { get => playerPosition; set => playerPosition = value; }

    public void ResetPlayerPositionData() => playerPosition = Vector2.zero;

}
