using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using Thirst.Models;

namespace Thirst.Behavior
{
	internal class ThirstBehavior : CampaignBehaviorBase
	{

        public override void RegisterEvents()
		{
            CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.DailyTickParty));
            //CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
            CampaignEvents.OnTutorialCompletedEvent.AddNonSerializedListener(this, new Action<string>(this.OnTutorialCompleted));
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailyTickSettlement));
            CampaignEvents.OnGameEarlyLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameEarlyLoaded));
            CampaignEvents.MobilePartyCreated.AddNonSerializedListener(this, new Action<MobileParty>(this.MobilePartyCreated));
            CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.MobilePartyDestroyed));
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
        }

        private void OnNewGameCreated(CampaignGameStarter starter)
        {
            SubModule.thirst.InitializeItems();
            SubModule.thirst.InitializeParties();
            SubModule.thirst.InitializeSettlements();
            AddWaterOnGameStart();
        }
        private void OnGameEarlyLoaded(CampaignGameStarter gameStarter)
        {
            SubModule.thirst.InitializeItems();
            SubModule.thirst.InitializeParties();
            SubModule.thirst.InitializeSettlements();
            SubModule.thirst.InitializePlayer();
        }

        private void OnTutorialCompleted(string tutorial)
        {
            SubModule.thirst.InitializeItems();
            SubModule.thirst.InitializeParties();
            SubModule.thirst.InitializeSettlements();
            MobileParty party = MobileParty.MainParty;
            party.ItemRoster.AddToCounts(SubModule.thirst.Water, 3);
            AddWaterOnGameStart();
            PartyWaterConsumptionModel.IsWaterGiven = true;
        }

        private void MobilePartyCreated(MobileParty party)
        {
            if (!SubModule.thirst.partyThirst.ContainsKey(party))
            {
                SubModule.thirst.partyThirst[party] = new PartyWaterConsumptionModel();
            }
        }

        private void MobilePartyDestroyed(MobileParty party, PartyBase partyBase)
        {
            if (SubModule.thirst.partyThirst.ContainsKey(party))
            {
                SubModule.thirst.partyThirst.Remove(party);
            }
        }

        private void DailyTickParty(MobileParty party)
        {
            if (SubModule.thirst.partyThirst.ContainsKey(party))
            {
                SubModule.thirst.partyThirst[party].PartyConsumeWater(party);
            }
        }

        private void OnGameLoadFinished()
        {
            if (!PartyWaterConsumptionModel.IsWaterGiven)
            {
                AddWaterOnGameStart();
                PartyWaterConsumptionModel.IsWaterGiven = true;
            }
        }

        public override void SyncData(IDataStore dataStore)
		{
			
		}

        private void AddWaterToParties()
        {
            foreach (MobileParty party in MobileParty.All)
            {
                int sections = 0;
                for (int i = 0; i < party.MemberRoster.TotalManCount/20; i++)
                {
                    sections++;
                }
                int waterType = MBRandom.RandomInt(0, 5);
                while (sections > 0)
                {
                    party.ItemRoster.AddToCounts(SubModule.thirst.Water, 4);
                }
            }
        }

        private void AddWaterToSettlements()
        {
            foreach (Settlement settlement in Settlement.All)
            {
                int count = MBRandom.RandomInt(25, 50);
                int waterType = MBRandom.RandomInt(0, 5);
                settlement.ItemRoster.AddToCounts(SubModule.thirst.Water, count);
            }
        }

        public void AddWaterOnGameStart()
        {
            if (!PartyWaterConsumptionModel.IsWaterGiven)
            {
                MobileParty.MainParty.ItemRoster.AddToCounts(SubModule.thirst.Water, 2);
                AddWaterToSettlements();
                PartyWaterConsumptionModel.IsWaterGiven = true;
            }
        }

        private void DailyTickSettlement(Settlement settlement)
        {
            if (SubModule.thirst.settlementThirst.ContainsKey(settlement))
            {
                int count = MBRandom.RandomInt(1, 5);
                bool hasAqueduct = false;
                ItemObject waterToAdd;
                if (settlement.IsTown)
                {
                    foreach (Building building in settlement.Town.Buildings)
                    {
                        if (building.BuildingType == DefaultBuildingTypes.SettlementAquaducts)
                        {
                            hasAqueduct = true;
                        }
                    }

                    if (hasAqueduct)
                    {
                        if (hasAqueduct)
                        {
                            waterToAdd = SubModule.thirst.Water;
                            settlement.ItemRoster.AddToCounts(waterToAdd, count * 3);
                        }
                    }
                    else
                    {
                        waterToAdd = SubModule.thirst.Water;
                        settlement.ItemRoster.AddToCounts(waterToAdd, count*2);
                    }
                }
                else
                {
                    waterToAdd = SubModule.thirst.Water;
                    settlement.ItemRoster.AddToCounts(waterToAdd, count);
                }
            }
            else
            {
                SubModule.thirst.ReInitializeSettlement(settlement);
            }
        }
    }
}
