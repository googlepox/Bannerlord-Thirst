using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.SaveSystem;
using Thirst.Managers;
using Thirst.Models;

namespace Thirst
{
    internal class ThirstSaveDefiner : SaveableTypeDefiner
    {

        public ThirstSaveDefiner() : base(685372151)
        {

        }

        protected override void DefineClassTypes()
        {
            this.AddClassDefinition(typeof(PartyWaterConsumptionModel), 1);
            this.AddClassDefinition(typeof(ThirstManager), 2);
        }

        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(Dictionary<MobileParty, PartyWaterConsumptionModel>));
            ConstructContainerDefinition(typeof(Dictionary<Settlement, PartyWaterConsumptionModel>));
        }

    }
}
