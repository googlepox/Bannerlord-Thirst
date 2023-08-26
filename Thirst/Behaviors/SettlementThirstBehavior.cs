using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace Thirst.Behaviors
{
    internal class SettlementThirstBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailyTickSettlement));
        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void DailyTickSettlement(Settlement settlement)
        {
            int count = MBRandom.RandomInt(1, 10);
            bool hasAqueduct = false;
            ItemObject waterToAdd = MBObjectManager.Instance.GetObject<ItemObject>(x => x.ItemCategory.StringId == "water");
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
                    settlement.ItemRoster.AddToCounts(waterToAdd, count * 3);
                }
                else
                {
                    settlement.ItemRoster.AddToCounts(waterToAdd, count*2);
                }
            }
            else
            {
                settlement.ItemRoster.AddToCounts(waterToAdd, count);
            }
        }

    }
}
