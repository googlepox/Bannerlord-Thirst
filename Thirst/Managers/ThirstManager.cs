using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameComponents;
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
        public Dictionary<MobileParty, PartyWaterConsumptionModel> partyThirst;
        
        [SaveableField(2)]
        public Dictionary<Settlement, SettlementWaterConsumptionModel> settlementThirst;

        public WaterItemCategory waterItemCategory; 
        public WaterItem waterItem;

        public ItemCategory WaterCat;

        public ItemObject Water;

        public ItemObject[] waterTypes;

        public PartyWaterConsumptionModel mainModel;

        public ThirstManager()
        {
            this.partyThirst = new Dictionary<MobileParty, PartyWaterConsumptionModel>();
            this.settlementThirst = new Dictionary<Settlement, SettlementWaterConsumptionModel>();
            this.waterTypes = new ItemObject[5];
            mainModel = new PartyWaterConsumptionModel();
        }

        public void InitializePlayer()
        {
            if (!partyThirst.ContainsKey(MobileParty.MainParty))
            {
                partyThirst.Add(MobileParty.MainParty, mainModel);
            }
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

        public void InitializeParties()
        {
            foreach (MobileParty party in MobileParty.All)
            {
                if (!SubModule.thirst.partyThirst.ContainsKey(party))
                {
                    SubModule.thirst.partyThirst.Add(party, new PartyWaterConsumptionModel());
                }
            }
        }

        public void InitializeSettlements()
        {
            foreach (Settlement settlement in Settlement.All)
            {
                if (!SubModule.thirst.settlementThirst.ContainsKey(settlement))
                {
                    SubModule.thirst.settlementThirst.Add(settlement, new SettlementWaterConsumptionModel());
                }
            }
        }

        public void ReInitializeParty(MobileParty mobileParty)
        {
            if (!SubModule.thirst.partyThirst.ContainsKey(mobileParty))
            {
                SubModule.thirst.partyThirst.Add(mobileParty, new PartyWaterConsumptionModel());
            }
        }

        public void ReInitializeSettlement(Settlement settlement)
        {
            if (!SubModule.thirst.settlementThirst.ContainsKey(settlement))
            {
                SubModule.thirst.settlementThirst.Add(settlement, new SettlementWaterConsumptionModel());
            }
        }
    }
}
