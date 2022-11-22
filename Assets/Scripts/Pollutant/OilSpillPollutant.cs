﻿public class OilSpillPollutant : PollutantBase
{
    protected override PollutantType PollutantType => PollutantType.OilSpill;

    protected override void CollectPollutant()
    {
        if (!playerInVicinity) return;
        RaiseCollectPollutant(PollutantType);
        Destroy(gameObject);
    }
}
