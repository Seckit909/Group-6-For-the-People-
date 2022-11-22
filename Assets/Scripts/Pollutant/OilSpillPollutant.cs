using System.Collections;
using UnityEngine;

public class OilSpillPollutant : PollutantBase
{
    protected override void OnDerivedMouseDown()
    {
        pollutantData.ResourceCount += 1;
        RaisePollutantDataUpdated();
        Destroy(gameObject);
    }
}
