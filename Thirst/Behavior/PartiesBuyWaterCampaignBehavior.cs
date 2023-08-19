using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.LinQuick;
using Thirst.Managers;
using Thirst.Models;

namespace Thirst.Behavior
{
    internal class PartiesBuyWaterCampaignBehavior : CampaignBehaviorBase
    {
        public PartyWaterBuyingModel buyModel = SubModule.buyModel;

        public override void RegisterEvents()
        {
            CampaignEvents.SettlementEntered.AddNonSerializedListener((object)this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
            CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener((object)this, new Action<MobileParty>(this.DailyTick));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void TryBuyingWater(MobileParty mobileParty, Settlement settlement)
        {
            int totalWaterSettlement = ThirstManager.settlementThirst[settlement].GetWater(settlement);
            if (!Campaign.Current.GameStarted || mobileParty.LeaderHero == null || !settlement.IsTown && !settlement.IsVillage || mobileParty.Army != null && mobileParty.Army.LeaderParty != mobileParty || !settlement.IsVillage && (mobileParty.MapFaction == null || mobileParty.MapFaction.IsAtWarWith(settlement.MapFaction)) || totalWaterSettlement <= 0)
            {
                return;
            }

            float minimumDaysToLast = settlement.IsVillage ? buyModel.MinimumDaysWaterToLastWhileBuyingWaterFromVillage : buyModel.MinimumDaysWaterToLastWhileBuyingWaterFromTown;
            if (mobileParty.Army == null)
            {
                this.BuyWaterInternal(mobileParty, settlement, this.CalculateWaterCountToBuy(mobileParty, minimumDaysToLast));
            }
            else
            {
                this.BuyWaterForArmy(mobileParty, settlement, minimumDaysToLast);
            }
        }

        private int CalculateWaterCountToBuy(MobileParty mobileParty, float minimumDaysToLast)
        {
            float num1 = (float)ThirstManager.partyThirst[mobileParty].GetWater(mobileParty) / -ThirstManager.partyThirst[mobileParty].GetWaterChange(mobileParty);
            float num2 = minimumDaysToLast - num1;
            return (double)num2 > 0.0 ? (int)(-(double)ThirstManager.partyThirst[mobileParty].GetWaterChange(mobileParty) * (double)num2) : 0;
        }

        private void BuyWaterInternal(
          MobileParty mobileParty,
          Settlement settlement,
          int numberOfWaterItemsNeededToBuy)
        {
            if (mobileParty.IsMainParty)
            {
                return;
            }

            for (int index = 0; index < numberOfWaterItemsNeededToBuy; ++index)
            {
                ItemRosterElement itemRosterElement;
                float itemElementsPrice;
                buyModel.FindItemToBuy(mobileParty, settlement, out itemRosterElement, out itemElementsPrice);
                EquipmentElement equipmentElement = itemRosterElement.EquipmentElement;
                if (equipmentElement.Item == null)
                {
                    break;
                }

                if ((double)itemElementsPrice <= (double)mobileParty.LeaderHero.Gold)
                {
                    SellItemsAction.Apply(settlement.Party, mobileParty.Party, itemRosterElement, 1);
                }

                equipmentElement = itemRosterElement.EquipmentElement;
            }
        }

        private void BuyWaterForArmy(
          MobileParty mobileParty,
          Settlement settlement,
          float minimumDaysToLast)
        {
            float num1 = mobileParty.Army.Parties.SumQ<MobileParty>((Func<MobileParty, float>)(x => ThirstManager.partyThirst[x].GetWaterChange(x)));
            List<(int, int)> valueTupleList = new List<(int, int)>(mobileParty.Army.Parties.Count);
            float num2 = 0.0f;
            foreach (MobileParty andAttachedParty in mobileParty.Army.Parties)
            {
                float num3 = ThirstManager.partyThirst[andAttachedParty].GetWaterChange(andAttachedParty) / num1;
                int waterCountToBuy = this.CalculateWaterCountToBuy(andAttachedParty, minimumDaysToLast);
                valueTupleList.Add(((int)((double)ThirstManager.settlementThirst[settlement].GetWater(settlement) * (double)num3), waterCountToBuy));
                num2 += (float)waterCountToBuy;
            }
            bool flag = (double)ThirstManager.settlementThirst[settlement].GetWater(settlement) < (double)num2;
            int index = 0;
            foreach ((int, int) valueTuple in valueTupleList)
            {
                int numberOfWaterItemsNeededToBuy = flag ? valueTuple.Item1 : valueTuple.Item2;
                MobileParty mobileParty1 = mobileParty.Army.Parties.ElementAt<MobileParty>(index);
                if (!mobileParty1.IsMainParty)
                {
                    this.BuyWaterInternal(mobileParty1, settlement, numberOfWaterItemsNeededToBuy);
                }

                ++index;
            }
        }

        public void DailyTick(MobileParty mobileParty)
        {
            if (mobileParty != null && mobileParty.CurrentSettlement != null &&
                ThirstManager.partyThirst.ContainsKey(mobileParty) && ThirstManager.settlementThirst.ContainsKey(mobileParty.CurrentSettlement))
            {
                this.TryBuyingWater(mobileParty, mobileParty.CurrentSettlement);
            }
        }

        public void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
        {
            if (mobileParty != null && settlement != null && ThirstManager.partyThirst.ContainsKey(mobileParty) && ThirstManager.settlementThirst.ContainsKey(settlement))
            {
                this.TryBuyingWater(mobileParty, settlement);
            }
        }
    }
}
