public class PlasticPollutant : PollutantBase
{
    protected override PollutantType PollutantType => PollutantType.Plastic;

    protected override void CollectPollutant()
    {
        if (!playerInVicinity) return;
        RaiseCollectPollutant(PollutantType);
        Destroy(gameObject);
    }
}
