using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using Thirst.Managers;

namespace Thirst.Behavior
{
    internal class PartyThirstBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnGameEarlyLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameEarlyLoaded));
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
            CampaignEvents.MobilePartyCreated.AddNonSerializedListener(this, new Action<MobileParty>(this.MobilePartyCreated));
            CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.MobilePartyDestroyed));
            CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.DailyTickParty));
            CampaignEvents.SettlementEntered.AddNonSerializedListener((object)this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("PartyThirstData", ref ThirstManager.partyThirst);
            dataStore.SyncData("ThirstIsWaterGiven", ref ThirstManager.IsWaterGiven);
            dataStore.SyncData("MainPartyRemainingWaterPercentage", ref ThirstManager.mainPartyRemainingWaterPercentage);
            dataStore.SyncData("MainPartyIsDehydrated", ref ThirstManager.mainPartyIsDehydrated);
        }

        private void OnNewGameCreated(CampaignGameStarter starter)
        {
            PartyThirstManager.InitializeParties();
            ThirstManager.AddWaterOnGameStart();
            ThirstManager.mainPartyRemainingWaterPercentage = 0;
            ThirstManager.mainPartyIsDehydrated = false;
        }

        private void OnGameEarlyLoaded(CampaignGameStarter gameStarter)
        {
            PartyThirstManager.InitializeParties();
            ThirstManager.AddWaterOnGameStart();
            PartyThirstManager.InitializePlayer();
        }

        private void MobilePartyCreated(MobileParty mobileParty)
        {
            if (!ThirstManager.partyThirst.ContainsKey(mobileParty))
            {
                ThirstManager.partyThirst[mobileParty] = new ThirstData(0, false);
            }
        }

        private void MobilePartyDestroyed(MobileParty mobileParty, PartyBase partyBase)
        {
            if (ThirstManager.partyThirst.ContainsKey(mobileParty))
            {
                ThirstManager.partyThirst.Remove(mobileParty);
            }
        }

        private void DailyTickParty(MobileParty mobileParty)
        {
            if (mobileParty != null)
            {
                if (mobileParty.CurrentSettlement != null)
                {
                    PartyWaterBuyingManager.TryBuyingWater(mobileParty, mobileParty.CurrentSettlement);
                }
                PartyThirstManager.PartyConsumeWater(mobileParty);
            }
        }

        public void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
        {
            if (mobileParty != null && settlement != null)
            {
                PartyWaterBuyingManager.TryBuyingWater(mobileParty, settlement);
            }
        }
    }
}
