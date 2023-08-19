using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using Thirst.Managers;
using Thirst.Models;

namespace Thirst.Behavior
{
    internal class ThirstBehavior : CampaignBehaviorBase
    {

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.DailyTickParty));
            CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
            CampaignEvents.OnTutorialCompletedEvent.AddNonSerializedListener(this, new Action<string>(this.OnTutorialCompleted));
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailyTickSettlement));
            CampaignEvents.OnGameEarlyLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameEarlyLoaded));
            CampaignEvents.MobilePartyCreated.AddNonSerializedListener(this, new Action<MobileParty>(this.MobilePartyCreated));
            CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.MobilePartyDestroyed));
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
        }

        private void OnNewGameCreated(CampaignGameStarter starter)
        {
            SubModule.uiExtender.Register(typeof(SubModule).Assembly);
            SubModule.uiExtender.Enable();
            SubModule.thirst.InitializeItems();
            SubModule.thirst.InitializeParties();
            //SubModule.thirst.InitializeSettlements();
            AddWaterOnGameStart();
        }
        private void OnGameEarlyLoaded(CampaignGameStarter gameStarter)
        {
            SubModule.uiExtender.Register(typeof(SubModule).Assembly);
            SubModule.uiExtender.Enable();
            SubModule.thirst.InitializeItems();
            SubModule.thirst.InitializeParties();
            //SubModule.thirst.InitializeSettlements();
            SubModule.thirst.InitializePlayer();
        }

        private void OnTutorialCompleted(string tutorial)
        {
            SubModule.thirst.InitializeItems();
            SubModule.thirst.InitializeParties();
            //SubModule.thirst.InitializeSettlements();
            AddWaterOnGameStart();
            ThirstManager.IsWaterGiven = true;
        }

        private void MobilePartyCreated(MobileParty party)
        {
            if (!ThirstManager.partyThirst.ContainsKey(party))
            {
                ThirstManager.partyThirst[party] = new PartyWaterConsumptionModel();
            }
        }

        private void MobilePartyDestroyed(MobileParty party, PartyBase partyBase)
        {
            if (ThirstManager.partyThirst.ContainsKey(party))
            {
                ThirstManager.partyThirst.Remove(party);
            }
        }

        private void DailyTickParty(MobileParty party)
        {
            if (ThirstManager.partyThirst.ContainsKey(party))
            {
                ThirstManager.partyThirst[party].PartyConsumeWater(party);
            }
        }

        private void OnGameLoadFinished()
        {
            SubModule.uiExtender.Enable();
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("PartyThirstData", ref ThirstManager.partyThirst);
            //dataStore.SyncData("SettlementThirstData", ref ThirstManager.settlementThirst);
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
            if (!ThirstManager.IsWaterGiven)
            {
                MobileParty.MainParty.ItemRoster.AddToCounts(SubModule.thirst.Water, 2);
                AddWaterToSettlements();
                AddWaterToParties();
                ThirstManager.IsWaterGiven = true;
            }
        }

        private void DailyTickSettlement(Settlement settlement)
        {
            if (ThirstManager.settlementThirst.ContainsKey(settlement))
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
                        waterToAdd = SubModule.thirst.Water;
                        settlement.ItemRoster.AddToCounts(waterToAdd, count * 3);
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
