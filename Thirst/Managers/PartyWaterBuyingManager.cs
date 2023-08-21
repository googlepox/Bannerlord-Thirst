using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.LinQuick;

namespace Thirst.Managers
{
    internal class PartyWaterBuyingManager
    {
        public static float MinimumDaysWaterToLastWhileBuyingWaterFromTown => 30f;

        public static float MinimumDaysWaterToLastWhileBuyingWaterFromVillage => 8f;

        public static float LowCostWaterPriceAverage => 15f;


        public static void TryBuyingWater(MobileParty mobileParty, Settlement settlement)
        {
            int totalWaterSettlement = SettlementThirstManager.GetWater(settlement);
            if (!Campaign.Current.GameStarted || mobileParty.LeaderHero == null || !settlement.IsTown && !settlement.IsVillage || mobileParty.Army != null && mobileParty.Army.LeaderParty != mobileParty || !settlement.IsVillage && (mobileParty.MapFaction == null || mobileParty.MapFaction.IsAtWarWith(settlement.MapFaction)) || totalWaterSettlement <= 0)
            {
                return;
            }

            float minimumDaysToLast = settlement.IsVillage ? MinimumDaysWaterToLastWhileBuyingWaterFromVillage : MinimumDaysWaterToLastWhileBuyingWaterFromTown;
            if (mobileParty.Army == null)
            {
                BuyWaterInternal(mobileParty, settlement, CalculateWaterCountToBuy(mobileParty, minimumDaysToLast));
            }
            else
            {
                BuyWaterForArmy(mobileParty, settlement, minimumDaysToLast);
            }
        }

        public static int CalculateWaterCountToBuy(MobileParty mobileParty, float minimumDaysToLast)
        {
            float num1 = PartyThirstManager.GetWater(mobileParty) / -PartyThirstManager.GetWaterChange(mobileParty);
            float num2 = minimumDaysToLast - num1;
            return num2 > 0.0 ? (int)(-PartyThirstManager.GetWaterChange(mobileParty) * num2) : 0;
        }

        private static void BuyWaterInternal(
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
                FindItemToBuy(mobileParty, settlement, out itemRosterElement, out itemElementsPrice);
                EquipmentElement equipmentElement = itemRosterElement.EquipmentElement;
                if (equipmentElement.Item == null)
                {
                    break;
                }

                if (itemElementsPrice <= mobileParty.LeaderHero.Gold)
                {
                    SellItemsAction.Apply(settlement.Party, mobileParty.Party, itemRosterElement, 1);
                }
            }
        }

        private static void BuyWaterForArmy(
          MobileParty mobileParty,
          Settlement settlement,
          float minimumDaysToLast)
        {
            float num1 = mobileParty.Army.Parties.SumQ(x => PartyThirstManager.GetWaterChange(x));
            List<(int, int)> valueTupleList = new List<(int, int)>(mobileParty.Army.Parties.Count);
            float num2 = 0.0f;
            foreach (MobileParty andAttachedParty in mobileParty.Army.Parties)
            {
                float num3 = PartyThirstManager.GetWaterChange(andAttachedParty) / num1;
                int waterCountToBuy = CalculateWaterCountToBuy(andAttachedParty, minimumDaysToLast);
                valueTupleList.Add(((int)(SettlementThirstManager.GetWater(settlement) * num3), waterCountToBuy));
                num2 += waterCountToBuy;
            }
            bool flag = SettlementThirstManager.GetWater(settlement) < num2;
            int index = 0;
            foreach ((int, int) valueTuple in valueTupleList)
            {
                int numberOfWaterItemsNeededToBuy = flag ? valueTuple.Item1 : valueTuple.Item2;
                MobileParty mobileParty1 = mobileParty.Army.Parties.ElementAt<MobileParty>(index);
                if (!mobileParty1.IsMainParty)
                {
                    BuyWaterInternal(mobileParty1, settlement, numberOfWaterItemsNeededToBuy);
                }

                ++index;
            }
        }

        public static ItemRosterElement FindItemToBuy(
          MobileParty mobileParty,
          Settlement settlement,
          out ItemRosterElement itemElement,
          out float itemElementsPrice)
        {
            itemElement = ItemRosterElement.Invalid;
            itemElementsPrice = 0.0f;
            float num1 = 0.0f;
            SettlementComponent settlementComponent = settlement.SettlementComponent;
            int index1 = -1;
            for (int index2 = 0; index2 < settlement.ItemRoster.Count; ++index2)
            {
                ItemRosterElement elementCopyAtIndex = settlement.ItemRoster.GetElementCopyAtIndex(index2);
                if (elementCopyAtIndex.Amount > 0)
                {
                    if (elementCopyAtIndex.EquipmentElement.Item == null)
                    {
                        settlement.ItemRoster.Remove(elementCopyAtIndex);
                    }
                    EquipmentElement equipmentElement = elementCopyAtIndex.EquipmentElement;
                    if (equipmentElement.Item.ItemCategory.StringId == "water" ||
                        equipmentElement.Item.ItemCategory.StringId == "wine" ||
                        equipmentElement.Item.ItemCategory.StringId == "beer" ||
                        equipmentElement.Item.ItemCategory.StringId == "mead" ||
                        equipmentElement.Item.ItemCategory.StringId == "bf_mead")
                    {
                        int itemPrice = settlementComponent.GetItemPrice(elementCopyAtIndex.EquipmentElement, mobileParty);
                        equipmentElement = elementCopyAtIndex.EquipmentElement;
                        int itemValue = equipmentElement.ItemValue;
                        if (mobileParty != null && mobileParty.LeaderHero != null && itemPrice < 120 && mobileParty.LeaderHero.Gold >= itemPrice)
                        {
                            double num3;
                            num3 = (120 - itemPrice) * 0.0082999998703598976;
                            double num5;
                            num5 = (100 - itemValue) * 0.0099999997764825821;
                            float num7 = (float)num5;
                            float num8 = (float)(num3 * num3) * num7 * num7;
                            if (num8 > 0.0)
                            {
                                if (MBRandom.RandomFloat * (num1 + num8) >= num1)
                                {
                                    index1 = index2;
                                    itemElementsPrice = itemPrice;
                                }
                                num1 += num8;
                            }
                        }
                    }
                }
            }
            if (index1 == -1)
            {
                return ItemRosterElement.Invalid;
            }

            return settlement.ItemRoster.GetElementCopyAtIndex(index1);
        }
    }
}
