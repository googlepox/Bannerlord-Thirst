using TaleWorlds.CampaignSystem;

namespace Thirst.Behavior
{
    internal class WaterItemBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            //CampaignEvents.OnGameEarlyLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameEarlyLoaded));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void OnGameEarlyLoaded(CampaignGameStarter gameStarter)
        {

        }
    }
}
