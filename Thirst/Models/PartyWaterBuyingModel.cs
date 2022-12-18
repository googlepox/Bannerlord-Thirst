using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace Thirst.Models
{
    public class PartyWaterBuyingModel
    {

        public float MinimumDaysWaterToLastWhileBuyingWaterFromTown => 30f;

        public float MinimumDaysWaterToLastWhileBuyingWaterFromVillage => 8f;

        public float LowCostWaterPriceAverage => 15f;

        public void FindItemToBuy(
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
                        equipmentElement.Item.ItemCategory.StringId == "mead")
                    {
                        int itemPrice = settlementComponent.GetItemPrice(elementCopyAtIndex.EquipmentElement, mobileParty);
                        equipmentElement = elementCopyAtIndex.EquipmentElement;
                        int itemValue = equipmentElement.ItemValue;
                        if (mobileParty != null && mobileParty.LeaderHero != null && itemPrice < 120 && mobileParty.LeaderHero.Gold >= itemPrice)
                        {
                            double num3;
                            num3 = (double)(120 - itemPrice) * 0.0082999998703598976;
                            double num5;
                            num5 = (double)(100 - itemValue) * 0.0099999997764825821;
                            float num7 = (float)num5;
                            float num8 = (float)(num3 * num3) * num7 * num7;
                            if ((double)num8 > 0.0)
                            {
                                if ((double)MBRandom.RandomFloat * ((double)num1 + (double)num8) >= (double)num1)
                                {
                                    index1 = index2;
                                    itemElementsPrice = (float)itemPrice;
                                }
                                num1 += num8;
                            }
                        }
                    }
                }
            }
            if (index1 == -1)
                return;
            itemElement = settlement.ItemRoster.GetElementCopyAtIndex(index1);
        }

    }
}
