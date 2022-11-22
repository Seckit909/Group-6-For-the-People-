using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PollutantBase : MonoBehaviour
{
    [SerializeField] protected PollutantData pollutantData;
    [SerializeField] protected bool playerInVicinity;

    public static event Action<PollutantData> OnPollutantDataUpdated;

    public bool PlayerInVicinity { set => playerInVicinity = value; }


    void OnMouseDown()
    {
        OnDerivedMouseDown();
    }

    protected abstract void OnDerivedMouseDown();

    protected void RaisePollutantDataUpdated()
    {
        pollutantData.ResourceCount += 1;
        OnPollutantDataUpdated?.Invoke(pollutantData);
    }
}