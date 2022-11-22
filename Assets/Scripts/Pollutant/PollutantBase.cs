using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PollutantBase : MonoBehaviour
{
    [SerializeField] protected PollutantData pollutantData;
    [SerializeField] protected bool playerInVicinity;

    public static event Action<PollutantType> OnPollutantCollected;

    public bool PlayerInVicinity { set => playerInVicinity = value; }

    protected abstract PollutantType PollutantType { get; }
    
    void OnMouseDown()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        CollectPollutant();   
    }

    protected abstract void CollectPollutant();

    protected void RaiseCollectPollutant(PollutantType pollutant)
    {
        OnPollutantCollected?.Invoke(pollutant);
    }
}