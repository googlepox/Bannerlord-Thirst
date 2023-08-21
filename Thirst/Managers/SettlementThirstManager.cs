using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace Thirst.Managers
{
    internal class SettlementThirstManager
    {
        public static void AddWaterToSettlements()
        {
            foreach (Settlement settlement in Settlement.All)
            {
                int count = MBRandom.RandomInt(25, 50);
                settlement.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>(x => x.ItemCategory.StringId == "water"), count);
            }
        }

        public static int GetWater(Settlement settlement)
        {
            int waterItems = 0;
            for (int index = 0; index < settlement.ItemRoster.Count; ++index)
            {
                ItemRosterElement itemRosterElement = settlement.ItemRoster[index];
                if (!itemRosterElement.IsEmpty)
                {
                    EquipmentElement equipmentElement = itemRosterElement.EquipmentElement;
                    ItemObject itemObject1 = equipmentElement.Item;
                    if (itemObject1 == MBObjectManager.Instance.GetObject<ItemObject>(x => x.ItemCategory.StringId == "water"))
                    {
                        waterItems += itemRosterElement.Amount;
                    }
                    else if (itemObject1.ItemCategory.StringId == "bf_mead" || itemObject1.ItemCategory.StringId == "mead" || itemObject1.ItemCategory.StringId == "beer" || itemObject1.ItemCategory.StringId == "wine")
                    {
                        waterItems += (itemRosterElement.Amount);
                    }
                }
            }
            return waterItems;
        }
    }
}
