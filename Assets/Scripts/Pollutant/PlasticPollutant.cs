namespace P106.Main.Pollutant
{
    public class PlasticPollutant : PollutantBase
    {
        public override PollutantType PollutantType => PollutantType.Plastic;

        protected override void CollectPollutant()
        {
            if (!playerInVicinity) return;
            RaiseCollectPollutant(PollutantType);
            Destroy(gameObject);
        }
    }
}