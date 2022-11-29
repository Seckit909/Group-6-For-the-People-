namespace P106.Main.Pollutant
{
	public class OilSpillPollutant : PollutantBase
	{
		public override PollutantType PollutantType => PollutantType.OilSpill;

		protected override void CollectPollutant()
		{
			if (!playerInVicinity) return;
			RaiseCollectPollutant(PollutantType);
			Destroy(gameObject);
		}
	}
}

