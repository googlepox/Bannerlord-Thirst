using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;
using Thirst.ItemCategories;
using Thirst.Models;
using Thirst.WaterItems;

namespace Thirst.Managers
{
    public class ThirstManager
    {
        [SaveableField(1)]
        public static Dictionary<MobileParty, PartyWaterConsumptionModel> partyThirst;

        [SaveableField(2)]
        public static Dictionary<Settlement, SettlementWaterConsumptionModel> settlementThirst;

        [SaveableField(3)]
        public static bool IsWaterGiven = false;

        public WaterItemCategory waterItemCategory;
        public WaterItem waterItem;

        public ItemCategory WaterCat;

        public ItemObject Water;

        public ItemObject[] waterTypes;

        public PartyWaterConsumptionModel mainModel;

        public ThirstManager()
        {
            partyThirst = new Dictionary<MobileParty, PartyWaterConsumptionModel>();
            settlementThirst = new Dictionary<Settlement, SettlementWaterConsumptionModel>();
            this.waterTypes = new ItemObject[5];
            this.mainModel = new PartyWaterConsumptionModel();
        }

        public void InitializeItemCategories()
        {
            foreach (ItemCategory itemCat in Game.Current.ObjectManager.GetObjectTypeList<ItemCategory>())
            {
                if (itemCat.StringId == "water")
                {
                    WaterCat = itemCat;
                }
            }
        }

        public void InitializeItems()
        {
            foreach (ItemObject item in Items.All)
            {
                if (item.StringId == "water")
                {
                    Water = item;
                }
            }
        }

        public void InitializePlayer()
        {
            if (!partyThirst.ContainsKey(MobileParty.MainParty))
            {
                partyThirst.Add(MobileParty.MainParty, mainModel);
            }
        }

        public void InitializeParties()
        {
            if (partyThirst.Count > 0)
            {
                return;
            }

            foreach (MobileParty party in MobileParty.All)
            {
                if (!ThirstManager.partyThirst.ContainsKey(party))
                {
                    ThirstManager.partyThirst.Add(party, new PartyWaterConsumptionModel());
                }
            }
        }

        public void InitializeSettlements()
        {
            if (settlementThirst.Count > 0)
            {
                return;
            }

            foreach (Settlement settlement in Settlement.All)
            {
                if (!ThirstManager.settlementThirst.ContainsKey(settlement))
                {
                    ThirstManager.settlementThirst.Add(settlement, new SettlementWaterConsumptionModel());
                }
            }
        }

        public void ReInitializeParty(MobileParty mobileParty)
        {
            if (!ThirstManager.partyThirst.ContainsKey(mobileParty))
            {
                ThirstManager.partyThirst.Add(mobileParty, new PartyWaterConsumptionModel());
            }
        }

        public void ReInitializeSettlement(Settlement settlement)
        {
            if (!ThirstManager.settlementThirst.ContainsKey(settlement))
            {
                ThirstManager.settlementThirst.Add(settlement, new SettlementWaterConsumptionModel());
            }
        }
    }
}
